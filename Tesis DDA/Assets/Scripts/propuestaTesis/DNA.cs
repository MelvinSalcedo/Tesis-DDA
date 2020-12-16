using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA {
    public float fitness;
    int longitud_cromosomas = 0;
    public string[] gene;
    public float mutationRate = 0.3f;

    private System.Random random;

    //Variables para el fitness
    float damageReceived_Player = 0;
    float lifePlayer = 0;

    float hitsBlocked_Player = 0;
    float hitsLossed_Player = 0;
    float damageDeal_Player = 0;
    float damageDeal_NPC = 0;

    float time = 0;

    /*
        damageReceived_Player
        lifePlayer
        hitsBlocked_Player
        hitsLossed_Player
        damageDeal_Player
        damageDeal_NPC;
        time
    */

    public DNA(string[] ChaisBinary, float[] dataForFitness) {

        damageReceived_Player = dataForFitness[0];
        lifePlayer = dataForFitness[1];

        hitsBlocked_Player = dataForFitness[2];
        hitsLossed_Player = dataForFitness[3];
        damageDeal_Player = dataForFitness[4];
        damageDeal_NPC = dataForFitness[5];

        time = dataForFitness[6];

        gene = new string[ChaisBinary.Length];
        for (int i = 0; i < ChaisBinary.Length; i++) {
            gene[i] = ChaisBinary[i];
            longitud_cromosomas += 1;
        }
    }

    public void fitnessCal() {
        if (time == 0) { time = 1;}
        if (hitsLossed_Player == 0) { hitsLossed_Player = 1;}
        if (lifePlayer == 0) { lifePlayer = 1;}

        float a = (hitsBlocked_Player / hitsLossed_Player);
        float b = (damageDeal_Player - damageDeal_NPC) / time;
        float hability = a * b;

        float dificulty = (damageDeal_NPC - damageReceived_Player) / (lifePlayer * time);

        fitness = hability - dificulty;

    }

    public DNA crossOverID(DNA partner) {
        string a = "";
        string b = "";
        string c = "";

        string[] chainBinary_s = new string[longitud_cromosomas];


        int midpt = longitud_cromosomas / 2; // Random.Range(0, target.Length);
        for (int i = 0; i < longitud_cromosomas; i++) {
            if (i <= midpt)
                chainBinary_s[i] = partner.gene[i];
            else
                chainBinary_s[i] = gene[i];
        }

        float[] dataForFitnessFunctions = new float[7];
        dataForFitnessFunctions[0] = 1;
        dataForFitnessFunctions[1] = 1;
        dataForFitnessFunctions[2] = 1;
        dataForFitnessFunctions[3] = 1;
        dataForFitnessFunctions[4] = 1;
        dataForFitnessFunctions[5] = 1;
        dataForFitnessFunctions[6] = 1;

        DNA child = new DNA(chainBinary_s, dataForFitnessFunctions);
        /*for (int i = 0; i < gene.Length; i++) {
            a += gene[i].ToString() + " ";
            b += partner.gene[i].ToString() + " ";
            c += child.gene[i].ToString() + " ";
        }
        if (a == b) {
            Debug.Log("PADRES IGUALES");
        }
        else {
            Debug.Log("a " + a);
            Debug.Log("b " + b);
            Debug.Log("c " + c);
        }*/
        
      
        return child;
    }


    public void Mutate() {

        float r = Random.Range(0f, 1f);
        

        string antes = "";
        /*for (int i = 0; i < longitud_cromosomas; i++) {
            antes += gene[i] + " | ";
        }
        Debug.Log ("A = " + antes);*/
        if (r < mutationRate) {
            Debug.Log("MUTACION");

            for (int j = 0; j < longitud_cromosomas; j++) {

                int r2 = Random.Range(0, gene[j].Length);
                string temp = "";
                int p = 0;

                for (int i = 0; i < gene[j].Length; i++) {
                    if (p ==r2) {
                        if (gene[j][i] == '0')
                            temp += "1";

                        else
                            temp += "0";
                    }
                    else {
                        temp += gene[j][i];
                    }
                    p++;
                }
                gene[j] = gene[j].Replace(gene[j], temp);
            }
        }

        /*antes = "";
        for (int i = 0; i < longitud_cromosomas; i++) {
            antes += gene[i] + " | ";
        }
        Debug.Log("D = " + antes);*/
    }
}