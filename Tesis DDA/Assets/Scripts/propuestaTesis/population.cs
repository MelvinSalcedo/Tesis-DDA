using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class population : MonoBehaviour {

    GameObject gaobj;

    public float mutaionRate = 0.01f;

    public bool finished = false;

    public int numMaxIteraciones = 1;

    int numPadres = 2;
    float BestFitFitness;
    int TamPoblacion = 1;
    int lengtToChainBanry;
    List<DNA> BaseDeDatosBestDNA=new List<DNA>();

    DNA comportamientoReciente;

    public population(string[] caracteristicasJuego,float[] dataForFitness) {

        lengtToChainBanry = caracteristicasJuego.Length;
        DNA populationSet = new DNA(caracteristicasJuego,dataForFitness);
        populationSet.fitnessCal();

        BaseDeDatosBestDNA.Add(populationSet);

    }

    public void AddNewCromosoma(string[] caracteristicasJuego, float[] dataForFitness) {
        DNA populationSet = new DNA(caracteristicasJuego, dataForFitness);
        populationSet.fitnessCal();
        //print("new cromosoma = "+ populationSet.fitness);
        comportamientoReciente = populationSet;
        BaseDeDatosBestDNA.Add(populationSet);
    }

    public string[] AlgoritmoGenetico() {
        string[] pesos = new string[lengtToChainBanry];

        for (int iteraciones = 0; iteraciones < numMaxIteraciones; iteraciones++) {

            BaseDeDatosBestDNA.Sort(SortByScore);
            int contador = 0;
            for (int i = 0; i < BaseDeDatosBestDNA.Count; i++) {
                if (BaseDeDatosBestDNA.Count < 5)
                    break;
                contador++;
                BaseDeDatosBestDNA.RemoveAt(i);
                i--;
            }

            /*for (int i = 0; i < BaseDeDatosBestDNA.Count; i++) {
                print(BaseDeDatosBestDNA[i].fitness);
            }*/

            //Debug.Log("CHILD " + BaseDeDatosBestDNA.Count);
            //cruzamiento de los individous selccionados


            for (int i = 0; i < 1; i++) {
                int a = Random.Range(0,BaseDeDatosBestDNA.Count);
                int b=0;
                while (b == a) {b= Random.Range(0, BaseDeDatosBestDNA.Count); }
                DNA partnerA = BaseDeDatosBestDNA[a];
                //DNA partnerB = BaseDeDatosBestDNA[b];

                DNA child = partnerA.crossOverID(comportamientoReciente);
                child.Mutate();

                pesos=child.gene;
                //Debug.Log("**************** "+ crossoverPool.Count);
                //Debug.Log("CHILD ="+ partnerB.gene[0]);
            }
        }
        return pesos;
    }

    static int SortByScore(DNA p1, DNA p2) {
        return p1.fitness.CompareTo(p2.fitness);
    }

}
