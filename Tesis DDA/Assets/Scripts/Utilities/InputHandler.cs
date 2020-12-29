using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public Automata_Player ap;

    float vertical;
    float horizontal;
    bool runInput;

    bool ataqueDebil;
    float rt_axis;
    bool ataqueFuerte;
    bool lb_input;
    float lt_axis;
    bool lt_input;
    bool lt_inp;
    bool Esquivar;
    bool a_input;
    bool y_input;
    bool x_input;

    bool leftAxis_down;
    bool Enfocar;

    float b_timer;
    float rt_timer;
    float lt_timer;

    StateManager states;
    CamaraManager camManager;

    private bool lookOnCmaera=false;
    private bool esquivarOne=false;
    float delta;
    // Start is called before the first frame update
    void Start() {
        states = GetComponent<StateManager>();
        states.Init();

        camManager = CamaraManager.singleton;
        camManager.Init(states);
    }

    private void FixedUpdate() {
        delta = Time.fixedDeltaTime;
       if (ap.enabled == false)
            GetInput();
        //Void_Esquivar();
        UpdateStates();
        states.FixedTick(delta);
        camManager.Tick(delta);

        ResetInputNStates();
    }

    void Update() {
        
        delta = Time.deltaTime;
        states.Tick(delta);
        ResetInputNStates();
    }

    void GetInput() {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        Esquivar = Input.GetButton("Esquivar");
        a_input = Input.GetButton("A");
        y_input = Input.GetButtonDown("Y");
        /*if (Input.GetKeyDown("e")) {
            Debug.Log("y");
            y_input = true;
        }
        else {
            y_input = false;
        }*/
        x_input = Input.GetButton("X");


        ataqueFuerte = Input.GetButton("ataqueFuerte");
        rt_axis = Input.GetAxis("RT");

        if (rt_axis != 0) {
            ataqueFuerte = true;
        }

        lt_input = Input.GetButton("LT");
        lt_axis = Input.GetAxis("LT");

        if (lt_axis != 0) {
            lt_input = true;
        }

        ataqueDebil = Input.GetButton("ataqueDebil");
        lb_input = Input.GetButton("LB");

        Enfocar = Input.GetButtonUp("Enfocar");

        if (Esquivar)
            b_timer += delta;
    }

    void UpdateStates() {

        states.horizontal = horizontal;
        states.horizontal = vertical;

        Vector3 v = vertical * camManager.transform.forward;
        Vector3 h = horizontal * camManager.transform.right;

        states.moveDir = (v + h).normalized;

        float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        states.moveAmount = Mathf.Clamp01(m);

        //states.roolInput = b_input;
        if (x_input)
            Esquivar = false;

        if (Esquivar && b_timer > 0.5f) {
            states.run = (states.moveAmount > 0);
            
        }
        
        if (Esquivar == false && b_timer > 0 && b_timer < 0.5f) {
            ///Debug.Log("%%%%%%%%%%%%%%%%%%%%%%");
            esquivarOne = true;
            states.roolInput = true;
           
        }

        states.itemInput = x_input;
        states.rt = ataqueFuerte;
        states.lt = lt_input;
        states.rb = ataqueDebil;
        states.lb = lb_input;

        if (y_input) {
            states.isTwoHanded = !states.isTwoHanded;
            states.HandleTwoHanded();
        }
        if (states.lockOnTarget != null) {
            if (states.lockOnTarget.eStates.isDead) {
                states.lockOn = false;
                states.lockOnTarget = null;
                states.lockOnTransform = null;
                camManager.lockon = false;
                camManager.lockonTarget =null;

            }
        }
        if (Enfocar) {
            states.lockOn = !states.lockOn;

            if (states.lockOnTarget == null) {
                states.lockOn = false;
            }
            

            camManager.lockonTarget = states.lockOnTarget;
            states.lockOnTransform = camManager.lockonTransform;
            camManager.lockon = states.lockOn;
           
            lookOnCmaera = true;
            //if(states.lockOnTransform)
        }
    }

    void ResetInputNStates() {
        if (Esquivar == false)
            b_timer = 0;
        if (states.roolInput)
            states.roolInput = false;
        if (states.run)
            states.run = false;
    }

    public void Void_Enfocar() {

        if (lookOnCmaera == false)
            Enfocar = true;
        else {
            Enfocar = false;
        }
        
    }

    public void Void_Mover(float v,float h) {
        Void_Enfocar();
        vertical = v;
        horizontal =h;
    }

    public void Quieto(float v, float h) {
        Void_Enfocar();
        vertical = 0;
        horizontal = 0;
    }

    public void Void_AtaqueDebil() {
        ataqueDebil = true;
        StartCoroutine(time());
    }

    public void Void_AtaqueFuerte() {
        ataqueFuerte = true;
        StartCoroutine(time());
    }

    public void Void_Esquivar() {
        b_timer = 0.3f;
        Esquivar = false;
    }

    IEnumerator IE_esquivar() {
        
        

        /*int t = 0;
        while (true) {
            Esquivar = true;
            yield return new WaitForEndOfFrame();
            t += 1;
            if (esquivarOne == true)
                break;
            
        }*/
        
        //yield return new WaitForSeconds(0.05f);
        yield return null;
        b_timer = 0.3f;
        Esquivar = false;

    }

    public void Void_Escudar() {
        GameObject go = this.gameObject.transform.GetChild(1).gameObject;
        go.GetComponent<Animator>().SetBool("block", true);
    }


    IEnumerator time() {
        yield return new WaitForSeconds(0.4f);
        ataqueFuerte = false;
        ataqueDebil = false;
        Esquivar = false;
    }
}
