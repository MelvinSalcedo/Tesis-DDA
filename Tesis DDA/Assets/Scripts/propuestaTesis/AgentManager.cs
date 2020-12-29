using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class AgentManager : MonoBehaviour {
    public StateManager stateManager;
    public EnemyStates stateEnemy;
    public AiHandler iaHandler;

    public Animation anim;

    private int InputCountANN = 9;
    private int GeneracionActual = 0;

    private float bestFit = 0;
    private float time = 0;

    private NeuralNetwork net;
    private population p;


    private float[] inputsTO_NN;//entradas para la red nueronal
    private int[] layers;//capas de la red neuronal

    public bool ejecutarAnnAgain = false;


    //void Start
    void Start() {
        inputsTO_NN = new float[InputCountANN];
        layers = new int[] { InputCountANN,9, 9, 8};

        net = new NeuralNetwork(layers);

        string[] g = net.GetAllWeightNN();

        float[] DF = new float[7];
        DF[0] = 0; DF[1] = 100; DF[2] = 0; DF[3] = 0; DF[4] = 0; DF[5] = 0; DF[6] = 0;

        //creamos nueva pobacion y pasamos los pesos de la ANN y los datos para el calculo de fitness
        p = new population(g, DF);

    }

    //Void Update
    int numEntrenamiento = 100;
    bool prE = false;

    void Update() {

        if (numEntrenamiento <= 100) {
            AlgoritmoDDA();
            numEntrenamiento += 1;
        }
        else {
            prE = true;
            //numEntrenamiento = 0;
        }
        

        if (ejecutarAnnAgain == false && prE==true) {
            // mostramos en que generacion estamos 
            Debug.Log("_______ENTRENAMIENTO ___ " + GeneracionActual+" ");
            GeneracionActual++;

            //ejecutamos el algitmo DDA
            AlgoritmoDDA();
            ejecutarAnnAgain = true;
            prE = false;
        }

    }

    void AlgoritmoDDA() {
        
        //asignamos todos los datos para el calculo de la funcion fitness
        float[] dataForFitnessFunctions = new float[7];
        dataForFitnessFunctions[0] = stateManager.damageRecived_player;
        dataForFitnessFunctions[1] = stateManager.health;
        dataForFitnessFunctions[2] = stateManager.numHistBloquedPlayer;
        dataForFitnessFunctions[3] = stateManager.numHistLostPlayer;
        dataForFitnessFunctions[4] = stateManager.damageDealPlayer;
        dataForFitnessFunctions[5] = stateManager.damageDeal_NPC;
        dataForFitnessFunctions[6] = stateManager.timeGame;

        //extraemos todos los pesos de la ann en formato binario
        string[] g = net.GetAllWeightNN();

        //agregamos a la poblacion actual un nuevo individuo con los datos nesesarios 
        p.AddNewCromosoma(g, dataForFitnessFunctions);
        
        //extraemos todos los pesos de la la RNN
        string[] weightNN = p.AlgoritmoGenetico();

        //asignamos todas las entradas para la red neuronal
        inputsTO_NN[0] = stateManager.Distance;//distancia
        inputsTO_NN[1] = stateEnemy.health; //salud DDA
        inputsTO_NN[2] = stateManager.health; //salud player
        inputsTO_NN[3] = stateManager.timeGame;// tiempo de juego
        inputsTO_NN[4] = StateManager.usingShield;// si el player usa escudo
        inputsTO_NN[5] = StateManager.doesItMove; // si el player esta quieto(0) else(1)
        inputsTO_NN[6] = StateManager.Atacked; // si el player esta ataco hace poco 10s (1) else(0)
        inputsTO_NN[7] = StateManager.isLooking; // si el player esta enfocando (1) else(0)
        inputsTO_NN[8] = StateManager.heRetired; // si el player se retiro (1) else(0)


        //calculamos el FFP y obtenemos todos los pesos actualizados convertidos de binario a float
        float[] output = net.FeedForward(inputsTO_NN, weightNN);

        //asignamos a cada salida el float que le corresponde
        Dictionary<string, float> accionD = new Dictionary<string, float>();
        for (int i = 0; i < output.Length; i++) {
            accionD[i.ToString()] = output[i];
        }
        
        //ordenamos las salidas de mayora menor
        var result = accionD.OrderByDescending(i => i.Value);

        //solo vamos a guardarlos comportamientos que superen el 0.5f de probavilidad
        Queue<string> colaAccionesNPC = new Queue<string>();
        foreach (KeyValuePair<string, float> kvp in result) {
            if (kvp.Value > 0.5f) {
                     if (kvp.Key == "0") { colaAccionesNPC.Enqueue("acercarse"); }
                else if (kvp.Key == "1") { colaAccionesNPC.Enqueue("ataque fuerte"); }
                else if (kvp.Key == "2") { colaAccionesNPC.Enqueue("ataque debil"); }
                else if (kvp.Key == "3") { colaAccionesNPC.Enqueue("escudar"); }
                else if (kvp.Key == "4") { colaAccionesNPC.Enqueue("desencudar"); }
                else if (kvp.Key == "5") { colaAccionesNPC.Enqueue("esquivar"); }
                else if (kvp.Key == "6") { colaAccionesNPC.Enqueue("retirarse"); }
                else if (kvp.Key == "7") { colaAccionesNPC.Enqueue("observar"); }
                
            }
        }
        //guardamos los movimientos que se ecutaran en la cola de acciones del NPC
        iaHandler.colaEjecuciones = colaAccionesNPC;
    }

}


