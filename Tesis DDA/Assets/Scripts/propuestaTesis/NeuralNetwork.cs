using UnityEngine;
using System.Collections.Generic;
using System;

public class NeuralNetwork{
    private int[] layers; //layers
    private float[][] neurons; //neuron matrix
    private float[][][] weights; //weight matrix
    private float fitness; //fitness of the network


    /// Inicia una red neuronal con pesos aleatorios
    public NeuralNetwork(int[] layers)
    {
        // copia profunda de capas de esta red
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
            //Debug.Log(layers[i]);
        }
        //generate matrix
        InitNeurons();//creamos la matriz de neuronas con 0
        InitWeights();
    }

    /// Constructor de copia profunda
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    /// Creamos una matriz de neuronas
    private void InitNeurons(){
        List<float[]> neuronsList = new List<float[]>();

        // recorre todas las capas
        for (int i = 0; i < layers.Length; i++) {
            // agregar capa a la lista de neuronas
            neuronsList.Add(new float[layers[i]]);

            //Debug.Log("Lenght " + neuronsList[i].Length);
        }
        // convertir lista en matriz
        neurons = neuronsList.ToArray();
        /*Debug.Log("Neurons");
        for (int i = 0; i < neurons.Length; i++) {
            for (int j = 0; j < neurons[i].Length; j++) {
                Debug.Log(i+" = "+neurons[i][j]);
            }
        }*/
    }

    // Creamos matriz de pesos.
    private void InitWeights()
    {
        // lista de pesos que luego se convertirá en una matriz de pesos 3D
        List<float[][]> weightsList = new List<float[][]>();

        
        for (int i = 1; i < layers.Length; i++) {//todas las capas -1

            List<float[]> layerWeightsList = new List<float[]>(); 
            int neuronsInPreviousLayer = layers[i - 1]; //capa anterior
            //Debug.Log("________________________________");
            for (int j = 0; j < neurons[i].Length; j++) { //Capa actual
                
                
                float[] neuronWeights = new float[neuronsInPreviousLayer]; //w capa anterior

                // iterar sobre todas las neuronas en la capa anterior 
                //y establecer los pesos aleatoriamente entre 0.5f y -0.5
                string a = "";
                for (int k = 0; k < neuronsInPreviousLayer; k++)//cantidad de neronas por capa(previoas)
                {
                    // dar pesos aleatorios a los pesos de las neuronas
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f,0.5f);
                    //neuronWeights[k] = i; ;
                    a += neuronWeights[k].ToString()+" | ";
                }
                //Debug.Log(j +" capa "+ (i-1) +","+i + " pesos = " +a);
                // agrega pesos de neuronas de esta capa actual a pesos de capa
                layerWeightsList.Add(neuronWeights); 
            }
            // agregue los pesos de estas capas convertidos en matriz 2D en la lista de pesos
            weightsList.Add(layerWeightsList.ToArray()); 
        }

        weights = weightsList.ToArray(); // convertir a matriz 3D
    }

    // Alimenta esta red neuronal con una matriz de entrada determinada

    private double Sigmoide(float t) {
        double p = 1 / (1 + Math.Pow(Math.E, -t));
        return p;
            
    }

    public float[] FeedForward(float[] inputs, string[] binaryChain) {

        string pesosRNN = "";
        int pos=0;
        for (int i = 0; i < weights.Length; i++) {
            for (int j = 0; j < weights[i].Length; j++) {
                for (int k = 0; k < weights[i][j].Length; k++) {

                    float num = BinaryStringToSingle(binaryChain[pos]);
                    pos++;
                    weights[i][j][k]= num;
                    pesosRNN += num.ToString() + " | "; 
                }
            }
        }

        //Add inputs to the neuron matrix
        for (int i = 0; i < inputs.Length; i++) {
            neurons[0][i] = inputs[i];
        }

        // iterar sobre todas las neuronas y computar los valores de feedforward.
        string a = "";
        for (int i = 1; i < layers.Length; i++) {
            for (int j = 0; j < neurons[i].Length; j++) {
                float value = 0f;

                //suma de todas las conexiones de pesos de esta neurona
                //  pondera sus valores en la capa anterior
                for (int k = 0; k < neurons[i - 1].Length; k++) {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                a += value.ToString() + " | ";
                neurons[i][j] = (float)Sigmoide(value); //Hyperbolic tangent activation
            }
            //Debug.Log(a);
        }

        return neurons[neurons.Length - 1]; //return output layer
    }

    // Mutar los pesos de las redes neuronales
    public string[] GetAllWeightNN(){
        string pesosRNN = "";

        List<string> weightTtemporary=new List<string>();
        int numCapas = 0;
        for (int i = 0; i < weights.Length; i++) {
            for (int j = 0; j < weights[i].Length; j++) {
                for (int k = 0; k < weights[i][j].Length; k++) {
                    numCapas += 1;
                    float weight = weights[i][j][k];
                    pesosRNN += weight.ToString() + " | ";
                    string binary = SingleToBinaryString(weight);
                    weightTtemporary.Add(binary);
                }
            }
        }

        string[] g = weightTtemporary.ToArray();
        return g;
    }
   
    public static string SingleToBinaryString(float f) {
        byte[] b = BitConverter.GetBytes(f);
        int i = BitConverter.ToInt32(b, 0);
        return Convert.ToString(i, 2);
    }

    public static float BinaryStringToSingle(string s) {
        int i = Convert.ToInt32(s, 2);
        byte[] b = BitConverter.GetBytes(i);
        return BitConverter.ToSingle(b, 0);
    }
}
