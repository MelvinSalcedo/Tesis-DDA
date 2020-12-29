using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyStates : MonoBehaviour {

    [Header("Caracteristicas")]
   
    public int FuiHerodoRecientemente = 0;
    public float health=100;
    public int RecientementeBloqueeUnAtaque = 0;

    [Header("Stats")]
    public Slider sliderHealt;
    public GameObject CambasSliderHealt;

    [Header("values")]
    public float delta;
    public float horizontal;
    public float vertical;

    

    [Header("States")]
    public bool isInvicible;
    public bool canMove;

    public bool isDead;
    public bool hasDestination;
    public Vector3 targetDestination;
    public Vector3 dirTotarget;
    public bool rotateToTarget;

    public Animator anim;
    EnemyTarget enTarget;
    AnimtorHook a_hook;
    public Rigidbody rigid;
    public NavMeshAgent agent;

    

    public LayerMask ignoreLayers;

    List<Rigidbody> ragdollRigs=new List<Rigidbody>();
    List<Collider> ragdollCollider= new List<Collider>();


    [Header("private Data")]
    private bool isShield;
    private bool trigger_countShield=false;
    private float random;
   // private DataRecolected dataRecolected;
    private string updateAnimation;

    public void Init() {
        health = 100;
        anim = GetComponentInChildren<Animator>();
        enTarget = GetComponent<EnemyTarget>();
        enTarget.Init(this);

        rigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        rigid.isKinematic = true;

        a_hook = anim.GetComponent<AnimtorHook>();
        if (a_hook == null)
            a_hook = anim.gameObject.AddComponent<AnimtorHook>();
        a_hook.Init(null, this);
        InitRagDoll();
        //dataRecolected = GameObject.Find("DataRecolected").GetComponent<DataRecolected>();
        ignoreLayers = ~(1 << 9);
    }

    void InitRagDoll() {
        Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigs.Length; i++) {
            if (rigs[i] == rigid)
                continue;
            ragdollRigs.Add(rigs[i]);
            rigs[i].isKinematic = true;

            Collider col = rigs[i].GetComponent<Collider>();
            col.isTrigger = true;
            ragdollCollider.Add(col);
        }
    }

    public void EnableRagDoll() {
        

        for (int i = 0; i < ragdollRigs.Count; i++) {
            ragdollRigs[i].isKinematic = false;
            ragdollCollider[i].isTrigger = false;
        }
        Collider controllerCollider=rigid.gameObject.GetComponent<Collider>();
        controllerCollider.enabled = false;

        StartCoroutine("CloseAnimator");


    }

    IEnumerator CloseAnimator() {
        yield return new WaitForEndOfFrame();
        anim.enabled = false;
        this.enabled = false;
        DataRecolected.instancia.derrotaaNpcDda += 1;
        DataRecolected.instancia.asignarCLases();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    public void Tick(float d) {
        delta = d;

        canMove = anim.GetBool("canMove");
        
        if ((health<35) && rotateToTarget) {
            anim.SetBool("block", true);
            isShield = true;
        }

        if (rotateToTarget) {
            LookTowardsTarget();
            random = Random.Range(0,1);

        }
        LookSliderTowardTarget();
        if (health <= 0) {
            if (!isDead) {
                isDead = true;
                EnableRagDoll();
            }
        }

        if (isInvicible) {
            isInvicible = !canMove;
        }
        if (canMove) {//delete somtign
            anim.applyRootMotion = false;
            MoveAnimation();
        }
        else {
            if(anim.applyRootMotion==false)
                anim.applyRootMotion = true;
        }
    }

    public void updateStrinAccionRetroceso(string s) {
        updateAnimation = s;
    }

    public void MoveAnimation() {
        //anim.SetBool("lockOn", true);



        if (updateAnimation == "retirarse") {
            anim.SetBool("lockOn", true);

            anim.SetFloat("vertical", -0.8f, 0.2f, delta);
        }
        else {
            anim.SetBool("lockOn", false);
            float square = agent.desiredVelocity.sqrMagnitude;
            float v = Mathf.Clamp(square, 0, .5f);
            anim.SetFloat("vertical", v, 0.2f, delta);
        }

    }

    public void LookTowardsTarget() {
        Vector3 dir = dirTotarget;
        dir.y = 0;
        if (dir == Vector3.zero)
            dir = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * 5);
    }

    public void LookSliderTowardTarget() {
        Vector3 dir = dirTotarget;
        dir.y = 0;
        if (dir == Vector3.zero)
            dir = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        CambasSliderHealt.transform.rotation = 
            Quaternion.Slerp(CambasSliderHealt.transform.rotation, targetRotation, delta * 5);
    }

    public void SetDestinatio(Vector3 d) {

        if (!hasDestination) {
            hasDestination = true;
            agent.isStopped = false;
            agent.SetDestination(d);
            targetDestination = d;
        }

    }

    
    public void DoDamage(float v, float tipe) {
        if (isInvicible)
            return;

        if (tipe >= 0.2f) {
            if (isShield == true && !rotateToTarget)
                health -= v / 3;
            else
                health -= v;
            anim.Play("damage_1");
        }
        else {
            //anim.Play("Step_back");
            float rr = Random.Range(0f,1f);
            if (rr < 0.5f) {
                DataRecolected.instancia.EsquivarFa += 1;
                health -= v;
            }
            else
                DataRecolected.instancia.EsquivarEx += 1;
        }

        anim.applyRootMotion = true;
        anim.SetBool("canMove",false);

        sliderHealt.value = (int)health;
        isInvicible = true;
    }
}
