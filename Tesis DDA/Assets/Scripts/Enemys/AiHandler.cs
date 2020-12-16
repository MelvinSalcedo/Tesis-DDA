using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHandler : MonoBehaviour {
    public AgentManager agManager;

    public Transform positionInitial;

    public AIAttacks[] ai_attacks;
    AIAttacks currentAtack;
    public EnemyStates states;
    public DamageGenerate damageGenerate;
    public StateManager en_states;
    public Transform target;

    public GameObject[] defaultDamageColliders;

    public float sight;
    public float fov_angle;

    public int closeCount = 10;
    int _close;

    public int frameCount = 30;
    int _frame;

    public int attackCount = 30;
    int _attack;

    float dis;
    float angle;
    float delta;

    bool countExits = false;

    Vector3 dirToTarget;
    private DataRecolected dataRecolected;


    float distanceFromTarget() {
        if (target == null)
            return 100;
        return Vector3.Distance(target.position, transform.position);

    }

    float angleToTarget() {

        float a = 180;
        if (target) {
            Vector3 d = dirToTarget;
            a = Vector3.Angle(d, transform.position);
        }
        return a;
    }

    //public Queue<float> myQueue = new Queue<float>();
    public List<Dictionary<string, float>> estadosANN;

    public string SolaAccion;

    private void Start() {
        if (states == null)
            states = GetComponent<EnemyStates>();
        dataRecolected = GameObject.Find("DataRecolected").GetComponent<DataRecolected>();
        states.Init();
    }
    public AIstate aiState;

    public enum AIstate {
        far, close, inSight, attacking, restShield
    }

    bool managerQueue=false;

    public Queue<string> colaEjecuciones=new Queue<string>();


    bool triggerEscudar = false;
    bool triggerAtacar = false;


    private void Update() {
        delta = Time.deltaTime;
        dis = distanceFromTarget();
        angle = angleToTarget();

        if (target)
            dirToTarget = target.position - transform.position;
        states.dirTotarget = dirToTarget;

        ejecutarSecuencuaDeComandos();

        switch (SolaAccion) {
            case "acercarse"://carga: acércate al doble de velocidad
                acercarse();
                break;
            case "ataque fuerte"://retirarse en la direccion opuesta
                ataqueFuerte();
                break;
            case "ataque debil"://carga: acércate al doble de velocidad
                ataqueFuerte();
                break;
            case "retirarse"://retirarse en la direccion opuesta
                retirarse();
                break;
            case "escudar"://retirarse en la direccion opuesta
                Escudar();
                break;
            case "esquivar"://carga: acércate al doble de velocidad
                esquivar();
                break;
            case "observar"://carga: acércate al doble de velocidad
                Desencudar();
                break;
            case "ignorar"://retirarse en la direccion opuesta
                Desencudar();
                break;
            default:
                break;

        }
        
        states.Tick(delta);
        if (triggerEscudar == true) {
            _RaycastToTarget();
        }
        //ejecucionesME();


    }

    void ejecucionesME() {

        /*switch (aiState) {
            case AIstate.far:
                HandleFarSight();
                break;
            case AIstate.close:
                HandleCloseSight();
                break;
            case AIstate.inSight:
                InSight();
                break;
            case AIstate.restShield:
                VrestShield();
                break;
            case AIstate.attacking:
                if (states.canMove) {
                    aiState = AIstate.inSight;
                    states.rotateToTarget = true;
                    //states.agent.enabled = true;
                }
                break;
            default:
                break;
        }

        states.Tick(delta);*/
    }

    void HandleFarSight() {
        if (target == null)
            return;
        _frame++;
        if (_frame > frameCount) {
            _frame = 0;
            if (dis < sight) {
                if (angle < fov_angle) {
                    aiState = AIstate.close;
                }
            }
        }

    }

    void HandleCloseSight() {
        _close++;
        if (_close > closeCount) {
            _close = 0;

            if (dis > sight || angle > fov_angle) {

                aiState = AIstate.far;
                return;
            }

        }
        RaycastToTarget();
    }

    public void OpenDamageColliders() {
        if (currentAtack == null)
            return;
        if (currentAtack.isDefaultDamageColliders) {
            ObjetListStatus(defaultDamageColliders, true);
        }
        else {
         //   ObjetListStatus(currentAtack.dam, true);
        }

    }

    void ObjetListStatus(GameObject[] l,bool status) {
        for (int i = 0; i < l.Length; i++) {
            l[i].SetActive(status);
        }
    }

    void GoToTarget() {
        states.hasDestination = false;
        states.SetDestinatio(target.position);
    }

    void InSight() {
        
        HandleCoolDowns();

        float d2 = Vector3.Distance(states.targetDestination, target.position);

        if (d2 > 2 || dis > sight * 0.5f) {
            GoToTarget();
            if (countExits == false) {
                dataRecolected.AcercarseLentamenteEx += 1;
                countExits=true;
            }
        }
        if (dis < 2) {
            states.agent.isStopped = true;
            countExits = false;
        }
        if (_attack > 0) {
            _attack--;
            return;
        }
         
        _attack = attackCount;

        currentAtack = WillAtack();

        if (currentAtack != null) {

            if (currentAtack.targetAnim == "gs_oh_attack_1")
                damageGenerate.t = 0;
            if (currentAtack.targetAnim == "gs_oh_attack_2")
                damageGenerate.t = 1;
            if (currentAtack.targetAnim == "oh_attack_1")
                damageGenerate.t = 2;
            damageGenerate.callCourutina();

            aiState = AIstate.attacking;
            states.anim.Play(currentAtack.targetAnim);
            states.anim.SetBool("canMove",false);
            //states.anim.SetBool(StaticsString.onEmpty,false);

            states.canMove = false;
            currentAtack._cool = currentAtack.coolDown;
            states.agent.isStopped = true;
            states.rotateToTarget = false;
            //states.agent.enabled = false;
            return;
        }

        

    }

    void HandleCoolDowns() {
        for (int i = 0; i < ai_attacks.Length; i++) {
            AIAttacks a = ai_attacks[i];
            if (a._cool > 0) {
                a._cool -= delta;
                if (a._cool < 0)
                    a._cool = 0;
                continue;
            }
        }

    }

    public AIAttacks WillAtack() {

        if (en_states.health % 40 == 0) {
            aiState = AIstate.restShield;
            //return null;
        }


        int w = 0;
        List<AIAttacks> l = new List<AIAttacks>();

        for (int i = 0; i < ai_attacks.Length; i++) {
            AIAttacks a = ai_attacks[i];
            if (a._cool > 0) {
                continue;
            }

            if (dis > a.minDIstance)
                continue;
            if (angle < a.minAngle)
                continue;
            if (angle > a.maxAngle)
                continue;
            if (a.weight == 0)
                continue;
            w += a.weight;
            l.Add(a);

        }

        if (l.Count == 0)
            return null;

        int ran = Random.Range(0, w + 1);
        int c_W = 0;
        for (int i = 0; i < l.Count; i++) {
            c_W += l[i].weight;
            if (c_W > ran) {

                return l[i];
            }
        }
        return null;

    }

    void RaycastToTarget() { }

    void _RaycastToTarget() {

        RaycastHit hit;
        Vector3 origin = transform.position;
        origin.y += 0.5f;
        Vector3 dir = dirToTarget;
        dir.y += 0.5f;

        if (Physics.Raycast(origin, dir, out hit, sight, states.ignoreLayers)) {
            StateManager st = hit.transform.GetComponentInParent<StateManager>();
            if (st != null) {
                states.rotateToTarget = true;
                aiState = AIstate.inSight;
                //states.SetDestinatio(target.position);
            }
        }
    }

    /********************************/

    void Escudar() {

        states.anim.SetBool("block", true);
        /*if (r >= 0.3f) {
            aiState = AIstate.inSight;
            //states.anim.SetBool("block", false);
        }
        else {
            //states.anim.SetBool("block", false);
            aiState = AIstate.attacking;
        }*/
        triggerEscudar = true;
        managerQueue = false;
    }

    void Desencudar() {
        states.anim.SetBool("block", false);
        triggerEscudar = false;
        managerQueue = false;
        
    }

    void acercarse() {
        RaycastToTarget();
        states.hasDestination = false;
        states.SetDestinatio(target.position);

        float d2 = Vector3.Distance(en_states.thisTransform.position, this.transform.position);
        
        if (d2 < 2f) {
            //Debug.Log("Next--------> " + d2);
            managerQueue = false;
        }
    }

    void ataqueFuerte() {
        if (triggerAtacar == false) {
            triggerAtacar = true;
            HandleCoolDowns();
            RaycastToTarget();

            //float d2 = Vector3.Distance(states.targetDestination, target.position);

            if (dis < 2) {
                states.agent.isStopped = true;
            }
            else {
                triggerAtacar = false;
                managerQueue = false;
                return;
            }

            string tipeAtaque = "";
            int a = Random.Range(0, 3);
            if (a == 0)
                tipeAtaque = "gs_oh_attack_1";
            else if (a == 1)
                tipeAtaque = "gs_oh_attack_2";
            else
                tipeAtaque = "gs_oh_attack_1";

            StartCoroutine(IE_VerifyAttack(tipeAtaque));
            /*currentAtack = WillAtack();

            if (currentAtack != null) {

                if (currentAtack.targetAnim == "gs_oh_attack_1")
                    damageGenerate.t = 0;
                if (currentAtack.targetAnim == "gs_oh_attack_2")
                    damageGenerate.t = 1;
                if (currentAtack.targetAnim == "oh_attack_1")
                    damageGenerate.t = 2;
                damageGenerate.callCourutina();
                StartCoroutine(IE_VerifyAttack());


                return;
            }*/
        }
    }

    IEnumerator IE_VerifyAttack(string tipeAtaque) {
        
        Debug.Log("=========>");
        //aiState = AIstate.attacking;
        states.anim.Play(tipeAtaque);
        states.anim.SetBool("canMove", false);
        //states.anim.SetBool(StaticsString.onEmpty,false);

        //states.canMove = false;
        yield return new WaitForSeconds(2f);
        states.agent.isStopped = true;
        //states.rotateToTarget = false;
        //states.agent.enabled = false;
        
        managerQueue = false;
        triggerAtacar = false;
    }

    void retirarse() {
        states.hasDestination = false;

        float d2 = Vector3.Distance(positionInitial.position, this.transform.position);
        if (d2 > 2)
            states.SetDestinatio(positionInitial.position);
        else {
            managerQueue = false;
        }
       
    }

    void esquivar() {
        float d2 = Vector3.Distance(en_states.thisTransform.position, this.transform.position);
        //Debug.Log("Next--------> " + d2);
        if (d2 < 4) {
            Debug.Log("Next--------> " + d2);
            SolaAccion = "";
            StartCoroutine(IE_esquivar());
        }
        else {
            managerQueue = false;
        }
        
    }

    IEnumerator IE_esquivar() {
        states.anim.Play("Step_back");
        yield return new WaitForSeconds(1.5f);
        managerQueue = false;
        yield return null;
    }

    void ejecutarSecuencuaDeComandos() {
        
        if (colaEjecuciones.Count > 0 ) {
            if (managerQueue == false) {

                SolaAccion = colaEjecuciones.Dequeue();
                Debug.Log("accion actual = " + SolaAccion);

                managerQueue = true;
            }
        }
        if (colaEjecuciones.Count == 0) {
            agManager.ejecutarAnnAgain = false;
            managerQueue = false;
        }
        /*if (accionActual == "escudar") {
            managerQueue = false;
        }*/
    }
}

[System.Serializable]
public class AIAttacks {
    public int weight;
    public float minDIstance;
    public float minAngle;
    public float maxAngle;

    public float coolDown=2;
    public float _cool;
    public string targetAnim;

    public bool isDefaultDamageColliders;
        
}