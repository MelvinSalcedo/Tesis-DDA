using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class AgentManager : MonoBehaviour {
    public AiHandler iaHandler;

    private int InputCount = 6;
    public float time = 0;
    private int[] layers;
    private List<NeuralNetwork> nets;

    public float bestFit = 0;

    NeuralNetwork net;

    public Animation anim;

    population p;

    float[] inputsTO_NN;

   public StateManager stateManager;
    public EnemyStates stateEnemy;

    
    public bool ejecutarAnnAgain=false;
    void Start() {
        inputsTO_NN = new float[InputCount];
        
        Debug.Log("num entradas " + InputCount);

        layers = new int[] {InputCount,7,8};
        net = new NeuralNetwork(layers);

        string[] g = net.GetAllWeightNN();

        float[] DF = new float[7];
        DF[0] = 0; DF[1] = 100; DF[2] = 0; DF[3] = 0; DF[4] = 0; DF[5] = 0; DF[6] = 0;
        p = new population(g, DF);


        //Debug.Log(stateManager.health);
    }

    int C = 0;
    void Update() {
        
        time += Time.deltaTime;
        if (ejecutarAnnAgain==false) {
            Debug.Log("GENERACION " + C);
            C++;
            ejecutarDDA();
            ejecutarAnnAgain = true;
        }
        if (time <= -1) {
            time = 0;
            ejecutarDDA();
        }
    }

    void ejecutarDDA() {
        string[] g = net.GetAllWeightNN();
        /*
            damageReceived_Player
            lifePlayer
            hitsBlocked_Player
            hitsLossed_Player
            damageDeal_Player
            damageDeal_NPC;
            time
        */
        //DATOS PARA LA FUNCION FITNESS
        float[] dataForFitnessFunctions = new float[7];
        dataForFitnessFunctions[0] = stateManager.damageRecived_player;
        dataForFitnessFunctions[1] = stateManager.health;
        dataForFitnessFunctions[2] = stateManager.numHistBloquedPlayer;
        dataForFitnessFunctions[3] = stateManager.numHistLostPlayer;
        dataForFitnessFunctions[4] = stateManager.damageDealPlayer;
        dataForFitnessFunctions[5] = stateManager.damageDeal_NPC;
        dataForFitnessFunctions[6] = stateManager.timeGame;



        p.AddNewCromosoma(g, dataForFitnessFunctions);
        //p.InterationsForGA = 0;

        string[] weightNN = p.AlgoritmoGenetico();

        //ENTRADAS A LA RED NEURONAL
        inputsTO_NN[0] = stateManager.Distance;
        inputsTO_NN[1] = stateEnemy.health;
        inputsTO_NN[2] = stateManager.health;
        inputsTO_NN[3] = Random.Range(0, 2);//stateEnemy.FuiHerodoRecientemente;
        inputsTO_NN[4] = Random.Range(0, 2);
        inputsTO_NN[5] = Random.Range(0, 2);//stateEnemy.RecientementeBloqueeUnAtaque;
                                            //inputsTO_NN[7] = Random.Range(0, 2);

        float[] output = net.FeedForward(inputsTO_NN, weightNN);

        //iaHandler.myQueue.Clear();

        string caracteristicas = "";
        Dictionary<string, float> accionD = new Dictionary<string, float>();
        for (int i = 0; i < output.Length; i++) {
            accionD[i.ToString()] = output[i];
        }

        var result = accionD.OrderByDescending(i => i.Value);

        Queue<string> colaAccionesNPC = new Queue<string>();

        foreach (KeyValuePair<string, float> kvp in result) {
            //Debug.Log("k = "+kvp.Key +" v = "+ kvp.Value);
            if (kvp.Value > 0.5f) {
                if (kvp.Key == "0") { colaAccionesNPC.Enqueue("acercarse"); }
                
                else if (kvp.Key == "1") { colaAccionesNPC.Enqueue("ataque fuerte"); }
                else if (kvp.Key == "2") { colaAccionesNPC.Enqueue("ataque debil"); }
                
                else if (kvp.Key == "3") { colaAccionesNPC.Enqueue("escudar"); }
                else if (kvp.Key == "4") { colaAccionesNPC.Enqueue("esquivar"); }
                else if (kvp.Key == "5") { colaAccionesNPC.Enqueue("retirarse"); }
                else if (kvp.Key == "6") { colaAccionesNPC.Enqueue("observar"); }
                else if (kvp.Key == "7") { colaAccionesNPC.Enqueue("ignorar"); }
            }
        }

        /*while (colaAccionesNPC.Count > 0) {
            string namec = colaAccionesNPC.Dequeue();
            Debug.Log("comportamiento ===== " + namec);
        }*/


        float temporal = 0;
        string llave = "";


        for (int i = 0; i < inputsTO_NN.Length; i++) {
            caracteristicas += inputsTO_NN[i].ToString() + " | ";
        }
        Debug.Log("Entradas = " + caracteristicas);

        caracteristicas = "";
        for (int i = 0; i < output.Length; i++) {
            caracteristicas += output[i].ToString() + " | ";
            if (temporal < output[i]) {
                temporal = output[i];
                llave = i.ToString();
            }
        }
        //Debug.Log("caracteristicas = " + caracteristicas);

        if (llave == "0") { llave = "acercarse"; }
        else if (llave == "1") { llave = "ataque fuerte"; }
        else if (llave == "2") { llave = "ataque debil"; }
        else if (llave == "3") { llave = "retirarse"; }
        else if (llave == "4") { llave = "escudar"; }
        else if (llave == "5") { llave = "esquivar"; }
        else if (llave == "6") { llave = "observar"; }
        else if (llave == "7") { llave = "ignorar"; }

        iaHandler.SolaAccion = llave;
        iaHandler.colaEjecuciones = colaAccionesNPC;
        //Debug.Log("Comportamiento ======> "+ llave);

        for (int i = 0; i < output.Length; i++) {

            //Debug.Log("salida = " + output[i]);
        }

    }

}



