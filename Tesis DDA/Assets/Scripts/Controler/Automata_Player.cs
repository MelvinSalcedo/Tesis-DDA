using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Automata_Player : MonoBehaviour {
    public bool modoAtaque = false;
    public bool modoDefensa = false;

    public DamageCollider dc;

    private NavMeshAgent nav;
    private float time = 0;
    private float time2 = 0;

    public Transform NPC;
    public InputHandler inputHandler;

    public enum estados { acercarse, ataque, esquivar, retroceder, escudar, quieto }

    public estados maquinaEstados;

    private bool triggerAcercarse = false;
    private bool triggerAtacar = false;
    private bool triggerEscudar = false;
    private bool triggerRetroceder = false;
    private bool triggerEsquivar = false;
    private bool triggerQuieto = false;
    private bool trigger = false;

    void Start() {
        nav = GetComponent<NavMeshAgent>();
        maquinaEstados = estados.acercarse;
    }

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
            case estados.quieto:
                if (!triggerQuieto) {
                    StartCoroutine(Quieto());
                    time2 += Time.deltaTime;
                }
                break;
            default:
                break;
        }
    }

    IEnumerator Acercarse() {
        StateManager.doesItMove = 0;
        triggerAcercarse = true;
        StateManager.isAtack = 0;
        while (Vector3.Distance(NPC.position, this.transform.position) > 2) {
            inputHandler.Void_Mover(0.75f, 0);
            yield return null;
        }
        inputHandler.Void_Mover(0, 0);
        yield return new WaitForSeconds(0.15f);

        float r = Random.Range(0f, 1f);

        if (modoAtaque == true) {
            if (r > 0.25f)
                maquinaEstados = estados.ataque;
            else
                maquinaEstados = estados.escudar;
        }

        if (modoDefensa == true) {
            if (r > 0.75f)
                maquinaEstados = estados.ataque;
            else
                maquinaEstados = estados.escudar;
        }
        else {
            if (r > 0.5f)
                maquinaEstados = estados.ataque;
            else
                maquinaEstados = estados.escudar;
        }

        triggerAcercarse = false;
    }

    IEnumerator Ataque() {
        triggerAtacar = true;
        StateManager.isAtack = 1;
        float r = Random.Range(0f, 1f);
        if (r < 0.5) {
            dc.tipoAtaque = "ad";
            inputHandler.Void_AtaqueDebil();
        }
        else {
            dc.tipoAtaque = "af";
            inputHandler.Void_AtaqueFuerte();
        }

        yield return new WaitForSeconds(0.51f);

        r = Random.Range(0f, 1f);
        if (r < 0.5f)
            maquinaEstados = estados.escudar;
        else
            maquinaEstados = estados.ataque;

        triggerAtacar = false;
    }

    IEnumerator Escudar() {
        StateManager.usingShield = 1;
        triggerEscudar = true;
        inputHandler.Void_Escudar();

        yield return new WaitForSeconds(1);

        float r = Random.Range(0f, 1f);
        if (modoAtaque == true) {
            if (r < 0.35)
                maquinaEstados = estados.ataque;
            else
                maquinaEstados = estados.esquivar;
        }
        else if (modoDefensa == true) {
            if (r < 0.55f)
                maquinaEstados = estados.retroceder;
            else
                maquinaEstados = estados.esquivar;
        }
        else {
            if (r < 0.5f)
                maquinaEstados = estados.retroceder;
            else
                maquinaEstados = estados.esquivar;
        }

        triggerEscudar = false;
    }

    IEnumerator Retroceder() {
        triggerRetroceder = true;
        StateManager.doesItMove = 0;
        StateManager.isAtack = 0;
        while (Vector3.Distance(NPC.position, this.transform.position) < 3) {
            inputHandler.Void_Mover(-0.85f, 0);
            yield return null;
        }
        yield return new WaitForSeconds(1);

        float r = Random.Range(0f, 1f);
        if (r < 0.7f)
            maquinaEstados = estados.acercarse;
        else {
            maquinaEstados = estados.quieto;
        }



        triggerRetroceder = false;
    }

    IEnumerator Esquivar() {
        triggerEsquivar = true;
        StateManager.isAtack = 0;
        int t = 0;
        while (t < 60) {
            t += 1;
            inputHandler.Void_Esquivar();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);

        float r = Random.Range(0f, 1f);
        if (modoAtaque == true) {
            if (r < 0.2f)
                maquinaEstados = estados.acercarse;
            else
                maquinaEstados = estados.ataque;
        }
        else if (modoDefensa == true) {
            if (r < 0.45f)
                maquinaEstados = estados.acercarse;
            else
                maquinaEstados = estados.ataque;
        }
        else {

            if (r < 0.5f)
                maquinaEstados = estados.acercarse;
            else
                maquinaEstados = estados.ataque;

        }
        triggerEsquivar = false;
    }

    IEnumerator Quieto() {
        triggerQuieto = true;
        StateManager.doesItMove = 1;
        while (Vector3.Distance(NPC.position, this.transform.position) > 5 ) {
            if (time2 < 5)
                inputHandler.Quieto(0, 0);
            else
                break;
            yield return null;
        }
        inputHandler.Void_Mover(0.75f, 0);
        
        maquinaEstados = estados.ataque;
        time2 = 0;
        triggerQuieto = false;

    }
}
