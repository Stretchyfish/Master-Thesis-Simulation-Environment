using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using System.Linq;


// Code coming from: https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef
// Combined with https://gist.github.com/rotolonico/6a8758c97dd00721fde3b4647cae70ed

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    public int[] layers;
    public float[][] neurons;
    public float[][] biases;
    public float[][][] weights;

    public float[][] desiredNeurons;
    public float[][] biasesSmudge;
    public float[][][] weightsSmudge;

    public float fitness = 0;

    public float WeightDecay = 0.001f;
    public float LearningRate = 0.1f;

    // Constructor
    public NeuralNetwork(int[] layers)
    {
        // Set layers equal to new layers
        /*
        this.layers = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        */

        this.layers = layers;

        // Initialize neurons
        List<float[]> neuronList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronList.Add(new float[layers[i]]);
        }
        this.neurons = neuronList.ToArray();

        // Initialize biases with random values
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < this.layers.Length; i++)
        {
            float[] bias = new float[this.layers[i]];
            for (int j = 0; j < this.layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-1, 1f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();

        // Initialize weights with random values
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < this.layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = this.layers[i - 1];

            for (int j = 0; j < this.neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-1, 1f);
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        this.weights = weightsList.ToArray();


        // Instantiate array to be used for backpropagation later
        desiredNeurons = new float[layers.Length][];
        biasesSmudge = new float[layers.Length][];
        weightsSmudge = new float[layers.Length - 1][][];

        for(int i = 0; i < layers.Length; i++)
        {
            desiredNeurons[i] = new float[layers[i]];
            biasesSmudge[i] = new float[layers[i]];
        }

        for(int i = 0; i < layers.Length - 1; i++)
        {
            weightsSmudge[i] = new float[neurons[i + 1].Length][];
            for(int j = 0; j < weights[i].Length; j++)
            {
                weightsSmudge[i][j] = new float[neurons[i].Length];
            }
        }

        /* Debug line, might be usefull in the future
        if(weights.Length == weightsSmudge.Length && weights[0].Length == weightsSmudge.Length && weights[0][0].Length == weightsSmudge[0][0].Length)
        {
            Debug.Log("They are simular");
        }
        */
    }

    public float ActivationFunction(float value)
    {
        // return (float)Math.Tanh(value); // Tanh
        return 1f / (1f + (float) Mathf.Exp(-value)); // Sigmoid
    }

    public float SigmoidDerivative(float value)
    {
        return value * (1 - value);
    }

    public void SuprevisedTraining(float[][] trainingInputs, float[][] trainingOutput)
    {
        for(int i = 0; i < trainingInputs.Length; i++)
        {
            // Feed forward get calculated output
            FeedForward(trainingInputs[i]);

            // Set the desired values to the desired training output
            for(int j = 0; j < desiredNeurons[desiredNeurons.Length - 1].Length; j++)
            {
                desiredNeurons[desiredNeurons.Length - 1][j] = trainingOutput[i][j];
            }

            // Go through every layer except the first layer, since that is input
            for(int j = neurons.Length - 1; j >= 1; j--)
            {
                for(int k = 0; k < neurons[j].Length; k++)
                {
                    float biasSmudge = SigmoidDerivative(neurons[j][k]) * (desiredNeurons[j][k] - neurons[j][k]);
                    biasesSmudge[j][k] += biasSmudge;

                    for(int l = 0; l < neurons[j - 1].Length; l++)
                    {
                        float weightSmudge = neurons[j - 1][l] * biasSmudge;
                        weightsSmudge[j - 1][k][l] += weightSmudge;

                        float valueSmudge = weights[j - 1][k][l] * biasSmudge;
                        desiredNeurons[j - 1][l] += valueSmudge;
                    }
                }
            }
        }

        for(int i = neurons.Length - 1; i >= 1; i--)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                biases[i][j] += biasesSmudge[i][j] * LearningRate;
                biases[i][j] *= 1 - WeightDecay;
                biasesSmudge[i][j] = 0;

                for(int k = 0; k < neurons[i - 1].Length; k++)
                {
                    weights[i - 1][j][k] += weightsSmudge[i - 1][j][k] * LearningRate;
                    weights[i - 1][j][k] *= 1 - WeightDecay;
                    weightsSmudge[i - 1][j][k] = 0;
                }

                desiredNeurons[i][j] = 0;
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        // Set first layer to input layer
        for (int i = 0; i < inputs.Length; i++)
        {
            this.neurons[0][i] = inputs[i];
        }

        // Feed forward calculations
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;

            for (int j = 0; j < this.neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += this.weights[i - 1][j][k] * this.neurons[i - 1][k];
                }
                this.neurons[i][j] = ActivationFunction(value + biases[i][j]);
            }
        }
        return this.neurons[neurons.Length - 1];
    }

    public void Mutate(int chance, float range)
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                if (UnityEngine.Random.Range(0, 1) < chance)
                {
                    biases[i][j] += UnityEngine.Random.Range(-range, range);
                }
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if (UnityEngine.Random.Range(0, 1) < chance)
                    {
                        weights[i][j][k] += UnityEngine.Random.Range(-range, range);
                    }
                }
            }
        }
    }

    public void Load(string path)//this loads the biases and weights from within a file into the neural network.
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }

    public void Save(string path)//this is used for saving the biases and weights within the network to a file.
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }

    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }

    public NeuralNetwork copy(NeuralNetwork nn) 
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                nn.biases[i][j] = biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    nn.weights[i][j][k] = weights[i][j][k];
                }
            }
        }
        return nn;
    }

    public NeuralNetwork copyWithFitness(NeuralNetwork nn)
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                nn.biases[i][j] = biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    nn.weights[i][j][k] = weights[i][j][k];
                }
            }
        }
        nn.fitness = fitness;

        return nn;
    }

}
