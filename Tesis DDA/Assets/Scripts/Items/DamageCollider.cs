using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour{

    public Animator anim;

    private void OnTriggerEnter(Collider other) {
        EnemyStates eStates=other.transform.GetComponentInParent<EnemyStates>();
        
        if (eStates == null)
            return;
        
        float r = Random.Range(0f,1f);
        if(anim.GetBool("two_handed")==true)
            eStates.DoDamage(35, r);
        else
            eStates.DoDamage(10,r);
    }
}
