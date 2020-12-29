using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour{

    public Animator anim;
    public string tipoAtaque;

    BoxCollider bc;
    private void Start() {
        bc = GetComponent<BoxCollider>();
    }

    private bool oneshot = false;
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemigo") {
            EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

            if (eStates == null)
                return;


            float r = Random.Range(0f, 1f);
            if (tipoAtaque == "ad")
                eStates.DoDamage(10, r);
            else
                eStates.DoDamage(20, r);

        }
    }


}
