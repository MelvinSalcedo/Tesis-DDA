using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGenerate : MonoBehaviour
{
    public int t = 0;
    public int t2 = 0;
    public EnemyStates es;
    public bool enter=false;

    private DataRecolected dataRecolected;

    private void Start() {
        dataRecolected = GameObject.Find("DataRecolected").GetComponent<DataRecolected>();
    }

    private void OnTriggerEnter(Collider other) {
        StateManager eStates = other.transform.GetComponent<StateManager>();

        if (eStates == null)
            return;

        if (es.canMove == false && enter==false) {
            eStates.DoDamage(10);
            enter = true;
            t2 = -1;
        }
        StartCoroutine(IE_VerifybollEnter());

        

    }

    IEnumerator IE_VerifybollEnter() {
        while (es.canMove==false) {
            enter = true;
            yield return null;
        }
        enter = false;
        
    }

    public void callCourutina() {
        StartCoroutine(IE_VerifyAttack());
    }

    IEnumerator IE_VerifyAttack() {
        yield return new WaitForSeconds(0.7f);
        if (t2 == -1) {
            if (t == 0)
                dataRecolected.Ataque_debilEx += 1;
            if (t == 1)
                dataRecolected.Ataque_FuerteEx += 1;
            if (t == 2)
                dataRecolected.Ataque_debilEx += 1;
            t2 = 0;
        }
        else {
            if (t == 0)
                dataRecolected.Ataque_debilFa += 1;
            if (t == 1)
                dataRecolected.Ataque_FuerteFa += 1;
            if (t == 2)
                dataRecolected.Ataque_debilFa += 1;
            
        }
        
    }
}
