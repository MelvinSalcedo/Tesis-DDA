using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    float vertical;
    float horizontal;
    bool runInput;

    bool rb_input;
    float rt_axis;
    bool rt_input;
    bool lb_input;
    float lt_axis;
    bool lt_input;
    bool lt_inp;
    bool b_input;
    bool a_input;
    bool y_input;
    bool x_input;

    bool leftAxis_down;
    bool righttAxis_down;

    float b_timer;
    float rt_timer;
    float lt_timer;

    StateManager states;
    CamaraManager camManager;
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
        GetInput();
        UpdateStates();
        states.FixedTick(delta);
        camManager.Tick(delta);

        ResetInputNStates();
    }

    void Update() {

        delta = Time.deltaTime;
        //GetInput();
        states.Tick(delta);
        ResetInputNStates();
    }

    void GetInput() {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        b_input = Input.GetButton("B");
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


        rt_input = Input.GetButton("RT");
        rt_axis = Input.GetAxis("RT");

        if (rt_axis != 0) {
            rt_input = true;
        }

        lt_input = Input.GetButton("LT");
        lt_axis = Input.GetAxis("LT");

        if (lt_axis != 0) {
            lt_input = true;
        }

        rb_input = Input.GetButton("RB");
        lb_input = Input.GetButton("LB");

        righttAxis_down = Input.GetButtonUp("L");

        if (b_input)
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
            b_input = false;

        if (b_input && b_timer > 0.5f) {
            states.run = (states.moveAmount > 0);
        }
        if (b_input == false && b_timer > 0 && b_timer < 0.5f) {
            states.roolInput = true;
        }

        states.itemInput = x_input;
        states.rt = rt_input;
        states.lt = lt_input;
        states.rb = rb_input;
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
        if (righttAxis_down) {
            states.lockOn = !states.lockOn;

            if (states.lockOnTarget == null) {
                states.lockOn = false;
            }
            

            camManager.lockonTarget = states.lockOnTarget;
            states.lockOnTransform = camManager.lockonTransform;
            camManager.lockon = states.lockOn;
            //if(states.lockOnTransform)
        }
    }

    void ResetInputNStates() {
        if (b_input == false)
            b_timer = 0;
        if (states.roolInput)
            states.roolInput = false;
        if (states.run)
            states.run = false;
    }

}
