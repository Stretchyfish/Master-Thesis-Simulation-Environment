using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class GenericAgent : Agent
{
    /********** PUBLIC VARIABLES *******************/
    public LowLevelRobotController controller;
    public float UpdateFrequency = 10;
    
    [SerializeField]
    public Transform[] Targets;

    public float DebugStepness;
    public float DebugMorphology;
    public float[] input;
    public float[] output;
    public float[] lastOutput;

    [SerializeField]
    public Vector3[] FootEstimates;

    public int MaximumSimulationTime = 0;

    public int CurrentSimulationTime = 0;
    public NeuralNetwork network;

    /********** PRIVATE VARIABLES *****************/
    private float FrequencyCounter = 0;
    private int TargetsReachedCounter = 0;
    private Vector3 InitialPosition;
    private Quaternion InitialOrientation;

    private float direction;
    private float terrainSteepness;
    private float morphology;

    private void Awake()
    {
        InitialPosition = this.transform.position;
        InitialOrientation = this.transform.rotation;
        lastOutput = new float[4]{0, 0, 0, 0};

        network = new NeuralNetwork(new int[4]{2, 10, 10, 4});
        network.Load("Assets/SavedNetworks/" + "ExperimentationNetwork" + ".txt");
    }

    private void FixedUpdate() // Main loop
    {
        // Update simulation time
        if(MaximumSimulationTime > 0)
        {
            CurrentSimulationTime++;

            if(CurrentSimulationTime > MaximumSimulationTime)
            {
                AddReward(-Vector3.Distance(this.transform.position, Targets[TargetsReachedCounter].position));
                EndEpisode();
            }
        }

        // Check if target is reached
        float distanceToTarget = Vector3.Distance(this.transform.position, Targets[TargetsReachedCounter].position);
        if(distanceToTarget < 1)
        {
            AddReward(50);
            if(TargetsReachedCounter >= Targets.Length - 1)
            {
                return;
            }
            TargetsReachedCounter++;
        }

        // Controller
        if(FrequencyCounter > 1/UpdateFrequency)
        {
            FrequencyCounter = 0;

            // Find Heading Vector to target
            Vector3 VectorToTarget = Targets[TargetsReachedCounter].position - this.transform.position;
            Vector3 HeadingVector = VectorToTarget.normalized;

            // Direction Interpretation
            float AngleToTarget = Vector3.SignedAngle(this.transform.forward, HeadingVector, Vector3.up);
            float squezedAngleToTarget = ValueRangeMapping(AngleToTarget, -180, 180, -1, 1);

            direction = 1;
            if(squezedAngleToTarget >= -0.15f && squezedAngleToTarget <= 0.15f)
            {
                direction = 1;
            }
            else if(squezedAngleToTarget < -0.15f)
            {
                direction = 2;
            }
            else if(squezedAngleToTarget > 0.15f)
            {
                direction = 3;
            }

            // Terrain Difficulty Estimation
            Vector3[] FullLegRelativePositions = new Vector3[6] { new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1) };
            float[] FullLegCollisionStatus = new float[6] { -1, -1, -1, -1, -1, -1 };

            /*
            FullLegRelativePositions[0] = controller.SimpleLegKinematics(0);
            FullLegRelativePositions[1] = controller.SimpleLegKinematics(1);
            FullLegRelativePositions[2] = controller.SimpleLegKinematics(2);
            FullLegRelativePositions[3] = controller.SimpleLegKinematics(3);
            FullLegRelativePositions[4] = controller.SimpleLegKinematics(4);
            FullLegRelativePositions[5] = controller.SimpleLegKinematics(5);
            */

            FullLegCollisionStatus[0] = controller.IndividualFootStatus(0);
            FullLegCollisionStatus[1] = controller.IndividualFootStatus(1);
            FullLegCollisionStatus[2] = controller.IndividualFootStatus(2);
            FullLegCollisionStatus[3] = controller.IndividualFootStatus(3);
            FullLegCollisionStatus[4] = controller.IndividualFootStatus(4);
            FullLegCollisionStatus[5] = controller.IndividualFootStatus(5);

            FootEstimates = FullLegRelativePositions;

            terrainSteepness = TerrainDifficultyDeterminer(FullLegRelativePositions, FullLegCollisionStatus);
            DebugStepness = terrainSteepness;

            morphology = MorphologyIdentifier(FullLegCollisionStatus);
            DebugMorphology = morphology;

            /******************* Insert neural network here *******************/
            // input = new float[3]{direction, terrainSteepness, morphology};
            input = new float[2]{terrainSteepness, morphology};
            float[] newOutputs = network.FeedForward(input);
            
            newOutputs[0] = ValueRangeMapping(Mathf.Clamp(newOutputs[0], 0, 1), -1, 1, 5, 40);
            newOutputs[1] = ValueRangeMapping(Mathf.Clamp(newOutputs[1], 0, 1), -1, 1, 5, 40);
            newOutputs[2] = ValueRangeMapping(Mathf.Clamp(newOutputs[2], 0, 1), -1, 1, 0.01f, 0.09f);
            newOutputs[3] = ValueRangeMapping(Mathf.Clamp(newOutputs[3], 0, 1), -1, 1, 5, 40);

            GaitBank((int)morphology, (int)direction, newOutputs[0], newOutputs[1], newOutputs[2], newOutputs[3]);  
            /*****************************************************************/
        }

        FrequencyCounter += Time.fixedDeltaTime; // Must be in the bottom
    }

    public override void OnEpisodeBegin()
    {
        controller.ResetRobotPositionAndOrientation(InitialPosition, InitialOrientation, false);
        CurrentSimulationTime = 0;
        TargetsReachedCounter = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(ValueRangeMapping(input[0], 1, 3, -1, 1));
        sensor.AddObservation(ValueRangeMapping(input[1], 0, 20, -1, 1));
        sensor.AddObservation(ValueRangeMapping(input[2], 0, 1, -1, 1));        
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        output = new float[4];

        output[0] = ValueRangeMapping(Mathf.Clamp(vectorAction[0], -1, 1), -1, 1, 5, 40);
        output[1] = ValueRangeMapping(Mathf.Clamp(vectorAction[1], -1, 1), -1, 1, 5, 40);
        output[2] = ValueRangeMapping(Mathf.Clamp(vectorAction[2], -1, 1), -1, 1, 0.1f, 0.9f);
        output[3] = ValueRangeMapping(Mathf.Clamp(vectorAction[3], -1, 1), -1, 1, 5, 40);

        //GaitBank((int)morphology, (int)direction, output[0], output[1], output[2], output[3]);  
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0.25f;
        actionsOut[1] = 0.5f;
        actionsOut[2] = 0.5f;
        actionsOut[3] = 0.5f;
    }

    private float LastTerrainDifficultyObserved = 0;
    private float TerrainDifficultyDeterminer(Vector3[] LegPositions, float[] LegCollsions)
    {
        float highestAngleObserved = 0;
        int NumberOfLegsOnGround = 0;
        for(int i = 0; i < LegCollsions.Length; i++)
        {
            if(LegCollsions[i] == 1) // If colliding check its
            {
                NumberOfLegsOnGround++;
                for(int j = 0; j < LegCollsions.Length; j++)
                {
                    if(j != i && LegCollsions[j] == 1)
                    {
                        float angleBetweenPoints = Mathf.Atan((LegPositions[j].y - LegPositions[i].y) / (LegPositions[j].z - LegPositions[i].z)) * Mathf.Rad2Deg;
                        // Inpiration: https://stackoverflow.com/questions/22200453/calculate-angle-between-horizontal-axis-and-two-points

                        if(angleBetweenPoints > highestAngleObserved)
                        {
                            highestAngleObserved = angleBetweenPoints;
                        }
                    }
                }
            }
        }

        if(NumberOfLegsOnGround < 2)
        {
            return LastTerrainDifficultyObserved;
        }

        LastTerrainDifficultyObserved = highestAngleObserved;
        return highestAngleObserved;
    }

    private float MorphologyIdentifier(float[] LegCollisions)
    {
        if(LegCollisions[4] == -1 && LegCollisions[5] == -1)
        {
            return 0; // Quadruped
        }
        return 1; // Hexapod        
    }

    /************************* GAIT CONTROL RELATED MEMBERS ************************************/
    private void GaitBank(int morphology, int direction, float VerticalStepSize, float HorizontalStepSize, float speed, float height)
    {
        if(morphology == 0)
        {
            QuadrupedTrotGait(direction, VerticalStepSize, HorizontalStepSize, speed, height);
        }

        if(morphology == 1)
        {
            HexapodTripodGait(direction, VerticalStepSize, HorizontalStepSize, speed, height);
        }

        if(morphology > 1) // Exception catch
        {
            Debug.Log("Tried to use a Morphology in the Gait Bank that does not exist.");
            return;
        }
    }

    private enum WalkCycle
    {
        Standing,
        StraitWalking,
        leftTurning,
        rightTurning,
        StanceRecovery
    }
    private WalkCycle currentWalkCycle = WalkCycle.StraitWalking;

    private float timeStep1 = 0;
    private float timeStep2 = 0;

    private void HexapodTripodGait(int direction, float VerticalStepSize, float HorizontalStepSize, float speed, float height)
    {
        float Group1UpperLegAxis1 = VerticalStepSize * Mathf.Sqrt((1 + Mathf.Pow(11, 2)) / (1 + Mathf.Pow(11, 2) * Mathf.Pow(Mathf.Cos(2 * Mathf.PI * timeStep1), 2))) * Mathf.Cos(2 * Mathf.PI * timeStep1);
        float Group1UpperLegAxis2 = HorizontalStepSize * Mathf.Sin(2 * Mathf.PI * timeStep1);
        float Group1UnderLegAxis1 = height;

        float Group2UpperLegAxis1 = VerticalStepSize * Mathf.Sqrt((1 + Mathf.Pow(11, 2)) / (1 + Mathf.Pow(11, 2) * Mathf.Pow(Mathf.Cos(2 * Mathf.PI * timeStep2 + Mathf.PI), 2))) * Mathf.Cos(2 * Mathf.PI * timeStep2 + Mathf.PI);
        float Group2UpperLegAxis2 = HorizontalStepSize * Mathf.Sin(2 * Mathf.PI * timeStep2 + Mathf.PI);
        float Group2UnderLegAxis1 = height;

        float legDir1, legDir2, legDir3, legDir4, legDir5, legDir6;
        legDir1 = legDir2 = legDir3 = legDir4 = legDir5 = legDir6 = 1f;

        timeStep1 += speed;
        timeStep2 += speed;

        switch (currentWalkCycle)
        {
            case WalkCycle.Standing:

                timeStep1 -= speed;
                timeStep2 -= speed;

                if(direction != 0)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;


            case WalkCycle.StraitWalking:

                if (direction != 1)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;

            case WalkCycle.StanceRecovery:

                if (timeStep1 >= 1 && direction == 0)
                {
                    currentWalkCycle = WalkCycle.Standing;
                    break;
                }

                if (timeStep1 >= 1 && direction == 1)
                {
                    currentWalkCycle = WalkCycle.StraitWalking;
                    break;
                }

                if (timeStep1 >= 1 && direction == 2)
                {
                    currentWalkCycle = WalkCycle.leftTurning;
                    break;
                }

                if (timeStep1 >= 1 && direction == 3)
                {
                    currentWalkCycle = WalkCycle.rightTurning;
                    break;
                }

                break;

            case WalkCycle.leftTurning:

                legDir1 = -1;
                legDir2 = -1;
                legDir3 = -1;

                if (direction != 2)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;

            case WalkCycle.rightTurning:

                legDir4 = -1;
                legDir5 = -1;
                legDir6 = -1;

                if (direction != 3)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;

            default:
                break;
        }

        controller.SetLegConfiguration(0, Group1UpperLegAxis1, -legDir1 * Group1UpperLegAxis2, Group1UnderLegAxis1);
        controller.SetLegConfiguration(2, Group1UpperLegAxis1, -legDir3 * Group1UpperLegAxis2, Group1UnderLegAxis1);
        controller.SetLegConfiguration(4, Group1UpperLegAxis1, legDir5 * Group1UpperLegAxis2, Group1UnderLegAxis1);

        controller.SetLegConfiguration(1, Group2UpperLegAxis1, -legDir2 * Group2UpperLegAxis2, Group2UnderLegAxis1);
        controller.SetLegConfiguration(3, Group2UpperLegAxis1, legDir4 * Group2UpperLegAxis2, Group2UnderLegAxis1);
        controller.SetLegConfiguration(5, Group2UpperLegAxis1, legDir6 * Group2UpperLegAxis2, Group2UnderLegAxis1);

        if (timeStep1 >= 1)
        {
            timeStep1 = 0;
        }

        if (timeStep2 >= 1)
        {
            timeStep2 = 0;
        }
    }

    private void QuadrupedTrotGait(int direction, float VerticalStepSize, float HorizontalStepSize, float speed, float height)
    {
        float Group1UpperLegAxis1 = VerticalStepSize * Mathf.Sqrt((1 + Mathf.Pow(11, 2)) / (1 + Mathf.Pow(11, 2) * Mathf.Pow(Mathf.Cos(2 * Mathf.PI * timeStep1), 2))) * Mathf.Cos(2 * Mathf.PI * timeStep1);
        float Group1UpperLegAxis2 = HorizontalStepSize * Mathf.Sin(2 * Mathf.PI * timeStep1);
        float Group1UnderLegAxis1 = height;

        float Group2UpperLegAxis1 = VerticalStepSize * Mathf.Sqrt((1 + Mathf.Pow(11, 2)) / (1 + Mathf.Pow(11, 2) * Mathf.Pow(Mathf.Cos(2 * Mathf.PI * timeStep2 + Mathf.PI), 2))) * Mathf.Cos(2 * Mathf.PI * timeStep2 + Mathf.PI);
        float Group2UpperLegAxis2 = HorizontalStepSize * Mathf.Sin(2 * Mathf.PI * timeStep2 + Mathf.PI);
        float Group2UnderLegAxis1 = height;

        float legDir1, legDir2, legDir3, legDir4;
        legDir1 = legDir2 = legDir3 = legDir4 = 1f;

        timeStep1 += speed;
        timeStep2 += speed;

        switch (currentWalkCycle)
        {
            case WalkCycle.Standing:

                timeStep1 -= speed;
                timeStep2 -= speed;

                if(direction != 0)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;


            case WalkCycle.StraitWalking:

                if (direction != 1)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;

            case WalkCycle.StanceRecovery:

                if (timeStep1 >= 1 && direction == 0)
                {
                    currentWalkCycle = WalkCycle.Standing;
                    break;
                }

                if (timeStep1 >= 1 && direction == 1)
                {
                    currentWalkCycle = WalkCycle.StraitWalking;
                    break;
                }

                if (timeStep1 >= 1 && direction == 2)
                {
                    currentWalkCycle = WalkCycle.leftTurning;
                    break;
                }

                if (timeStep1 >= 1 && direction == 3)
                {
                    currentWalkCycle = WalkCycle.rightTurning;
                    break;
                }

                break;

            case WalkCycle.leftTurning:

                legDir1 = -1;
                legDir2 = -1;

                if (direction != 2)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;

            case WalkCycle.rightTurning:

                legDir3 = -1;
                legDir4 = -1;

                if (direction != 3)
                {
                    currentWalkCycle = WalkCycle.StanceRecovery;
                }

                break;

            default:
                Debug.Log("Used an impossible direction in gait bank.");
                break;
        }
    
        controller.SetLegConfiguration(0, Group1UpperLegAxis1, -legDir1 * Group1UpperLegAxis2, Group1UnderLegAxis1);
        controller.SetLegConfiguration(2, Group1UpperLegAxis1, legDir3 * Group1UpperLegAxis2, Group1UnderLegAxis1);

        controller.SetLegConfiguration(1, Group2UpperLegAxis1, -legDir2 * Group2UpperLegAxis2, Group2UnderLegAxis1);
        controller.SetLegConfiguration(3, Group2UpperLegAxis1, legDir4 * Group2UpperLegAxis2, Group2UnderLegAxis1);

        if (timeStep1 >= 1)
        {
            timeStep1 = 0;
        }

        if (timeStep2 >= 1)
        {
            timeStep2 = 0;
        }
    }


    /************************* EXTRA FUNCTIONALITY ************************************/
    private float ValueRangeMapping(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (value - oldMin) * (newMax - newMin) / (oldMax - oldMin); // Inspired from https://prime31.github.io/simple-value-mapping/
    }
}
