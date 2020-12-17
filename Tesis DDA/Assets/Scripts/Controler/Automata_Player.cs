using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Automata_Player : MonoBehaviour
{
    private NavMeshAgent nav;
    private float time = 0;

    public Transform NPC;
    public InputHandler inputHandler;

    public enum estados{acercarse,ataque,esquivar,retroceder,escudar}

    public estados maquinaEstados;

    void Start(){
        nav = GetComponent<NavMeshAgent>();
        maquinaEstados = estados.acercarse;
    }




    private bool triggerAcercarse = false;
    private bool triggerAtacar = false;
    private bool triggerEscudar = false;
    private bool triggerRetroceder = false;
    private bool triggerEsquivar = false;
    private bool trigger = false;

    void FixedUpdate() {

        switch (maquinaEstados) {
            case estados.acercarse:
                if (!triggerAcercarse)
                    StartCoroutine(Acercarse());
                break;
            case estados.ataque:
                if (!triggerAtacar)
                    StartCoroutine(Ataque());
                break;
            case estados.escudar:
                if (!triggerEscudar)
                    StartCoroutine(Escudar());
                break;
            case estados.retroceder:
                if (!triggerRetroceder)
                    StartCoroutine(Retroceder());
                break;
            case estados.esquivar:
                if (!triggerEsquivar)
                    StartCoroutine(Esquivar());
                break;
            default:
                break;
        }
    }

    IEnumerator Acercarse() {
        triggerAcercarse = true;

        while (Vector3.Distance(NPC.position, this.transform.position)> 2) {
            inputHandler.Void_Mover(0.75f, 0);
            yield return null;
        }
        inputHandler.Void_Mover(0, 0);
        yield return new WaitForSeconds(0.5f);

        float r = Random.Range(0f, 1f);
        if (r > 0.5f)
            maquinaEstados = estados.ataque;
        else
            maquinaEstados = estados.escudar;

        triggerAcercarse = false;
    }

    IEnumerator Ataque() {
        triggerAtacar = true;

        float r = Random.Range(0f, 1f);
        if (r < 0.5)
            inputHandler.Void_AtaqueDebil();
        else
            inputHandler.Void_AtaqueFuerte();

        yield return new WaitForSeconds(1);

        r = Random.Range(0f, 1f);
        if (r < 0.5f) 
            maquinaEstados = estados.escudar;
        else 
            maquinaEstados = estados.ataque;

        triggerAtacar = false;
    }

    IEnumerator Escudar() {
        triggerEscudar=true;
        inputHandler.Void_Escudar();
        yield return new WaitForSeconds(1);

        float r = Random.Range(0f, 1f);
        if (r < 0.5f)
            maquinaEstados = estados.retroceder;
        else
            maquinaEstados = estados.esquivar;

        triggerEscudar = false;
    }

    IEnumerator Retroceder() {
        triggerRetroceder = true;
       
        while (Vector3.Distance(NPC.position, this.transform.position) < 3) {
            inputHandler.Void_Mover(-0.85f, 0);
            yield return null;
        }    
        yield return new WaitForSeconds(1);
        
        maquinaEstados = estados.acercarse;

        triggerRetroceder = false;
    }

    IEnumerator Esquivar() {
        triggerEsquivar = true;

        
        int t = 0;
        while (t < 60) {
            t += 1;
            inputHandler.Void_Esquivar();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);

        float r = Random.Range(0f, 1f);
        if (r < 0.5f)
            maquinaEstados = estados.acercarse;
        else
            maquinaEstados = estados.ataque;

        triggerEsquivar = false;
    }


}
