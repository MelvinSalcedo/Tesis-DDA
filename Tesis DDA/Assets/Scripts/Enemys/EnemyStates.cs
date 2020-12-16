using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private DataRecolected dataRecolected;

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
        dataRecolected = GameObject.Find("DataRecolected").GetComponent<DataRecolected>();
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
    }

    public void Tick(float d) {
        delta = d;

        canMove = anim.GetBool("canMove");
        //canMove = anim.GetBool(StatickString.onEmty);
        if ((health<35) && rotateToTarget) {
            anim.SetBool("block", true);
            isShield = true;
            if (trigger_countShield == false) {
                dataRecolected.EscudarseEx += 1;
                trigger_countShield = true;
            }
        }
        else {
            //anim.SetBool("block", false);
            trigger_countShield = false;
        }
        if (rotateToTarget) {
            LookTowardsTarget();
            random = Random.Range(0,1);
            //if(random<0.3f)

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

    public void MoveAnimation() {
        float square = agent.desiredVelocity.sqrMagnitude;
        float v = Mathf.Clamp(square, 0,.5f); 
        anim.SetFloat("vertical", v, 0.2f, delta);

        /*Vector3 desire = agent.desiredVelocity;
        Vector3 relative = transform.InverseTransformDirection(desire);
        float v = relative.z;
        float h = relative.x;
        v = Mathf.Clamp(v,-0.5f,.5f);
        h = Mathf.Clamp(h,-0.5f,.5f);

        anim.SetFloat("horizontal", h, 0.2f, delta);
        anim.SetFloat("vertical", v, 0.2f, delta);*/

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
                dataRecolected.EsquivarFa += 1;
                health -= v;
            }
            else
                dataRecolected.EsquivarEx += 1;
        }

        anim.applyRootMotion = true;
        anim.SetBool("canMove",false);

        sliderHealt.value = (int)health;
        isInvicible = true;
    }
}
