using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimtorHook : MonoBehaviour {

    Animator anim;
    StateManager states;
    EnemyStates eStates;
    Rigidbody rigid;
    

    public float rm_multi;
    bool rolling;
    float roll_t;
    float delta;
    AnimationCurve rool_curve;

    //public AnimationCurve rollCurve;


    public void Init(StateManager st, EnemyStates eSt) {
        states = st;
        eStates = eSt;
        if (st != null) {
            anim = st.anim;
            rigid = st.rigid;
            rool_curve = states.roll_curve;
            delta = st.delta;
        }
        if (eSt != null) {
            anim = eSt.anim;
            rigid = eSt.rigid;
            delta = eSt.delta;
        }

      //  rollCurve = st.roll_curve;
    }

    public void InitForRoll() {
        rolling = true;
        roll_t = 0;
    }

    public void CloseRoll() {
        if (rolling == false) 
            return;
        rm_multi = 1;
        roll_t=0;
        rolling = false;
    }

    private void OnAnimatorMove() {
        if (states == null && states==null)
            return;
        if (rigid == null)
            return;

        if (states != null) {
            if (states.canMove)
                return;
            delta = states.delta;
        }
        if (eStates != null) {
            if (eStates.canMove)
                return;
            delta = eStates.delta;
        }

        if (states.canMove)
            return;

         

        rigid.drag = 0;

        if (rm_multi == 0) {
            rm_multi = 1;
        }

        if (rolling == false) {
            Vector3 delta2 = anim.deltaPosition;
            delta2.y = 0;
            Vector3 v = (delta2 * rm_multi) / delta;

            if (eStates) {
                eStates.agent.velocity = v;
            }
            else {
                rigid.velocity = v;
            }

            
        }
        else {
            roll_t += delta/0.6f;
            if (roll_t > 1) {
                roll_t = 1;
            }
            if (states == null)
                return;

            float zValue = rool_curve.Evaluate(roll_t);
            Vector3 v1 = Vector3.forward * zValue;
            Vector3 relative = transform.TransformDirection(v1);
            Vector3 v2 = (relative * rm_multi);
            rigid.velocity = v2;
        }
    }

    public void OpenDamageColliders() {
        if (states == null)
            return;
        states.invetoryManager.curWeapon.w_hook.openDamageColliders();
    }

    public void CloseDamageColliders() {
        if (states == null)
            return;
        states.invetoryManager.curWeapon.w_hook.CloseDamageColliders();
    }

}
