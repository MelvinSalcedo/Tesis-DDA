using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHandler : MonoBehaviour {

    public AgentManager agManager;
    public EnemyStates states;
    public StateManager en_states;

    public DamageGenerate damageGnerate;

    public Transform positionInitial;
    public Transform target;


    public float sight;
    public string SolaAccion;

    private float dis;
    private float angle;
    private float delta;
    private float delta2;

    private bool triggerEscudar = false;
    private bool triggerAtacar = false;
    private bool managerQueue = false;

    private bool escudar = false;
    private bool retirarme = false;
    private bool acercarme = false;

    private Vector3 dirToTarget;

    public List<Dictionary<string, float>> estadosANN;
    public Queue<string> colaEjecuciones = new Queue<string>();

    //void Start
    private void Start() {
        if (states == null)
            states = GetComponent<EnemyStates>();
        states.Init();
    }

    //void Update
    private void Update() {
        delta = Time.deltaTime;
        dis = distanceFromTarget();
        angle = angleToTarget();

        if (target)
            dirToTarget = target.position - transform.position;
        states.dirTotarget = dirToTarget;

        ejecutarSecuencuaDeComandos();
        states.updateStrinAccionRetroceso(SolaAccion);
        switch (SolaAccion) {
            case "acercarse":
                acercarse();
                break;
            case "ataque fuerte":
                ataqueFuerte();
                break;
            case "ataque debil":
                ataqueDevil();
                break;
            case "esquivar":
                esquivar();
                break;
            case "escudar":
                Escudar();
                break;
            case "desencudar":
                Desencudar();
                break;
            case "retirarse":
                retirarse();
                break;
            case "observar":
                observar();
                break;
            default:
                break;
        }
        states.Tick(delta);
        if (triggerEscudar == true) {
            _RaycastToTarget();
        }
    }

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
                //states.SetDestinatio(target.position);
            }
        }
    }

    void Escudar() {
        if (escudar == false ) {
            DataRecolected.instancia.EscudarseEx += 1;
            escudar = true;
        }
        states.anim.SetBool("block", true);
        triggerEscudar = true;
        managerQueue = false;
    }

    void Desencudar() {
        escudar = false;
        states.anim.SetBool("block", false);
        triggerEscudar = false;
        managerQueue = false;

    }

    bool observarTrigger = false;

    void observar() {
        delta2 = Time.deltaTime;
        RaycastToTarget();
        states.hasDestination = false;

        float d2 = Vector3.Distance(en_states.thisTransform.position, this.transform.position);

        if (d2 > 3f) {
            if (observarTrigger == false) {
                StartCoroutine(ieObservar());

                observarTrigger = true;
            }
            
        }
        else {
            managerQueue = false;
        }

    }
    IEnumerator ieObservar() {
        yield return new WaitForSeconds(5);
        DataRecolected.instancia.ObservarlEx += 1;
        managerQueue = false;
        observarTrigger = false;
    }

    void acercarse() {
        RaycastToTarget();
        states.hasDestination = false;
        states.SetDestinatio(target.position);

        float d2 = Vector3.Distance(en_states.thisTransform.position, this.transform.position);

        if (d2 < 2f) {
           // if (acercarme == false) {
                DataRecolected.instancia.AcercarseLentamenteEx += 1;
             //   acercarme = true;
            //}
            managerQueue = false;
            retirarme = false;
        }
    }

    void retirarse() {
        states.hasDestination = false;

        float d2 = Vector3.Distance(positionInitial.position, this.transform.position);
        if (d2 > 2)
            states.SetDestinatio(positionInitial.position);
        else {
            if (retirarme == false) {
                DataRecolected.instancia.RetirsaseEx += 1;
                retirarme = true;
            }
            managerQueue = false;
        }

    }

    void ataqueFuerte() {

        if (triggerAtacar == false) {
            triggerAtacar = true;

            RaycastToTarget();

            if (dis < 2) {
                states.agent.isStopped = true;
            }
            else {
                triggerAtacar = false;
                managerQueue = false;
                return;
            }

            damageGnerate.TipoDeAtaque("ataqueFuerte");
            string tipeAtaque = "";

            int a = Random.Range(0, 2);
            if (a == 0)
                tipeAtaque = "gs_oh_attack_1";
            else if (a == 1)
                tipeAtaque = "gs_oh_attack_2 1";
            

            StartCoroutine(IE_VerifyAttack(tipeAtaque));
        }
    }

    void ataqueDevil() {
        if (triggerAtacar == false) {
            triggerAtacar = true;

            RaycastToTarget();

            if (dis < 2) {
                states.agent.isStopped = true;
            }
            else {
                triggerAtacar = false;
                managerQueue = false;
                return;
            }
            damageGnerate.TipoDeAtaque("ataqueDevil");
            string tipeAtaque = "";
            tipeAtaque = "oh_attack_1";


            StartCoroutine(IE_VerifyAttack(tipeAtaque));
        }
    }
    IEnumerator IE_VerifyAttack(string tipeAtaque) {

        //Debug.Log("=========>");
        //aiState = AIstate.attacking;
        states.anim.Play(tipeAtaque);
        states.anim.SetBool("canMove", false);
        //states.anim.SetBool(StaticsString.onEmpty,false);

        //states.canMove = false;
        yield return new WaitForSeconds(1.2f);
        states.agent.isStopped = true;
        //states.rotateToTarget = false;
        //states.agent.enabled = false;

        managerQueue = false;
        triggerAtacar = false;
        if (tipeAtaque == "gs_oh_attack_1" || tipeAtaque == "gs_oh_attack_2 1") {
            if (damageGnerate.AcertoElAtaque == false) {
                DataRecolected.instancia.Ataque_debilFa += 1;
            }
        }
        else {
            if (damageGnerate.AcertoElAtaque == false) {
                DataRecolected.instancia.Ataque_FuerteFa += 1;
            }
        }
        damageGnerate.AcertoElAtaque = false;
    }

    void esquivar() {
        float d2 = Vector3.Distance(en_states.thisTransform.position, this.transform.position);
        if (d2 < 4) {
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

        if (colaEjecuciones.Count > 0) {
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