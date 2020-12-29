using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRecolected : MonoBehaviour {
    [Header("Victorias y derrotas")]
    public int victoriaNpcDda = 0;
    public int derrotaaNpcDda = 0;

    [Header("Acercarse camiando")]
    public int AcercarseLentamenteEx = 0;

    [Header("Retirarse")]
    public int RetirsaseEx = 0;

    [Header("Escudar")]
    public int EscudarseEx = 0;

    [Header("Essquivar")]
    public int EsquivarEx = 0;
    public int EsquivarFa = 0;

    [Header("Ataque fuerte")]
    public int Ataque_FuerteEx = 0;
    public int Ataque_FuerteFa = 0;

    [Header("Ataque debil")]
    public int Ataque_debilEx = 0;
    public int Ataque_debilFa = 0;

    [Header("Observar")]
    public int ObservarlEx = 0;

    BatallasGandasPerdidtas ResultadosBatallasGanadas=new BatallasGandasPerdidtas();
    BatallasGandasPerdidtas ResultadosBatallasPerdidas = new BatallasGandasPerdidtas();

    public static DataRecolected instancia;

    private void Awake() {
        if (DataRecolected.instancia == null) {
            DataRecolected.instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void asignarCLases() {
        if (victoriaNpcDda == 1) {
            ResultadosBatallasGanadas.AsignarVBaloresBatallasGandasPerdidtas("g",AcercarseLentamenteEx,
                RetirsaseEx,EscudarseEx,EsquivarEx,EsquivarFa,Ataque_FuerteEx,Ataque_FuerteFa,Ataque_debilEx,Ataque_debilFa,
                ObservarlEx);
        }
        if (derrotaaNpcDda == 1) {
            ResultadosBatallasPerdidas.AsignarVBaloresBatallasGandasPerdidtas("p", AcercarseLentamenteEx,
                RetirsaseEx, EscudarseEx, EsquivarEx, EsquivarFa, Ataque_FuerteEx, Ataque_FuerteFa, Ataque_debilEx, Ataque_debilFa,
                ObservarlEx);
        }
        victoriaNpcDda = 0;
        derrotaaNpcDda = 0;
        AcercarseLentamenteEx = 0;
        RetirsaseEx = 0;
        EscudarseEx = 0;

        EsquivarEx = 0;
        EsquivarFa =0;

        Ataque_debilEx = 0;
        Ataque_debilFa = 0;

        Ataque_FuerteEx = 0;
        Ataque_FuerteFa = 0;

        ObservarlEx = 0;
        ResultadosBatallasGanadas.mostrarTodosLosResultados();
        ResultadosBatallasPerdidas.mostrarTodosLosResultados();
    }
}

public class BatallasGandasPerdidtas {
    string tipoDato = "";

    public int victoriaNpcDda = 0;
    public int derrotaaNpcDda = 0;


    public int AcercarseLentamenteEx = 0;

    public int RetirsaseEx = 0;

    public int EscudarseEx = 0;

    public int EsquivarEx = 0;
    public int EsquivarFa = 0;

    
    public int Ataque_FuerteEx = 0;
    public int Ataque_FuerteFa = 0;

    public int Ataque_debilEx = 0;
    public int Ataque_debilFa = 0;

    public int ObservarlEx = 0;

    public BatallasGandasPerdidtas() {;}

    public void AsignarVBaloresBatallasGandasPerdidtas(string b, int al, int r, int esc, int esX,
        int esF, int afX, int afF, int adX, int adF, int i) {

        if (b == "g") {
            victoriaNpcDda += 1;
            tipoDato = "g";
        }
        else {
            derrotaaNpcDda += 1;
            tipoDato = "p";
        }
        AcercarseLentamenteEx += al;
        RetirsaseEx += r;
        EscudarseEx += esc;

        EsquivarEx += esX;
        EsquivarFa += esF;
        Ataque_FuerteEx += afX;
        Ataque_FuerteFa += afF;
        Ataque_debilEx += adX;
        Ataque_debilFa += adF;
        ObservarlEx += i;

    }

    public void mostrarTodosLosResultados() {

        if (tipoDato == "g") {
            Debug.Log("--------------------RESULTADOS DE BATALLAS GANADAS--------------------");
            Debug.Log(victoriaNpcDda);
        }
        if (tipoDato == "p") {
            Debug.Log("--------------------RESULTADOS DE BATALLAS PERDIDAS--------------------");
            Debug.Log(derrotaaNpcDda);
        }

        Debug.Log("AcercarseLentamenteEx = "+ AcercarseLentamenteEx.ToString());
        Debug.Log("RetirsaseEx = " + RetirsaseEx.ToString());
        Debug.Log("EscudarseEx = " + EscudarseEx.ToString());

        Debug.Log("EsquivarEx = " + EsquivarEx.ToString());
        Debug.Log("EsquivarFa = " + EsquivarFa.ToString());
        Debug.Log("Ataque_FuerteEx = " + Ataque_FuerteEx.ToString());
        Debug.Log("Ataque_FuerteFa = " + Ataque_FuerteFa.ToString());
        Debug.Log("Ataque_debilEx = " + Ataque_debilEx.ToString());
        Debug.Log("Ataque_debilFa = " + Ataque_debilFa.ToString());
        Debug.Log("ObservarlEx = " + ObservarlEx.ToString());
    }
}