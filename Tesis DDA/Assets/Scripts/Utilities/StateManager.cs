using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {
    public Transform thisTransform;
    [Header("Caracteristicas")]

    public float Distance;
    public float numEnemy = 1;
    public int FuiHerodoRecientemente = 0;
    public int HeriAlEnemigoRecientemente = 0;
    public int health = 100;
    public int healthEnemigo = 100;
    public int RecientementeBloqueeUnAtaque = 0;
    public int RecientementeElEnemigoBloqueeUnAtaque = 0;

    /*
     * damageReceived_Player
     * lifePlayer
     * hitsBlocked_Player
     * hitsLossed_Player
     * damageDeal_Player
     * damageDeal_NPC;
     * time
     */
    public DataRecolected dataRecolected;
    public float damageRecived_player=0;
    public float numHistBloquedPlayer=0;
    public float numHistLostPlayer=0;
    public float damageDealPlayer=0;
    public float damageDeal_NPC = 0;
    public float timeGame = 0;

    //public float numEnemy=1;

    [Header("Life")]

    public Slider sliderLife;
    public GameObject yoDie;
    public EnemyStates es;

    [Header("Init")]
    public GameObject activeModel;

    [Header("Inputs")]
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public Vector3 moveDir;
    public bool rt, rb, lt, lb;
    public bool roolInput;
    public bool itemInput;


    [Header("Stats")]
    public float moveSpeed = 2;
    public float runSpeed = 3.5f;
    public float rotateSpeed = 6f;
    public float toGround = 0.5f;
    public float rollSpeed = 1;

    [Header("States")]
    public bool run;
    public bool onGround;
    public bool lockOn;
    public bool inAction;
    public bool canMove;
    public bool isTwoHanded;
    public bool usingItem;



    [Header("other")]
    public EnemyTarget lockOnTarget;
    public Transform lockOnTransform;
    public AnimationCurve roll_curve;


    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rigid;

    [HideInInspector]
    public AnimtorHook a_hook;

    [HideInInspector]
    public ActionManager actionManager;

    [HideInInspector]
    public InventoryManager invetoryManager;

    [HideInInspector]
    public float delta;
    [HideInInspector]
    public LayerMask ignoreLayer;


    public bool isInvicible = false;
    public bool pass;

    float _actionDelay;

    public void Init() {
        SetupAnimator();
        rigid = GetComponent<Rigidbody>();
        rigid.angularDrag = 999;
        rigid.drag = 4;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        invetoryManager = GetComponent<InventoryManager>();
        invetoryManager.Init();

        actionManager = GetComponent<ActionManager>();
        actionManager.Init(this);

        a_hook = activeModel.GetComponent<AnimtorHook>();
        if (a_hook == null)
            a_hook = activeModel.AddComponent<AnimtorHook>();
        a_hook.Init(this, null);

        a_hook = activeModel.AddComponent<AnimtorHook>();
        a_hook.Init(this, null);
        gameObject.layer = 8;
        ignoreLayer = ~(1 << 9);

        anim.SetBool("onGround", true);
    }

    void SetupAnimator() {
        if (activeModel == null) {
            anim = GetComponentInChildren<Animator>();
            if (anim == null) {
                Debug.Log("not model found");
            }
            else {
                activeModel = anim.gameObject;
            }
        }
        if (anim == null)
            anim = activeModel.GetComponent<Animator>();
        anim.applyRootMotion = false;
    }

    public void FixedTick(float d) {
        delta = d;

        usingItem = anim.GetBool("interacting");




        /*if (usingItem == false) {
            if(!invetoryManager.curWeapon.weaponModel.activeInHierarchy)
             
        }*/

        DetecItemAction();
        DetecActios();
        invetoryManager.curWeapon.weaponModel.SetActive(!usingItem);
        if (inAction) {//verifica si alguna mecanica se esta ejecutando
            anim.applyRootMotion = true;
            _actionDelay += delta;
            if (_actionDelay > 0.3f) {
                inAction = false;
                _actionDelay = 0;
            }
            else {
                return;
            }

        }

        //states.anim.SetBool(StaticsString.onEmpty);
        canMove = anim.GetBool("canMove");
        if (!canMove)
            return;

        //a_hook.rm_multi = 1;
        a_hook.CloseRoll();
        HandleRolls();

        anim.applyRootMotion = false;

        rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

        float targeSpeed = moveSpeed;

        if (usingItem) {
            run = false;
            moveAmount = Mathf.Clamp(moveAmount, 0, 0.45f);
        }

        if (run) {//Verifica si la teclla correr se esta ejecuntado
            
            targeSpeed = runSpeed;
        }

        if (onGround)
            rigid.velocity = moveDir * (targeSpeed * moveAmount);

        if (run)
            lockOn = false;

        Vector3 targetDir = (lockOn == false) ?
            moveDir
            :
            (lockOnTransform != null) ?
                lockOnTransform.transform.position - transform.position
                :
                moveDir;

        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
        transform.rotation = targetRotation;

        anim.SetBool("lockOn", lockOn);

        if (lockOn == false)
            HandleMovementAnimations();
        else
            HandelLockOnAnimatios(moveDir);
    }

    public void DetecItemAction() {
        if (canMove == false || usingItem)
            return;
        if (itemInput == false)
            return;

        ItemAction slot = actionManager.consumableItem;
        string targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;
        //invetoryManager.curWeapon.weaponModel.SetActive(false);
        usingItem = true;
        anim.Play(targetAnim);
    }

    public void DetecActios() {
        if (canMove == false || usingItem)
            return;
        if (rb == false && rt == false && lt == false && lb == false) {
            return;
        }
        string targetAnim = null;

        Action slot = actionManager.GetActionSlot(this);
        if (slot == null)
            return;
        targetAnim = slot.targetAnim;


        if (string.IsNullOrEmpty(targetAnim))
            return;
        canMove = false;
        inAction = true;
        anim.CrossFade(targetAnim, 0.2f);
        //rigid.velocity = Vector3.zero;

    }

    public void Tick(float d) {
        numHistLostPlayer = 0;//Pendiente
        numHistBloquedPlayer = dataRecolected.Ataque_debilFa + dataRecolected.Ataque_FuerteFa;
        damageRecived_player = 100-health;
        damageDealPlayer = 100-es.health;
        damageDeal_NPC = 100-health;

        timeGame += Time.deltaTime;
        Distance = Vector3.Distance(transform.position,es.transform.position);

        delta = d;
        onGround = OnGround();
        anim.SetBool("onGround", onGround);

        /*if (Input.GetKeyDown(KeyCode.F)) {
            anim.SetBool("block", true);
        }*/

    }

    void HandleRolls() {
        if (!roolInput || usingItem)
            return;
        float v = vertical;
        float h = horizontal;

        v = (moveAmount > 0.3f) ? 1 : 0;
        h = 0;


        /*if (lockOn == false) {
            v = (moveAmount>0.3f)?1:0;
            h = 0;
        }
        else {
            if (Mathf.Abs(v)< 0.3f) 
                v = 0;
            if (Mathf.Abs(h)< 0.3f) 
                h = 0;

        }*/

        if (v != 0) {
            if (moveDir == Vector3.zero)
                moveDir = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = targetRot;
            a_hook.InitForRoll();
            a_hook.rm_multi = rollSpeed;
        }
        else {
            a_hook.rm_multi = 1;
        }


        anim.SetFloat("vertical", v);
        anim.SetFloat("horizontal", h);

        canMove = false;
        inAction = true;
        anim.CrossFade("Rolls", 0.2f);

    }

    void HandleMovementAnimations() {
        anim.SetBool("run", run);
        anim.SetFloat("vertical", moveAmount, 0.4f, delta);
    }

    void HandelLockOnAnimatios(Vector3 moveDir) {
        Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
        float h = relativeDir.x;
        float v = relativeDir.z;

        anim.SetFloat("vertical", v, 0.2f, delta);
        anim.SetFloat("horizontal", h, 0.2f, delta);
    }

    public bool OnGround() {
        bool r = false;
        Vector3 origin = transform.position + (Vector3.up * toGround);
        Vector3 dir = -Vector3.up;
        float dis = toGround + 0.2f;
        RaycastHit hit;
        Debug.DrawRay(origin, dir * dis);
        if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayer)) {
            r = true;
            Vector3 targetPosition = hit.point;
            transform.position = targetPosition;

        }
        return r;
    }

    public void HandleTwoHanded() {
        anim.SetBool("two_handed", isTwoHanded);
        /*delete this line code */
        if (isTwoHanded == true)
            GameObject.Find("prop_shield_Hero").GetComponent<MeshRenderer>().enabled = false;
        else
            GameObject.Find("prop_shield_Hero").GetComponent<MeshRenderer>().enabled = true;
        if (isTwoHanded)
            actionManager.UpdateActionsTwoHanded();
        else
            actionManager.UpdateActionsOneHanded();
    }

    public void DoDamage(int v) {

        /*if (isInvicible==false)
          *  return;
        */
        health -= v;
        anim.Play("damage_1");

        anim.applyRootMotion = true;
        anim.SetBool("canMove", false);

        sliderLife.value = (int)health;

        if (health <= 0)
            yoDie.SetActive(true);
        //isInvicible = true;
        pass = false;
    }

    public void ResetAllGame() {

    }
}
