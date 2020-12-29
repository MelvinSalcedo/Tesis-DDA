using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGenerate : MonoBehaviour
{
    public int t = 0;
    public int t2 = 0;
    public EnemyStates es;
    public bool enter=false;

    string tipoAtaque = "";

    public bool AcertoElAtaque = false;
    private void OnTriggerEnter(Collider other) {
        StateManager eStates = other.transform.GetComponent<StateManager>();

        if (eStates == null)
            return;
           
            enter = true;
            t2 = -1;
            if (tipoAtaque=="ataqueDevil"){
                DataRecolected.instancia.Ataque_debilEx += 1;
                eStates.DoDamage(10);
            }
            else if(tipoAtaque == "ataqueFuerte") {
                DataRecolected.instancia.Ataque_FuerteEx += 1;
                eStates.DoDamage(20);
            }
            tipoAtaque = "";
            AcertoElAtaque = true;
        
        StartCoroutine(IE_VerifybollEnter());

    }

    IEnumerator IE_VerifybollEnter() {
        while (es.canMove==false) {
            enter = true;
            yield return null;
        }
        enter = false;
        
    }

    public void TipoDeAtaque(string s) {
        tipoAtaque = s;
    }

    IEnumerator IE_VerifyAttack() {
        yield return new WaitForSeconds(0.7f);
        if (t2 == -1) {
            if (t == 0)
                DataRecolected.instancia.Ataque_debilEx += 1;
            if (t == 1)
                DataRecolected.instancia.Ataque_FuerteEx += 1;
            if (t == 2)
                DataRecolected.instancia.Ataque_debilEx += 1;
            t2 = 0;
        }
        else {
            if (t == 0)
                DataRecolected.instancia.Ataque_debilFa += 1;
            if (t == 1)
                DataRecolected.instancia.Ataque_FuerteFa += 1;
            if (t == 2)
                DataRecolected.instancia.Ataque_debilFa += 1;
        }
        
    }
}
