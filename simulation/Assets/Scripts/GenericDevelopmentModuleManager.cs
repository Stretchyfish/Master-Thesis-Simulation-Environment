/***************************************************************************************
* Generic Development Module Manager
* Author: Mikkel Skov Maarssø
*
* The Genetic Algorithm part of the code is heavily inspired by the implementation on: https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef
****************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum SimulationMode
{
    Training,
    Demonstration,
    Parameters
}

public class GenericDevelopmentModuleManager : MonoBehaviour
{
    /******************** General Variables *************************/
    [HideInInspector]
    public SimulationMode currentMode = SimulationMode.Training;

    public GenericController HexController;

    public GenericController QuadController;

    public GameObject FlatTerrain;
    public GameObject DifficultTerrain;

    public List<Transform> FlatTargetList;
    public List<Transform> DifficultTargetList;

    public GameObject RobotCamera;

    public GraphControllerStatic TrainingUI;

    public GenerationCounterUI GenCounterUI;

    [SerializeField] private float currentTimeScale = 1;
    [SerializeField] private float desiredTimeScale = 1;
    
    [SerializeField] private bool RandomInstanstiationRotation = false;

    /******************** Training variables **************************/
    
    public enum TrainingType
    {
        suprevised,
        GeneticAlgorithm
    }
    [SerializeField] public TrainingType currentTrainingType = TrainingType.GeneticAlgorithm;

    private enum RobotModelToTrain
    {
        Hexapod, Quadruped, Both
    }
    [SerializeField] private RobotModelToTrain currentRobotTrain = RobotModelToTrain.Hexapod;

    public int[] TrainingNetworkTopology;

    public List<GenericController> population;
    public List<NeuralNetwork> networks;

    public int populationSize = 50;

    [Tooltip("The amount of time that passes before the current generation gets evaluated")]
    public float EvaluationTime = 60;

    private float EvaluationCounter = 0; // Time passed since start 

    public float MutationChance = 0.1f;
    public float MutationEffect = 0.5f;

    [Tooltip("The number of new generations generated before ending training")]
    
    public bool UseGenerationCriteria = true;
    public int GenerationCriteria = 50; 

    public bool UseFitnessCriteria = false;
    public float FitnessCriteria = 1;

    public bool SaveBestNetworkLastGen = true;
    public string BestNetworkLastGenName;

    public bool SaveBestNetworkOverall = false;
    public string BestNetworkOverallName;


    [SerializeField] private int currentGenerationNumber = 1;

    [SerializeField] private List<float> DebugAllFitness = new List<float>();

    [SerializeField] private float DebugAverageFitness = 0;

    [SerializeField] private float DebugBestFitnessOverall = Mathf.NegativeInfinity; // Change dependend on fitness

    [SerializeField] private bool RecordTrainingData = false;

    // Tip : [NonSerializedField] <--

    /******************** SUPREVISED LEARNING *************************/
    public int[] SuprevisedNetworkTopology;

    private NeuralNetwork SuprevisedNeuralNetwork;

    public float[] TestInput;
    // public float[] TestOutput;

    /******************** Demonstration variables *********************/

    private enum RobotModelToSimulate
    {
        Hexapod, Quadruped, Both
    }
    [SerializeField] private RobotModelToSimulate currentRobotSim = RobotModelToSimulate.Hexapod;

    private enum TerrainType
    {
        flat, difficult
    }
    [SerializeField] private TerrainType currentTerrainType = TerrainType.flat;

    public enum ControlType
    {
        StateMachine,
        Braintenberg,
        ClosedLoopControl,
        NetworkBased
    }
    [SerializeField] public ControlType currentControlType = ControlType.StateMachine;

    public bool LoadNetwork = false;
    public bool CreateRandomNetwork = false;
    public string LoadedNetworkName;
    public int[] DemonstrationNetworkTopology;

    public bool RecordData = false;

    /******************** Parameter timing variables ******************/

    public bool SaveParameterNetwork = false;
    public string ParameterNetworkName;


    /*************************** MAIN LOOPS ***************************/

    private void Awake()
    {
        switch(currentMode)
        {
            case SimulationMode.Demonstration:
                AwakeDemonstrationHandler();
                break;

            case SimulationMode.Training:
                AwakeTrainingHandler();
                break;

            case SimulationMode.Parameters:
                AwakeParameterHandler();
                break;
        }
    }

    private void AwakeDemonstrationHandler()
    {
        if(RecordData)
        {
            // Instantiate files for recording
            BeginRecording();
        }

        // Choose a target list based on the robot type
        List<Transform> targets = new List<Transform>();
        switch(currentTerrainType)
        {
            case TerrainType.flat:
                targets = FlatTargetList;
                break;

            case TerrainType.difficult:
                targets = DifficultTargetList;
                break;
        }

        if(LoadNetwork && CreateRandomNetwork)
        {
            Debug.Log("Both load network and create random network is active, this will cause problems");
        }

        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:
                HexController.gameObject.SetActive(true);
                QuadController.gameObject.SetActive(false);

                HexController.SetupRobot();
                HexController.ResetRobot(RandomInstanstiationRotation);
                HexController.Targets = targets.ToArray();

                if(LoadNetwork)
                {
                    HexController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                    HexController.network.Load("Assets/SavedNetworks/" + LoadedNetworkName + ".txt");
                }

                if(CreateRandomNetwork)
                {
                    HexController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                }

                break;

            case RobotModelToSimulate.Quadruped:
                HexController.gameObject.SetActive(false);
                QuadController.gameObject.SetActive(true);

                QuadController.SetupRobot();
                QuadController.ResetRobot(RandomInstanstiationRotation);
                QuadController.Targets = targets.ToArray();

                if(LoadNetwork)
                {
                    QuadController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                    QuadController.network.Load("Assets/SavedNetworks/" + LoadedNetworkName + ".txt");
                }

                if(CreateRandomNetwork)
                {
                    QuadController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                }

                break;

            case RobotModelToSimulate.Both:
                HexController.gameObject.SetActive(true);
                QuadController.gameObject.SetActive(true);

                HexController.SetupRobot();
                HexController.ResetRobot(RandomInstanstiationRotation);
                HexController.Targets = targets.ToArray();

                QuadController.SetupRobot();
                QuadController.ResetRobot(RandomInstanstiationRotation);
                QuadController.Targets = targets.ToArray();

                if(LoadNetwork)
                {
                    HexController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                    HexController.network.Load("Assets/SavedNetworks/" + LoadedNetworkName + ".txt");

                    QuadController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                    QuadController.network.Load("Assets/SavedNetworks/" + LoadedNetworkName + ".txt");
                }

                if(CreateRandomNetwork)
                {
                    HexController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                    QuadController.network = new NeuralNetwork(DemonstrationNetworkTopology);
                }

                break;
        }
    }

    private GraphControllerStatic SuprevisedGraph;

    private void AwakeTrainingHandler()
    {
        // Place Camera Appropriatly
        //RobotCamera.transform.position = new Vector3(0, 15, -45);

        switch(currentTrainingType)
        {
            case TrainingType.suprevised:

                SuprevisedGraph = GameObject.Find("/Canvas/Training UI/EpocTraining").GetComponent<GraphControllerStatic>();
                SuprevisedGraph.gameObject.SetActive(true);                

                SuprevisedNeuralNetwork = new NeuralNetwork(SuprevisedNetworkTopology);

                float[][] trainingInput;

                List<float[]> GeneratedInput = new List<float[]>();

                int a = 1;
                int b = 0;
                int c = -11;
                for(int i = 0; i < 40; i++)
                {
                    float[] newInput = new float[3];

                    c += 1;

                    if(c > 10)
                    {
                        a = 0;
                        b = 1;
                        c = -10;
                    }

                    newInput[0] = a;
                    newInput[1] = b;
                    newInput[2] = c;

                    /*
                    if(i < 50) // Old
                    {
                        newInput[0] = 1;
                        newInput[1] = 0;
                        // newInput[2] = UnityEngine.Random.Range(-10, 10);
                        newInput[2] = 0;
                    }
                    else
                    {
                        newInput[0] = 0;
                        newInput[1] = 1;
                        newInput[2] = 0;

                        // newInput[2] = UnityEngine.Random.Range(-10, 10);
                    }
                    */

                    GeneratedInput.Add(newInput);
                }
                trainingInput = GeneratedInput.ToArray();

                float[][] trainingOutput;

                List<float[]> GeneratedOutput = new List<float[]>();
                for(int i = 0; i < 40; i++)
                {
                    float[] newOutput = new float[4];

                    switch(i)
                    {
                        case 0: 
                        newOutput[0] = 0.995139718055725f;
                        newOutput[1] = 0.0253825038671494f;
                        newOutput[2] = 0.463771343231201f;
                        newOutput[3] = 0.998518407344818f;
                            break;

                        case 1:
                        newOutput[0] = 0.992087721824646f;
                        newOutput[1] = 0.0342708267271519f;
                        newOutput[2] = 0.491653889417648f;
                        newOutput[3] = 0.996902823448181f;
                            break;

                        case 2:
                        newOutput[0] = 0.98714405298233f;
                        newOutput[1] = 0.046f;
                        newOutput[2] = 0.519f;
                        newOutput[3] = 0.993f;
                            break;       

                        case 3:
                        newOutput[0] = 0.979f;
                        newOutput[1] = 0.061f;
                        newOutput[2] = 0.547f;
                        newOutput[3] = 0.986f;           
                            break;        

                        case 4:
                        newOutput[0] = 0.966f;
                        newOutput[1] = 0.082f;
                        newOutput[2] = 0.574f;
                        newOutput[3] = 0.972f;           
                            break;        


                        case 5:
                        newOutput[0] = 0.946f;
                        newOutput[1] = 0.108f;
                        newOutput[2] = 0.601f;
                        newOutput[3] = 0.943f;    
                            break;

                        case 6:
                        newOutput[0] = 0.915f;
                        newOutput[1] = 0.142f;
                        newOutput[2] = 0.628f;
                        newOutput[3] = 0.888f;    
                            break;

                        case 7:
                        newOutput[0] = 0.868f;
                        newOutput[1] = 0.185f;
                        newOutput[2] = 0.654f;
                        newOutput[3] = 0.792f;    
                            break;

                        case 8:
                        newOutput[0] = 0.801f;
                        newOutput[1] = 0.236f;
                        newOutput[2] = 0.678f;
                        newOutput[3] = 0.645f;    
                            break;

                        case 9:
                        newOutput[0] = 0.7126f;
                        newOutput[1] = 0.296f;
                        newOutput[2] = 0.702f;
                        newOutput[3] = 0.465f;  
                            break;
                        
                        case 10:
                        newOutput[0] = 0.602f;
                        newOutput[1] = 0.364f;
                        newOutput[2] = 0.725f;
                        newOutput[3] = 0.293f;  
                            break;

                        case 11:
                        newOutput[0] = 0.481f;
                        newOutput[1] = 0.439f;
                        newOutput[2] = 0.747f;
                        newOutput[3] = 0.165f;  
                            break;

                        case 12:
                        newOutput[0] = 0.362f;
                        newOutput[1] = 0.516f;
                        newOutput[2] = 0.7678f;
                        newOutput[3] = 0.086f;  
                            break;

                        case 13:
                        newOutput[0] = 0.258f;
                        newOutput[1] = 0.592f;
                        newOutput[2] = 0.787f;
                        newOutput[3] = 0.043f;  
                            break;

                        case 14:
                        newOutput[0] = 0.176f;
                        newOutput[1] = 0.664f;
                        newOutput[2] = 0.805f;
                        newOutput[3] = 0.021f;  
                            break;

                        case 15:
                        newOutput[0] = 0.115f;
                        newOutput[1] = 0.729f;
                        newOutput[2] = 0.822f;
                        newOutput[3] = 0.010f;  
                            break;                        

                        case 16:
                        newOutput[0] = 0.074f;
                        newOutput[1] = 0.786f;
                        newOutput[2] = 0.837f;
                        newOutput[3] = 0.004f;  
                            break;

                        case 17:
                        newOutput[0] = 0.0467f;
                        newOutput[1] = 0.833f;
                        newOutput[2] = 0.852f;
                        newOutput[3] = 0.002f;  
                            break;

                        case 18:
                        newOutput[0] = 0.029f;
                        newOutput[1] = 0.872f;
                        newOutput[2] = 0.866f;
                        newOutput[3] = 0.001f;  
                            break;

                        case 19:
                        newOutput[0] = 0.018f;
                        newOutput[1] = 0.902f;
                        newOutput[2] = 0.878f;
                        newOutput[3] = 0.0005f;  
                            break;

                        case 20:
                        newOutput[0] = 0.011f;
                        newOutput[1] = 0.926f;
                        newOutput[2] = 0.889f;
                        newOutput[3] = 0.0002f;  
                            break;

                        // Hexapod below

                        case 21: 
                        newOutput[0] = 0.373f;
                        newOutput[1] = 0.463f;
                        newOutput[2] = 0.865f;
                        newOutput[3] = 0.914f;
                            break;

                        case 22:
                        newOutput[0] = 0.371f;
                        newOutput[1] = 0.449f;
                        newOutput[2] = 0.856f;
                        newOutput[3] = 0.890f;
                            break;

                        case 23:
                        newOutput[0] = 0.369f;
                        newOutput[1] = 0.435f;
                        newOutput[2] = 0.846f;
                        newOutput[3] = 0.861f;
                            break;       

                        case 24:
                        newOutput[0] = 0.367f;
                        newOutput[1] = 0.421f;
                        newOutput[2] = 0.835f;
                        newOutput[3] = 0.825f;           
                            break;        

                        case 25:
                        newOutput[0] = 0.365f;
                        newOutput[1] = 0.407f;
                        newOutput[2] = 0.824f;
                        newOutput[3] = 0.781f;           
                            break;        


                        case 26:
                        newOutput[0] = 0.363f;
                        newOutput[1] = 0.393f;
                        newOutput[2] = 0.812f;
                        newOutput[3] = 0.731f;    
                            break;

                        case 27:
                        newOutput[0] = 0.361f;
                        newOutput[1] = 0.380f;
                        newOutput[2] = 0.800f;
                        newOutput[3] = 0.674f;    
                            break;

                        case 28:
                        newOutput[0] = 0.359f;
                        newOutput[1] = 0.367f;
                        newOutput[2] = 0.787f;
                        newOutput[3] = 0.611f;    
                            break;

                        case 29:
                        newOutput[0] = 0.357f;
                        newOutput[1] = 0.353f;
                        newOutput[2] = 0.773f;
                        newOutput[3] = 0.544f;    
                            break;

                        case 30:
                        newOutput[0] = 0.355f;
                        newOutput[1] = 0.340f;
                        newOutput[2] = 0.759f;
                        newOutput[3] = 0.476f;  
                            break;
                        
                        case 31:
                        newOutput[0] = 0.353f;
                        newOutput[1] = 0.328f;
                        newOutput[2] = 0.744f;
                        newOutput[3] = 0.408f;  
                            break;

                        case 32:
                        newOutput[0] = 0.351f;
                        newOutput[1] = 0.315f;
                        newOutput[2] = 0.729f;
                        newOutput[3] = 0.344f;  
                            break;

                        case 33:
                        newOutput[0] = 0.349f;
                        newOutput[1] = 0.303f;
                        newOutput[2] = 0.713f;
                        newOutput[3] = 0.285f;  
                            break;

                        case 34:
                        newOutput[0] = 0.347f;
                        newOutput[1] = 0.291f;
                        newOutput[2] = 0.696f;
                        newOutput[3] = 0.232f;  
                            break;

                        case 35:
                        newOutput[0] = 0.346f;
                        newOutput[1] = 0.280f;
                        newOutput[2] = 0.679f;
                        newOutput[3] = 0.187f;  
                            break;

                        case 36:
                        newOutput[0] = 0.344f;
                        newOutput[1] = 0.268f;
                        newOutput[2] = 0.662f;
                        newOutput[3] = 0.148f;  
                            break;                        

                        case 37:
                        newOutput[0] = 0.342f;
                        newOutput[1] = 0.257f;
                        newOutput[2] = 0.644f;
                        newOutput[3] = 0.117f;  
                            break;

                        case 38:
                        newOutput[0] = 0.340f;
                        newOutput[1] = 0.247f;
                        newOutput[2] = 0.626f;
                        newOutput[3] = 0.091f;  
                            break;

                        case 39:
                        newOutput[0] = 0.338f;
                        newOutput[1] = 0.236f;
                        newOutput[2] = 0.607f;
                        newOutput[3] = 0.071f;  
                            break;

                        case 40:
                        newOutput[0] = 0.336f;
                        newOutput[1] = 0.226f;
                        newOutput[2] = 0.588f;
                        newOutput[3] = 0.055f;  
                            break;
                    }

                    /*
                    if(i < 50) // Old
                    {
                        newOutput[0] = 0.1904f;
                        newOutput[1] = 0.589f;
                        newOutput[2] = 0.662f;
                        newOutput[3] = 0.116f;
                    }
                    else
                    {
                        newOutput[0] = 0.375f;
                        newOutput[1] = 0.300f;
                        newOutput[2] = 0.759f;
                        newOutput[3] = 0.391f;
                    }
                    */
                    GeneratedOutput.Add(newOutput);
                }
                trainingOutput = GeneratedOutput.ToArray();

                for(int epoc = 0; epoc < 1000; epoc++)
                {
                    SuprevisedNeuralNetwork.SuprevisedTraining(trainingInput, trainingOutput);

                    float SumError = 0;

                    float a1 = 1;
                    float b1 = 0;
                    float c1 = -11;
                    for(int i = 0; i < 40; i++)
                    {
                        c1 += 1;

                        if(c1 > 10)
                        {
                            a1 = 0;
                            b1 = 1;
                            c1 = -10;
                        }

                        float[] testOutputs = SuprevisedNeuralNetwork.FeedForward(new float[] {a1, b1, c1});

                        float[] newOutput = new float[4];

                        switch(i)
                        {
                            case 0: 
                            newOutput[0] = 0.995f;
                            newOutput[1] = 0.025f;
                            newOutput[2] = 0.463f;
                            newOutput[3] = 0.998f;
                                break;

                            case 1:
                            newOutput[0] = 0.992f;
                            newOutput[1] = 0.034f;
                            newOutput[2] = 0.491f;
                            newOutput[3] = 0.996f;
                                break;

                            case 2:
                            newOutput[0] = 0.9871f;
                            newOutput[1] = 0.046f;
                            newOutput[2] = 0.519f;
                            newOutput[3] = 0.993f;
                                break;       

                            case 3:
                            newOutput[0] = 0.979f;
                            newOutput[1] = 0.061f;
                            newOutput[2] = 0.547f;
                            newOutput[3] = 0.986f;           
                                break;        

                            case 4:
                            newOutput[0] = 0.966f;
                            newOutput[1] = 0.082f;
                            newOutput[2] = 0.574f;
                            newOutput[3] = 0.972f;           
                                break;        

                            case 5:
                            newOutput[0] = 0.946f;
                            newOutput[1] = 0.108f;
                            newOutput[2] = 0.601f;
                            newOutput[3] = 0.943f;    
                                break;

                            case 6:
                            newOutput[0] = 0.915f;
                            newOutput[1] = 0.142f;
                            newOutput[2] = 0.628f;
                            newOutput[3] = 0.888f;    
                                break;

                            case 7:
                            newOutput[0] = 0.868f;
                            newOutput[1] = 0.185f;
                            newOutput[2] = 0.654f;
                            newOutput[3] = 0.792f;    
                                break;

                            case 8:
                            newOutput[0] = 0.801f;
                            newOutput[1] = 0.236f;
                            newOutput[2] = 0.678f;
                            newOutput[3] = 0.645f;    
                                break;

                            case 9:
                            newOutput[0] = 0.7126f;
                            newOutput[1] = 0.296f;
                            newOutput[2] = 0.702f;
                            newOutput[3] = 0.465f;  
                                break;
                            
                            case 10:
                            newOutput[0] = 0.602f;
                            newOutput[1] = 0.364f;
                            newOutput[2] = 0.725f;
                            newOutput[3] = 0.293f;  
                                break;

                            case 11:
                            newOutput[0] = 0.481f;
                            newOutput[1] = 0.439f;
                            newOutput[2] = 0.747f;
                            newOutput[3] = 0.165f;  
                                break;

                            case 12:
                            newOutput[0] = 0.362f;
                            newOutput[1] = 0.516f;
                            newOutput[2] = 0.7678f;
                            newOutput[3] = 0.086f;  
                                break;

                            case 13:
                            newOutput[0] = 0.258f;
                            newOutput[1] = 0.592f;
                            newOutput[2] = 0.787f;
                            newOutput[3] = 0.043f;  
                                break;

                            case 14:
                            newOutput[0] = 0.176f;
                            newOutput[1] = 0.664f;
                            newOutput[2] = 0.805f;
                            newOutput[3] = 0.021f;  
                                break;

                            case 15:
                            newOutput[0] = 0.115f;
                            newOutput[1] = 0.729f;
                            newOutput[2] = 0.822f;
                            newOutput[3] = 0.010f;  
                                break;                        

                            case 16:
                            newOutput[0] = 0.074f;
                            newOutput[1] = 0.786f;
                            newOutput[2] = 0.837f;
                            newOutput[3] = 0.004f;  
                                break;

                            case 17:
                            newOutput[0] = 0.0467f;
                            newOutput[1] = 0.833f;
                            newOutput[2] = 0.852f;
                            newOutput[3] = 0.002f;  
                                break;

                            case 18:
                            newOutput[0] = 0.029f;
                            newOutput[1] = 0.872f;
                            newOutput[2] = 0.866f;
                            newOutput[3] = 0.001f;  
                                break;

                            case 19:
                            newOutput[0] = 0.018f;
                            newOutput[1] = 0.902f;
                            newOutput[2] = 0.878f;
                            newOutput[3] = 0.0005f;  
                                break;

                            case 20:
                            newOutput[0] = 0.011f;
                            newOutput[1] = 0.926f;
                            newOutput[2] = 0.889f;
                            newOutput[3] = 0.0002f;  
                                break;

                            // Hexapod below

                            case 21: 
                            newOutput[0] = 0.373f;
                            newOutput[1] = 0.463f;
                            newOutput[2] = 0.865f;
                            newOutput[3] = 0.914f;
                                break;

                            case 22:
                            newOutput[0] = 0.371f;
                            newOutput[1] = 0.449f;
                            newOutput[2] = 0.856f;
                            newOutput[3] = 0.890f;
                                break;

                            case 23:
                            newOutput[0] = 0.369f;
                            newOutput[1] = 0.435f;
                            newOutput[2] = 0.846f;
                            newOutput[3] = 0.861f;
                                break;       

                            case 24:
                            newOutput[0] = 0.367f;
                            newOutput[1] = 0.421f;
                            newOutput[2] = 0.835f;
                            newOutput[3] = 0.825f;           
                                break;        

                            case 25:
                            newOutput[0] = 0.365f;
                            newOutput[1] = 0.407f;
                            newOutput[2] = 0.824f;
                            newOutput[3] = 0.781f;           
                                break;        


                            case 26:
                            newOutput[0] = 0.363f;
                            newOutput[1] = 0.393f;
                            newOutput[2] = 0.812f;
                            newOutput[3] = 0.731f;    
                                break;

                            case 27:
                            newOutput[0] = 0.361f;
                            newOutput[1] = 0.380f;
                            newOutput[2] = 0.800f;
                            newOutput[3] = 0.674f;    
                                break;

                            case 28:
                            newOutput[0] = 0.359f;
                            newOutput[1] = 0.367f;
                            newOutput[2] = 0.787f;
                            newOutput[3] = 0.611f;    
                                break;

                            case 29:
                            newOutput[0] = 0.357f;
                            newOutput[1] = 0.353f;
                            newOutput[2] = 0.773f;
                            newOutput[3] = 0.544f;    
                                break;

                            case 30:
                            newOutput[0] = 0.355f;
                            newOutput[1] = 0.340f;
                            newOutput[2] = 0.759f;
                            newOutput[3] = 0.476f;  
                                break;
                            
                            case 31:
                            newOutput[0] = 0.353f;
                            newOutput[1] = 0.328f;
                            newOutput[2] = 0.744f;
                            newOutput[3] = 0.408f;  
                                break;

                            case 32:
                            newOutput[0] = 0.351f;
                            newOutput[1] = 0.315f;
                            newOutput[2] = 0.729f;
                            newOutput[3] = 0.344f;  
                                break;

                            case 33:
                            newOutput[0] = 0.349f;
                            newOutput[1] = 0.303f;
                            newOutput[2] = 0.713f;
                            newOutput[3] = 0.285f;  
                                break;

                            case 34:
                            newOutput[0] = 0.347f;
                            newOutput[1] = 0.291f;
                            newOutput[2] = 0.696f;
                            newOutput[3] = 0.232f;  
                                break;

                            case 35:
                            newOutput[0] = 0.346f;
                            newOutput[1] = 0.280f;
                            newOutput[2] = 0.679f;
                            newOutput[3] = 0.187f;  
                                break;

                            case 36:
                            newOutput[0] = 0.344f;
                            newOutput[1] = 0.268f;
                            newOutput[2] = 0.662f;
                            newOutput[3] = 0.148f;  
                                break;                        

                            case 37:
                            newOutput[0] = 0.342f;
                            newOutput[1] = 0.257f;
                            newOutput[2] = 0.644f;
                            newOutput[3] = 0.117f;  
                                break;

                            case 38:
                            newOutput[0] = 0.340f;
                            newOutput[1] = 0.247f;
                            newOutput[2] = 0.626f;
                            newOutput[3] = 0.091f;  
                                break;

                            case 39:
                            newOutput[0] = 0.338f;
                            newOutput[1] = 0.236f;
                            newOutput[2] = 0.607f;
                            newOutput[3] = 0.071f;  
                                break;

                            case 40:
                            newOutput[0] = 0.336f;
                            newOutput[1] = 0.226f;
                            newOutput[2] = 0.588f;
                            newOutput[3] = 0.055f;  
                                break;
                        }


                        SumError += Mathf.Abs(newOutput[0] - testOutputs[0]);
                        SumError += Mathf.Abs(newOutput[1] - testOutputs[1]);
                        SumError += Mathf.Abs(newOutput[2] - testOutputs[2]);                        
                        SumError += Mathf.Abs(newOutput[3] - testOutputs[3]);
                    }
   
                    /*
                    SumError += 0.1904f - testOutputs[0];
                    SumError += 0.589f - testOutputs[1];
                    SumError += 0.662f - testOutputs[2];
                    SumError += 0.116f - testOutputs[3];

                    testOutputs = SuprevisedNeuralNetwork.FeedForward(new float[] {0, 1, 0});
                    SumError += 0.375f - testOutputs[0];
                    SumError += 0.300f - testOutputs[1];
                    SumError += 0.759f - testOutputs[2];
                    SumError += 0.391f - testOutputs[3];
                    */

                    SuprevisedGraph.AddValueToGraph(SumError);
                }

                // float[] TestOutput1 = SuprevisedNeuralNetwork.FeedForward(new float[] {0, 0, 0} );
                // Debug.Log("Network Feed Forward 1: " + TestOutput1[0] + " , " + TestOutput1[1] + " , " + TestOutput1[2]);

                // float[] TestOutput2 = SuprevisedNeuralNetwork.FeedForward(new float[] {1, 1, 1} );
                // Debug.Log("Network Feed Forward 2: " + TestOutput2[0] + " , " + TestOutput2[1] + " , " + TestOutput2[2]);
                
                Debug.Log("Network test 1: " + SuprevisedNeuralNetwork.FeedForward(new float[] {1, 1, 0})[0]);
                Debug.Log("Network test 2: " + SuprevisedNeuralNetwork.FeedForward(new float[] {1, 1, 1})[0]);

                if(SaveParameterNetwork)
                {
                    SuprevisedNeuralNetwork.Save("Assets/SavedNetworks/" + ParameterNetworkName + ".txt");
                }

                /*
                List<Transform> targets = new List<Transform>();
                switch(currentTerrainType)
                {
                    case TerrainType.flat:
                        targets = FlatTargetList;
                        break;

                    case TerrainType.difficult:
                        targets = DifficultTargetList;
                        break;
                }

                HexapodAgent.enabled = true;
                HexapodAgent.SetupAgent(targets); 
                HexapodAgent.controller.SetInitialPosition();

                //HexapodAgent.controller.InstantiateRobot(true, targets);
                */

                break;

            case TrainingType.GeneticAlgorithm:

                if(RecordTrainingData)
                {
                    DataAnalyser.OpenFile("Training/AvrFitness");
                    DataAnalyser.OpenFile("Training/BestFitness");
                    DataAnalyser.OpenFile("Training/WorstFitness");
                }

                // Start graph
                TrainingUI.gameObject.SetActive(true);
                GenCounterUI.gameObject.SetActive(true);

                // Error checks
                if(population.Count % 2 != 0)
                {
                    Debug.Log("Population size is not divisible with two, will cause errors.");
                }
                
                if(UseGenerationCriteria == false && UseFitnessCriteria == false)
                {
                    Debug.Log("No end criteria has been specified, this will cause errors.");
                }

                if(SaveBestNetworkLastGen == true && BestNetworkLastGenName == "")
                {
                    Debug.Log("The network name for last generation is not specified, and will not saved, will cause conflicts");
                }

                if(SaveBestNetworkOverall == true && BestNetworkOverallName == "")
                {
                    Debug.Log("The network name for overall is not specified, and will not saved, will cause conflicts");
                }

                if(SaveBestNetworkLastGen == false && SaveBestNetworkOverall == false)
                {
                    Debug.Log("Be aware, no network will be saved");
                }

                // Generate population
                GeneratePopulation();

                AccumulatedReward = new float[population.Count];
                PopulationTargetsReached = new int[population.Count];
                break;
        }
    }

    private List<GenericController> ParameterRobots; 
    private void AwakeParameterHandler()
    {
        if(RecordData)
        {
            DataAnalyser.OpenFile("StableParameters_flat_Update");
            // DataAnalyser.OpenFile("StableParameters_Horizontal");
            // DataAnalyser.OpenFile("StableParameters_Speed");
            // DataAnalyser.OpenFile("StableParameters_Height");
        }

        // Choose a target list based on the robot type
        List<Transform> targets = new List<Transform>();
        switch(currentTerrainType)
        {
            case TerrainType.flat:
                targets = FlatTargetList;
                break;

            case TerrainType.difficult:
                targets = DifficultTargetList;
                break;
        }

        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:
                HexController.gameObject.SetActive(true);
                QuadController.gameObject.SetActive(false);
                HexController.SetupRobot();
                HexController.ResetRobot(false);
                HexController.Targets = targets.ToArray();
                break;

            case RobotModelToSimulate.Quadruped:
                HexController.gameObject.SetActive(false);
                QuadController.gameObject.SetActive(true);
                QuadController.SetupRobot();
                QuadController.ResetRobot(false);
                QuadController.Targets = targets.ToArray();
                break;

            case RobotModelToSimulate.Both:
                HexController.gameObject.SetActive(true);
                QuadController.gameObject.SetActive(true);

                HexController.SetupRobot();
                HexController.ResetRobot(false);
                HexController.Targets = targets.ToArray();

                QuadController.SetupRobot();
                QuadController.ResetRobot(false);
                QuadController.Targets = targets.ToArray();
                break;
        }

        currentVerticalStep = LowerVerticalStep;
        currentHorizontalStep = LowerHorizontalStep;
        currentSpeed = LowerSpeed;
        currentHeight = LowerHeight;
    }

    private void FixedUpdate()
    {
        switch(currentMode)
        {
            case SimulationMode.Demonstration:
                FixedUpdateDemonstrationHandler();
                break;

            case SimulationMode.Training:

                switch(currentTrainingType)
                {
                    case TrainingType.suprevised:

                        /*

                        float[] checkInput = new float[2] { 1, 0};
                        float[] checkOutput = SuprevisedNeuralNetwork.FeedForward(checkInput);
                        Debug.Log("Network Feed Forward: " + checkOutput[0] + " , " + checkOutput[1]);

                        checkInput = new float[2] { 0, 1};
                        checkOutput = SuprevisedNeuralNetwork.FeedForward(checkInput);
                        Debug.Log("Network Feed Forward: " + checkOutput[0] + " , " + checkOutput[1]);

                        */

                        break;
                        
                    case TrainingType.GeneticAlgorithm:
                        FixedUpdateTrainingHandler();
                        break;
                }

                break;

            case SimulationMode.Parameters:
                FixedParametersHandler();
                break;
        }    
    }

    private void FixedUpdateDemonstrationHandler()
    {
        float v = 10f;
        float h = 10f;
        float s = 0.5f;
        float he = 0;
        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:

                RobotCamera.transform.position = new Vector3(HexController.transform.position.x, 5, HexController.transform.position.z - 10);

                switch(currentControlType)
                {
                    case ControlType.StateMachine:
                        // HexController.StateBasedControl();
                        //Debug.Log("Can't do state based control");
                        HexController.StateBasedControl(v, h, s, he);
                        break;

                    case ControlType.Braintenberg:
                        // HexController.BraintenbergControl();
                        Debug.Log("Can't do braitenberg control");
                        break;

                    case ControlType.ClosedLoopControl:
                        // HexController.BetterClosedLoopControl();
                        Debug.Log("Can't do closed loop control");
                        break;

                    case ControlType.NetworkBased:
                        HexController.GenericControl();
                        break;
                }
                break;

            case RobotModelToSimulate.Quadruped:

                RobotCamera.transform.position = new Vector3(QuadController.transform.position.x, 5, QuadController.transform.position.z - 10);

                switch(currentControlType)
                {
                    case ControlType.StateMachine:
                        QuadController.StateBasedControl(v, h, s, he);
                        //Debug.Log("Can't do state based control");
                        break;

                    case ControlType.Braintenberg:
                        // QuadController.BraintenbergControl();
                        Debug.Log("Can't do braitenberg control");
                        break;

                    case ControlType.ClosedLoopControl:
                        // QuadController.BetterClosedLoopControl();
                        Debug.Log("Can't do closed loop control");
                        break;

                    case ControlType.NetworkBased:
                        QuadController.GenericControl();

                        if(RecordData) // If it is recording save data
                        {
                            RecordDataUpdate();
                        }

                        break;
                }

                break;

            case RobotModelToSimulate.Both:

                switch(currentControlType)
                {
                    case ControlType.StateMachine:
                        HexController.StateBasedControl(v, h, s, he);
                        QuadController.StateBasedControl(v, h, s, he);
                        break;

                    case ControlType.Braintenberg:
                        // HexController.BraintenbergControl();
                        // QuadController.BraintenbergControl();
                        break;

                    case ControlType.ClosedLoopControl:
                        // HexController.BetterClosedLoopControl();
                        // QuadController.BetterClosedLoopControl();

                        if(RecordData) // If it is recording save data
                        {
                            RecordDataUpdate();
                        }

                        break;

                    case ControlType.NetworkBased:
                        HexController.GenericControl();
                        QuadController.GenericControl();

                        if(RecordData) // If it is recording save data
                        {
                            RecordDataUpdate();
                        }

                        break;
                }
                break;
        }
    }

    private void FixedUpdateTrainingHandler() // The Genetic Algorithm implementation
    {
        EvaluatePopulation();

        // Count time since start and end after evaluation time finishes
        EvaluationCounter += Time.fixedDeltaTime;
        if(EvaluationCounter >= EvaluationTime)
        {
            EvaluationCounter = 0;

            EvaluateFitness();

            if(GACriteria())
            {
                Debug.Log("Finished on gen: " + currentGenerationNumber.ToString());

                if(SaveBestNetworkLastGen)
                {
                    networks[networks.Count - 1].Save("Assets/SavedNetworks/" + BestNetworkLastGenName + ".txt");
                }

                if(RecordTrainingData)
                {
                    DataAnalyser.CloseAllFiles();
                }

                EditorApplication.ExitPlaymode();
                return;
            }

            currentGenerationNumber++;
            GenCounterUI.UpdateGenerationCounterUI(currentGenerationNumber);
            ReproducePopulation();
        } 
    }

    public float LowerVerticalStep = 0;
    public float UpperVerticalStep = 45;
    public float VerticalStepDelta = 0.1f;
    public float currentVerticalStep = 0;
    
    public float LowerHorizontalStep = 0;
    public float UpperHorizontalStep = 45;
    public float HorizontalStepDelta = 0.1f;
    public float currentHorizontalStep = 0;

    public float LowerSpeed = 0.0001f;
    public float UpperSpeed = 0.1f;
    public float SpeedDelta = 0.001f;
    public float currentSpeed = 0.1f;

    public float LowerHeight = 0;
    public float UpperHeight = 45;
    public float HeightDelta = 1;
    public float currentHeight = 0;

    public float ParameterEvaluationTime = 60;
    public float ParameterTimePassed = 0;

    // private Vector3 StableRangeStart;
    // private Vector3 StableRangeEnd;

    // private bool PosibleStableRangeDetected = false;

    private void FixedParametersHandler()
    {
        ParameterTimePassed += Time.fixedDeltaTime;

        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:
                break;

            case RobotModelToSimulate.Quadruped:
                break;

            case RobotModelToSimulate.Both: // This needs more work to function
                break;
        }

        if(ParameterTimePassed > ParameterEvaluationTime || HexController.CompletedTask == true)
        {            
            if(RecordData)
            {
                DataAnalyser.FileWriteLine("StableParameters_flat_Update", currentVerticalStep);
                DataAnalyser.FileWriteLine("StableParameters_flat_Update", currentHorizontalStep);
                DataAnalyser.FileWriteLine("StableParameters_flat_Update", currentSpeed);
                // DataAnalyser.FileWriteLine("StableParameters_flat_Update", currentHeight);

                switch(currentRobotSim)
                {
                    case RobotModelToSimulate.Hexapod:
                        DataAnalyser.FileWriteLine("StableParameters_flat_Update", HexController.StabilityPercentage);
                        DataAnalyser.FileWriteLine("StableParameters_flat_Update", HexController.DebugAverageStabilityMargin);

                        if(HexController.CompletedTask == true)
                        {
                            DataAnalyser.FileWriteLine("StableParameters_flat_Update", 1);
                        }
                        else
                        {
                            DataAnalyser.FileWriteLine("StableParameters_flat_Update", 0);                           
                        }

                        break;

                    case RobotModelToSimulate.Quadruped:
                        DataAnalyser.FileWriteLine("StableParameters_flat_Update", QuadController.StabilityPercentage);
                        DataAnalyser.FileWriteLine("StableParameters_flat_Update", QuadController.DebugAverageStabilityMargin);

                        if(QuadController.CompletedTask == true)
                        {
                            DataAnalyser.FileWriteLine("StableParameters_flat_Update", 1);
                        }
                        else
                        {
                            DataAnalyser.FileWriteLine("StableParameters_flat_Update", 0);                       
                        }

                        break;

                    case RobotModelToSimulate.Both: // This needs more work to function
                        break;
                }


            }

            ParameterTimePassed = 0;
            UpdateTestParameters();
            ResetRobots(false);
        }

        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:
                HexController.StateBasedControl(currentVerticalStep, currentHorizontalStep, currentSpeed, currentHeight);
                // RobotStability = HexController.ParameterContrl(currentVerticalStep, currentHorizontalStep, currentSpeed);
                break;

            case RobotModelToSimulate.Quadruped:
                QuadController.StateBasedControl(currentVerticalStep, currentHorizontalStep, currentSpeed, currentHeight);
                // RobotStability = QuadController.ParameterContrl(currentVerticalStep, currentHorizontalStep, currentSpeed);
                break;

            case RobotModelToSimulate.Both:
                // RobotStability = HexController.ParameterContrl(currentVerticalStep, currentHorizontalStep, currentSpeed);
                // RobotStability = QuadController.ParameterContrl(currentVerticalStep, currentHorizontalStep, currentSpeed);
            break;
        }
    }

    /*************************** FUNCTIONS ***************************/

    private void UpdateTestParameters()
    {
        currentHorizontalStep += HorizontalStepDelta;

        if(currentHorizontalStep >= UpperHorizontalStep)
        {
            currentVerticalStep += VerticalStepDelta;
            currentHorizontalStep = LowerHorizontalStep;

            if(currentVerticalStep > UpperVerticalStep)
            {
                currentSpeed += SpeedDelta;
                currentVerticalStep = LowerVerticalStep;

                if(currentSpeed > UpperSpeed)
                {
                    if(RecordData)
                    {
                        DataAnalyser.CloseAllFiles();
                    }

                    EditorApplication.ExitPlaymode(); // End demonstration if final target has been reached.
                }
            }
        }
    }

    private void GeneratePopulation()
    {
        // Choose a target list based on the robot type
        List<Transform> targets = new List<Transform>();
        switch(currentTerrainType)
        {
            case TerrainType.flat:
                targets = FlatTargetList;
                break;

            case TerrainType.difficult:
                targets = DifficultTargetList;
                break;
        }

        switch(currentRobotTrain)
        {
            case RobotModelToTrain.Hexapod:
                HexController.gameObject.SetActive(true);
                QuadController.gameObject.SetActive(false);

                HexController.SetupRobot();
                HexController.ResetRobot(RandomInstanstiationRotation);
                HexController.Targets = targets.ToArray();

                population = new List<GenericController>();
                population.Add(HexController);

                for(int i = 0; i < populationSize - 1; i++)
                {
                    GenericController spawnedRobot = Instantiate(HexController.gameObject, HexController.transform.position, Quaternion.identity, this.transform).GetComponent<GenericController>();

                    spawnedRobot.tag = "Agent" + i.ToString();
                    LowLevelRobotController SpawnedLowController = spawnedRobot.GetComponent<LowLevelRobotController>();
                    for(int j = 0; j < SpawnedLowController.UpperLegs.Length; j++)
                    {
                        SpawnedLowController.UpperLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController.UnderLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController.foots[j].tag = "Agent" + i.ToString();
                    }

                    population.Add(spawnedRobot);
                    population[population.Count - 1].SetupRobot();
                    // I think it might be missing targets??
                    Debug.Log("Position: x = " + population[population.Count - 1].transform.position.x + ", y = " + population[population.Count - 1].transform.position.y + ", z = " + population[population.Count - 1].transform.position.z);
                }

                break;

            case RobotModelToTrain.Quadruped:
                HexController.gameObject.SetActive(false);
                QuadController.gameObject.SetActive(true);
 
                QuadController.SetupRobot();
                QuadController.ResetRobot(RandomInstanstiationRotation);
                QuadController.Targets = targets.ToArray();                

                population = new List<GenericController>();
                population.Add(QuadController);

                for(int i = 0; i < populationSize - 1; i++)
                {
                    GenericController spawnedRobot = Instantiate(QuadController.gameObject, HexController.transform.position, Quaternion.identity, this.transform).GetComponent<GenericController>();

                    spawnedRobot.tag = "Agent" + i.ToString();
                    LowLevelRobotController SpawnedLowController = spawnedRobot.GetComponent<LowLevelRobotController>();
                    for(int j = 0; j < SpawnedLowController.UpperLegs.Length; j++)
                    {
                        SpawnedLowController.UpperLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController.UnderLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController.foots[j].tag = "Agent" + i.ToString();
                    }

                    population.Add(spawnedRobot);
                    population[population.Count - 1].SetupRobot();

                    // population[population.Count - 1].ResetRobot(RandomInstanstiationRotation);
                    // population[population.Count - 1].Targets = targets.ToArray();
                }

                break;

            case RobotModelToTrain.Both:
                HexController.gameObject.SetActive(true);
                QuadController.gameObject.SetActive(true);

                HexController.SetupRobot();
                HexController.ResetRobot(RandomInstanstiationRotation);
                HexController.Targets = targets.ToArray();

                QuadController.SetupRobot();
                QuadController.ResetRobot(RandomInstanstiationRotation);
                QuadController.Targets = targets.ToArray();        

                population = new List<GenericController>();
                population.Add(HexController);
                population.Add(QuadController);

                for(int i = 0; i < populationSize - 2; i += 2)
                {
                    GenericController spawnedRobot1 = Instantiate(HexController.gameObject, HexController.transform.position, Quaternion.identity, this.transform).GetComponent<GenericController>();
                    GenericController spawnedRobot2 = Instantiate(QuadController.gameObject, HexController.transform.position, Quaternion.identity, this.transform).GetComponent<GenericController>();

                    spawnedRobot1.tag = "Agent" + i.ToString();
                    LowLevelRobotController SpawnedLowController = spawnedRobot1.GetComponent<LowLevelRobotController>();
                    for(int j = 0; j < SpawnedLowController.UpperLegs.Length; j++)
                    {
                        SpawnedLowController.UpperLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController.UnderLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController.foots[j].tag = "Agent" + i.ToString();
                    }

                    spawnedRobot2.tag = "Agent" + (i + 1).ToString();
                    LowLevelRobotController SpawnedLowController2 = spawnedRobot2.GetComponent<LowLevelRobotController>();
                    for(int j = 0; j < SpawnedLowController2.UpperLegs.Length; j++)
                    {
                        SpawnedLowController2.UpperLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController2.UnderLegs[j].tag = "Agent" + i.ToString();
                        SpawnedLowController2.foots[j].tag = "Agent" + i.ToString();
                    }

                    population.Add(spawnedRobot1);
                    population.Add(spawnedRobot2);

                    population[population.Count - 1].SetupRobot();
                    population[population.Count - 2].SetupRobot();

                    // population[population.Count - 1].ResetRobot(RandomInstanstiationRotation);
                    // population[population.Count - 1].Targets = targets.ToArray();
                    // population[population.Count - 2].ResetRobot(RandomInstanstiationRotation);
                    // population[population.Count - 2].Targets = targets.ToArray();
                }

                break;
        }

        // Assign neural networks to the robots
        networks = new List<NeuralNetwork>();
        if(LoadNetwork) // If you wanna continue training on a previous network
        {
            for(int i = 0; i < population.Count; i++)
            {
                NeuralNetwork newNetwork = new NeuralNetwork(TrainingNetworkTopology);
                newNetwork.Load("Assets/SavedNetworks/" + LoadedNetworkName + ".txt");

                networks.Add(newNetwork);
                population[i].network = networks[i];
            }
        }
        else // Using a new network
        {
            for(int i = 0; i < population.Count; i++)
            {
                NeuralNetwork newNetwork = new NeuralNetwork(TrainingNetworkTopology);
                networks.Add(newNetwork);
                population[i].network = networks[i];
            }
        }
    }

    public void TestSuprevisedNetwork()
    {
        float[] TestOutput;
        TestOutput = SuprevisedNeuralNetwork.FeedForward(TestInput);
        Debug.Log("Network Feed Forward: " + TestOutput[0] + " , " + TestOutput[1] + " , " + TestOutput[2]);
        // Debug.Log("Network Feed Forward: " + TestOutput[0]);
    }


    private float[] AccumulatedReward;
    private int[] PopulationTargetsReached;

    private void EvaluatePopulation()
    {
        for(int i = 0; i < population.Count; i++)
        {
            population[i].GenericControl();
        }

        for(int i = 0; i < population.Count; i++)
        {
            if(population[i].TargetsReachedCounter != population[i].Targets.Length)
            {
                AccumulatedReward[i] -= 0.02f;
            }

            if(PopulationTargetsReached[i] < population[i].TargetsReachedCounter)
            {
                PopulationTargetsReached[i] = population[i].TargetsReachedCounter;
                AccumulatedReward[i] += population[i].StabilityPercentage * 100;
            }

            // Remove this line if needed
            /*
            if(population[i].TargetsReachedCounter >= 4)
            {
                population[i].TriggerDamage();
            }
            */
        }
    }

    private void EvaluateFitness()
    {
        for(int i = 0; i < population.Count; i++)
        {
            // Update fitness using the Generic agents Fitness function
            FitnessFunction(i);
        }
        networks.Sort();

        // Debugging fitness (Optimize please...)
        DebugAllFitness = new List<float>();
        float DebugFitnessSum = 0;
        for(int i = 0; i < population.Count; i++)
        {
            DebugAllFitness.Add(networks[i].copyWithFitness(new NeuralNetwork(TrainingNetworkTopology)).fitness);
            DebugFitnessSum += networks[i].copyWithFitness(new NeuralNetwork(TrainingNetworkTopology)).fitness;
        }
        DebugAverageFitness = DebugFitnessSum / population.Count;
        TrainingUI.AddValueToGraph(DebugAverageFitness);
        TrainingUI.AddSecondValueToGraph(networks[networks.Count - 1].fitness);
        TrainingUI.AddThirdValueToGraph(networks[0].fitness);

        if(RecordTrainingData)
        {
            DataAnalyser.FileWriteLine("Training/AvrFitness", DebugAverageFitness);
            DataAnalyser.FileWriteLine("Training/BestFitness", networks[networks.Count - 1].fitness);
            DataAnalyser.FileWriteLine("Training/WorstFitness", networks[0].fitness);
        }

        // BE AWARE HERE! - Is dependent on fitness function.
        float checkFitness = networks[networks.Count - 1].copyWithFitness(new NeuralNetwork(TrainingNetworkTopology)).fitness;
        if(checkFitness >= DebugBestFitnessOverall) // Be aware this will need to change dependent on fitness function!
        {
            DebugBestFitnessOverall = checkFitness;

            if(SaveBestNetworkOverall)
            {
                networks[networks.Count - 1].Save("Assets/SavedNetworks/" + BestNetworkOverallName + ".txt");
            }
        }
    }


    private void FitnessFunction(int PopulationIndex)
    {
        // Vector3 CoMWorldPosition = Vector3.zero;

        // float distanceBetweenTargetAndRobot = Vector3.Distance(population[PopulationIndex].transform.position, population[PopulationIndex].Targets[population[PopulationIndex].TargetsReachedCounter].position);
        // float reward = 0;

        // reward = 50 * population[PopulationIndex].TargetsReachedCounter - distanceBetweenTargetAndRobot;


        // This is the original code
        /*
        float reward = 0;
        if(population[PopulationIndex].TargetsReachedCounter >= population[PopulationIndex].Targets.Length - 1)
        {
            reward = population[PopulationIndex].StabilityPercentage;
            // reward = population[PopulationIndex].DebugAverageStabilityMargin;
        }
        else if(population[PopulationIndex].TargetsReachedCounter >= 0)
        {
            reward = (0.5f / population[PopulationIndex].Targets.Length) * population[PopulationIndex].TargetsReachedCounter;
        }
        else
        {
            reward = 0;
        }
        */

        population[PopulationIndex].network.fitness = AccumulatedReward[PopulationIndex];
        population[PopulationIndex].DebugFitness = AccumulatedReward[PopulationIndex];

        AccumulatedReward[PopulationIndex] = 0;
        PopulationTargetsReached[PopulationIndex] = 0;

        // reward += population[PopulationIndex].TargetsReachedCounter * 10;

        // this is the original code
        // population[PopulationIndex].network.fitness = reward;
        // population[PopulationIndex].DebugFitness = reward;
    }

    private void ReproducePopulation()
    {
        // This will copy the lower half to the top half, change with different fitness function.
        for(int i = 0; i < population.Count / 2; i++) 
        {
            networks[i] = networks[i  + population.Count / 2].copy(new NeuralNetwork(TrainingNetworkTopology)); // Replace the on bad half with the good half
            
            // Keep a copy of the 25% best performer unmutated. 
            if(i < population.Count - Mathf.Floor(population.Count / 4) - 1)
            {
                networks[i].Mutate((int)(1 / MutationChance), MutationEffect);
            }
        }

        // Assign the new brains
        for (int i = 0; i < population.Count; i++)
        {
            population[i].network = networks[i];
        }

        // Reset environment
        for(int i = 0; i < population.Count; i++)
        {
            population[i].ResetRobot(RandomInstanstiationRotation); // Missing Add randomized spawning conditions
        }
    }

    private bool GACriteria()
    {
        // Missing - Performance criteria!
        if(currentGenerationNumber >= GenerationCriteria)
        {
            return true;
        }
        return false;
    }

    public void ChangeTerrain()
    {
        switch(currentTerrainType)
        {
            case TerrainType.flat:
                FlatTerrain.SetActive(true);
                DifficultTerrain.SetActive(false);

                for(int i = 0; i < FlatTargetList.Count; i++)
                {
                    FlatTargetList[i].gameObject.SetActive(true);
                }

                for(int i = 0; i < DifficultTargetList.Count; i++)
                {
                    DifficultTargetList[i].gameObject.SetActive(false);
                }

                break;

            case TerrainType.difficult:
                FlatTerrain.SetActive(false);
                DifficultTerrain.SetActive(true);

                for(int i = 0; i < FlatTargetList.Count; i++)
                {
                    FlatTargetList[i].gameObject.SetActive(false);
                }

                for(int i = 0; i < DifficultTargetList.Count; i++)
                {
                    DifficultTargetList[i].gameObject.SetActive(true);
                }

                break;
        }
    }

    public void BeginRecording()
    {
        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:
                DataAnalyser.OpenFile("state_Hex");
                DataAnalyser.OpenFile("Horizontal_Step_Hex");
                DataAnalyser.OpenFile("Vertical_Step_Hex");
                DataAnalyser.OpenFile("Speed_Hex");
                DataAnalyser.OpenFile("TargetsReachedHex");
                break;

            case RobotModelToSimulate.Quadruped:
                DataAnalyser.OpenFile("state_Quad");
                DataAnalyser.OpenFile("Horizontal_Step_Quad");
                DataAnalyser.OpenFile("Vertical_Step_Quad");
                DataAnalyser.OpenFile("Speed_Quad");
                DataAnalyser.OpenFile("TargetsReachedQuad");
                break;

            case RobotModelToSimulate.Both:
                DataAnalyser.OpenFile("state_Hex");
                DataAnalyser.OpenFile("Horizontal_Step_Hex");
                DataAnalyser.OpenFile("Vertical_Step_Hex");
                DataAnalyser.OpenFile("Speed_Hex");                
                DataAnalyser.OpenFile("TargetsReachedHex");

                DataAnalyser.OpenFile("state_Quad");
                DataAnalyser.OpenFile("Horizontal_Step_Quad");
                DataAnalyser.OpenFile("Vertical_Step_Quad");
                DataAnalyser.OpenFile("Speed_Quad");
                DataAnalyser.OpenFile("TargetsReachedQuad");

                DataAnalyser.OpenFile("Generic_Hexapod");
                DataAnalyser.OpenFile("Generic_Quadruped");
                break;
        }
    }

    public void RecordDataUpdate()
    {
        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:

                /*
                DataAnalyser.FileWriteLine("state_Hex", HexController.output[0]);
                DataAnalyser.FileWriteLine("Vertical_Step_Hex", HexController.output[1]);
                DataAnalyser.FileWriteLine("Horizontal_Step_Hex", HexController.output[2]);
                DataAnalyser.FileWriteLine("Speed_Hex", HexController.output[3]);
                DataAnalyser.FileWriteLine("TargetsReachedHex", HexController.TargetsReachedCounter);
                */

                break;

            case RobotModelToSimulate.Quadruped:

                /*
                DataAnalyser.FileWriteLine("state_Quad", QuadController.output[0]);
                DataAnalyser.FileWriteLine("Vertical_Step_Quad", QuadController.output[1]);
                DataAnalyser.FileWriteLine("Horizontal_Step_Quad", QuadController.output[2]);
                DataAnalyser.FileWriteLine("Speed_Quad", QuadController.output[3]);
                DataAnalyser.FileWriteLine("TargetsReachedQuad", QuadController.TargetsReachedCounter);
                */

                break;

            case RobotModelToSimulate.Both:
                /*
                DataAnalyser.FileWriteLine("state_Hex", HexController.output[0]);
                DataAnalyser.FileWriteLine("Vertical_Step_Hex", HexController.output[1]);
                DataAnalyser.FileWriteLine("Horizontal_Step_Hex", HexController.output[2]);
                DataAnalyser.FileWriteLine("Speed_Hex", HexController.output[3]);
                DataAnalyser.FileWriteLine("TargetsReachedHex", HexController.TargetsReachedCounter);

                DataAnalyser.FileWriteLine("state_Quad", QuadController.output[0]);
                DataAnalyser.FileWriteLine("Vertical_Step_Quad", QuadController.output[1]);
                DataAnalyser.FileWriteLine("Horizontal_Step_Quad", QuadController.output[2]);
                DataAnalyser.FileWriteLine("Speed_Quad", QuadController.output[3]);
                DataAnalyser.FileWriteLine("TargetsReachedQuad", QuadController.TargetsReachedCounter);
                */

                /*
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.input[0]); 
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.input[1]); 
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.input[2]); 

                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.output[0]); // vstep
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.output[1]); // hstep
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.output[2]); // speed
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.output[3]); // Underleg
                DataAnalyser.FileWriteLine("Generic_Hexapod", HexController.TargetsReachedCounter); // Underleg

                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.input[0]);
                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.input[1]);
                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.input[2]);

                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.output[0]);
                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.output[1]);
                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.output[2]);
                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.output[3]);
                */

                DataAnalyser.FileWriteLine("Generic_Quadruped", QuadController.TargetsReachedCounter); // Underleg
                break;
        }
    }

    public void EndRecording()
    {
        DataAnalyser.CloseAllFiles();
        RecordData = false;
    }

    public void ResetRobots(bool RandomSpawnOrientation)
    {
        switch(currentRobotSim)
        {
            case RobotModelToSimulate.Hexapod:
                HexController.ResetRobot(RandomSpawnOrientation);
                break;

            case RobotModelToSimulate.Quadruped:
                QuadController.ResetRobot(RandomSpawnOrientation);
                break;

            case RobotModelToSimulate.Both:
                HexController.ResetRobot(RandomSpawnOrientation);
                QuadController.ResetRobot(RandomSpawnOrientation);
                break;
        }
    }

    public void UpdateCurrentTimeScale()
    {
        currentTimeScale = Time.timeScale;
    }

    public void SetTimeScale()
    {
        Time.timeScale = desiredTimeScale;
    }
}
