using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NetworkExplorer : MonoBehaviour
{

    public string NetworkName;

    public int[] topology;
    private NeuralNetwork network;

    public float DirectionLower = 0;
    public float DirectionUpper = 3;
    public float DirectionDelta;
    public float DirectionCurrent = 0;

    public float TerrainSteepnessLower = 0;
    public float TerrainSteepnessUpper = 20;
    public float TerrainSteepnessDelta = 1;
    public float TerrainSteepnessCurrent = 0;

    public float MorphologyLower = 0;
    public float MorphologyUpper = 1;

    public float MorphologyDelta = 1;
    public float MorphologyCurrent = 0;

    public bool RecrodData = true;

    private void Awake()
    {
        network = new NeuralNetwork(topology);
        network.Load("Assets/SavedNetworks/" + NetworkName + ".txt");

        DirectionCurrent = DirectionLower;
        TerrainSteepnessCurrent = TerrainSteepnessLower;
        MorphologyCurrent = MorphologyLower;

        if(RecrodData == true)
        {
            DataAnalyser.OpenFile("Network" + NetworkName + "Exploration");
        }
    
        /***
        Functionality here
        ***/

        while(true)
        {
            // float[] outputs = network.FeedForward(new float[2] {TerrainSteepnessCurrent, MorphologyCurrent});
            float val1 = 0;
            float val2 = 0;
            float val3 = 0;
            if(MorphologyCurrent == 0)
            {
                val1 = 1;
                val2 = 0;
            }

            if(MorphologyCurrent == 1)
            {
                val1 = 0;
                val2 = 1;
            }


            float[] outputs = network.FeedForward(new float[3] {val1, val2, TerrainSteepnessCurrent});

            // float[] outputs = network.FeedForward(new float[2] {val1, val2});


            // DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", DirectionCurrent);
            DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", TerrainSteepnessCurrent);
            DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", MorphologyCurrent);

            DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", outputs[0]);
            DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", outputs[1]);
            DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", outputs[2]);
            DataAnalyser.FileWriteLine("Network" + NetworkName + "Exploration", outputs[3]);

            // DirectionCurrent += DirectionDelta;

            // if(DirectionCurrent > DirectionUpper)
            //{
                //DirectionCurrent = DirectionLower;

            TerrainSteepnessCurrent += TerrainSteepnessDelta;
            if(TerrainSteepnessCurrent > TerrainSteepnessUpper)
            {
                MorphologyCurrent += MorphologyDelta;
                TerrainSteepnessCurrent = TerrainSteepnessLower;

                if(MorphologyCurrent > MorphologyUpper)
                {
                    if(RecrodData)
                    {
                        DataAnalyser.CloseAllFiles();
                    }
                    Debug.Log("Got here");

                    EditorApplication.ExitPlaymode(); // End demonstration if final target has been reached.
                    return;
                }
            }
        }
    }
}
