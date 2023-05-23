using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class GenericController : MonoBehaviour
{
    /********** PUBLIC VARIABLES *******************/
    public LowLevelRobotController controller;
    public float UpdateFrequency = 10;
    
    [SerializeField]
    public Transform[] Targets;

    public float DebugStepness;
    public float DebugMorphology;
    public float[] DebugInput;
    public float[] DebugOutput;
    public float DebugStaticStability;
    public float DebugDynamicStability;
    public bool DebugIsStable;
    public float DebugFitness;

    public float DistanceMoved = 0;

    private Vector3 lastPosition;

    [SerializeField]
    public Vector3[] FootEstimates;
    public NeuralNetwork network;

    /********** PRIVATE VARIABLES *****************/
    private float FrequencyCounter = 1; // To make sure it makes a decission on the first step, otherwise it won't have any information to make decisions from.

    public int TargetsReachedCounter = 0;
    private Vector3 InitialPosition;
    private Quaternion InitialOrientation;

    private float direction;
    private float terrainSteepness;
    private float morphology;

    public float DebugComulativeStability = 0;
    public float DebugComulativeDynamicStability = 0;
    public float StabilityPercentage = 1;

    public float DebugAverageDynamicStabilityMargin = 0;

    public void SetupRobot()
    {
        InitialPosition = this.transform.position;
        InitialOrientation = this.transform.rotation;
    }

    public void ResetRobot(bool RandomOrientation)
    {
        if(InitialPosition == null)
        {
            Debug.Log("Here!");
        }

        controller.ResetRobotPositionAndOrientation(InitialPosition, InitialOrientation, RandomOrientation);
        TargetsReachedCounter = 0;
        NumberOfStableMeassurements = 0;
        NumberOfStabilityMeasurements = 0;
        DebugComulativeStability = 0;
        DebugAverageStabilityMargin = 0;
        DebugAverageDynamicStabilityMargin = 0;
        CompletedTask = false;
        LowestDynamicMarginObserved = Mathf.Infinity;
        DistanceMoved = 0;
        lastPosition = this.transform.position;
        TriggeredOnce = false;

        if(controller.DamageHasHappend == true)
        {
            controller.DamageUnHappened();
        }
    }

    public bool DebugPCoMIsCorrect = false;

    public bool CompletedTask = false;

    public float NumberOfStableMeassurements;
    public float NumberOfStabilityMeasurements = 0;
    public float DebugAverageStabilityMargin = 0;

    public float LowestDynamicMarginObserved = Mathf.Infinity;

    public bool StartSequenceGoing = true;
    private float StartTimePassed = 0;

    public void StateBasedControl(float VerticalStepSize, float HorizontalStepSize, float LegFrequency, float Height)
    {
        // Check if target is reached
        float distanceToTarget = Vector3.Distance(this.transform.position, Targets[TargetsReachedCounter].position);
        if(distanceToTarget < 1)
        {
            if(TargetsReachedCounter >= Targets.Length - 1)
            {
                // Debug.Log("Reached the end.");
                CompletedTask = true;
                // return;
            }
            else
            {
                TargetsReachedCounter++;
            }
        }

        // Controller
        if(FrequencyCounter > 1/UpdateFrequency)
        {
            FrequencyCounter = 0;

            if(StartSequenceGoing == true) // Start sequence to determine morphology.
            {
                StartTimePassed += 1/UpdateFrequency;

                float[] FullLegCollisionStatusStart = new float[6] { -1, -1, -1, -1, -1, -1 };
                FullLegCollisionStatusStart[0] = controller.IndividualFootStatus(0);
                FullLegCollisionStatusStart[1] = controller.IndividualFootStatus(1);
                FullLegCollisionStatusStart[2] = controller.IndividualFootStatus(2);
                FullLegCollisionStatusStart[3] = controller.IndividualFootStatus(3);
                FullLegCollisionStatusStart[4] = controller.IndividualFootStatus(4);
                FullLegCollisionStatusStart[5] = controller.IndividualFootStatus(5);
                morphology = MorphologyIdentifier(FullLegCollisionStatusStart);
                
                if(StartTimePassed > 1)
                {
                    StartSequenceGoing = false;
                }
            }

            // Stability
            NumberOfStabilityMeasurements++;
            if(IsStable() == true)
            {
                NumberOfStableMeassurements++;                
            }

            DebugPCoMIsCorrect = IsStaticallyStable();
            DebugIsStable = IsStable();
            DebugComulativeStability += StaticStabilityMargin();
            DebugComulativeDynamicStability += DynamicStabilityMargin(-44 / 4, 44 / 4, -64 / 4, 64 / 4);

            DebugStaticStability = StaticStabilityMargin();
            DebugDynamicStability = DynamicStabilityMargin(-44 / 4, 44 / 4, -64 / 4, 64 / 4);

            DebugAverageDynamicStabilityMargin = DebugComulativeDynamicStability / NumberOfStabilityMeasurements;
            DebugAverageStabilityMargin = DebugComulativeStability / NumberOfStabilityMeasurements;
            StabilityPercentage = NumberOfStableMeassurements / NumberOfStabilityMeasurements;

            // Find Heading Vector to target
            Vector3 VectorToTarget = Targets[TargetsReachedCounter].position - this.transform.position;
            Vector3 HeadingVector = VectorToTarget.normalized;

            // Direction Interpretation
            float AngleToTarget = Vector3.SignedAngle(this.transform.forward, HeadingVector, Vector3.up);

            direction = 1;
            if(AngleToTarget >= -30 && AngleToTarget <= 30)
            {
                direction = 1;
            }
            else if(AngleToTarget < -30)
            {
                direction = 2;
            }
            else if(AngleToTarget > 30)
            {
                direction = 3;
            }

            // Terrain Difficulty Estimation
            Vector3[] FullLegRelativePositions = new Vector3[6] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            float[] FullLegCollisionStatus = new float[6] { -1, -1, -1, -1, -1, -1 };

            FullLegRelativePositions[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);
            FullLegRelativePositions[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);
            FullLegRelativePositions[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);
            FullLegRelativePositions[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);
            FullLegRelativePositions[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);
            FullLegRelativePositions[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);

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

            /*****************************************************************/        
            LegFrequency = LegFrequency / UpdateFrequency;
            GaitBank((int)morphology, (int)direction, VerticalStepSize, HorizontalStepSize, LegFrequency, Height);  

        }
        
        FrequencyCounter += Time.fixedDeltaTime; // Must be in the bottom
    }

    public float[] previousOutput;

    private bool TriggeredOnce = false;

    

    public void GenericControl() // Main loop
    {
        // Check if target is reached
        float distanceToTarget = Vector3.Distance(this.transform.position, Targets[TargetsReachedCounter].position);
        if(distanceToTarget < 1)
        {
            if(TargetsReachedCounter >= Targets.Length - 1)
            {
                // Debug.Log("Reached the end.");
                CompletedTask = true;

                // Debug.Log("Distance Moved: " + DistanceMoved); // Use in evaluation

                // return;
            }
            else
            {
                TargetsReachedCounter++;

                if(TargetsReachedCounter >= 4 && TriggeredOnce == false)
                {
                    TriggerDamage();
                    TriggeredOnce = true;
                }
            }
        }

        // Controller
        if(FrequencyCounter > 1/UpdateFrequency)
        {
            FrequencyCounter = 0;

            if(StartSequenceGoing == true) // Start sequence to determine morphology.
            {
                StartTimePassed += 1/UpdateFrequency;

                float[] FullLegCollisionStatusStart = new float[6] { -1, -1, -1, -1, -1, -1 };
                FullLegCollisionStatusStart[0] = controller.IndividualFootStatus(0);
                FullLegCollisionStatusStart[1] = controller.IndividualFootStatus(1);
                FullLegCollisionStatusStart[2] = controller.IndividualFootStatus(2);
                FullLegCollisionStatusStart[3] = controller.IndividualFootStatus(3);
                FullLegCollisionStatusStart[4] = controller.IndividualFootStatus(4);
                FullLegCollisionStatusStart[5] = controller.IndividualFootStatus(5);
                morphology = MorphologyIdentifier(FullLegCollisionStatusStart);
                
                if(StartTimePassed > 1)
                {
                    StartSequenceGoing = false;
                }
            }

            // Stability
            NumberOfStabilityMeasurements++;
            if(IsStable() == true)
            {
                NumberOfStableMeassurements++;                
            }

            DistanceMoved += Vector3.Distance(this.transform.position, lastPosition);
            lastPosition = this.transform.position;

            DebugPCoMIsCorrect = IsStaticallyStable();
            DebugIsStable = IsStable();
            DebugComulativeStability += StaticStabilityMargin();
            DebugComulativeDynamicStability += DynamicStabilityMargin(-44 / 2, 44 / 2, -64 / 2, 64 / 2);

            DebugDynamicStability = DynamicStabilityMargin(-44 / 2, 44 / 2, -64 / 2, 64 / 2);

            if(DebugDynamicStability < LowestDynamicMarginObserved)
            {
                LowestDynamicMarginObserved = DebugDynamicStability;
            }

            DebugAverageStabilityMargin = DebugComulativeStability / NumberOfStabilityMeasurements;
            DebugAverageDynamicStabilityMargin = DebugComulativeDynamicStability / NumberOfStabilityMeasurements;
            StabilityPercentage = NumberOfStableMeassurements / NumberOfStabilityMeasurements;

            // DataAnalyser.FileWriteLine("StabilityDamage", LowestDynamicMarginObserved);

            // Find Heading Vector to target
            Vector3 VectorToTarget = Targets[TargetsReachedCounter].position - this.transform.position;
            Vector3 HeadingVector = VectorToTarget.normalized;

            // Direction Interpretation
            float AngleToTarget = Vector3.SignedAngle(this.transform.forward, HeadingVector, Vector3.up);
            
            /*
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
            */

            direction = 1;
            if(AngleToTarget >= -30 && AngleToTarget <= 30)
            {
                direction = 1;
            }
            else if(AngleToTarget < -30)
            {
                direction = 2;
            }
            else if(AngleToTarget > 30)
            {
                direction = 3;
            }

            // Terrain Difficulty Estimation
            Vector3[] FullLegRelativePositions = new Vector3[6] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            float[] FullLegCollisionStatus = new float[6] { -1, -1, -1, -1, -1, -1 };

            FullLegRelativePositions[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);
            FullLegRelativePositions[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);
            FullLegRelativePositions[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);
            FullLegRelativePositions[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);
            FullLegRelativePositions[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);
            FullLegRelativePositions[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);

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
            // float[] input = new float[2]{terrainSteepness, morphology};

            float val1 = 0;
            float val2 = 0;
            if(morphology == 0)
            {
                val1 = 1;
                val2 = 0;
            }

            if(morphology == 1)
            {
                val1 = 0;
                val2 = 1;
            }


            float[] input = new float[3]{val1, val2, terrainSteepness};
            

            
            // float[] input = new float[2]{val1, val2};

            DebugInput = input;

            //float[] output;
            //output = network.FeedForward(input);

            float[] output;
            output  = network.FeedForward(input);

            DebugOutput = output;
            previousOutput = output;

            /*****************************************************************/

            float[] convertedOutputs = new float[4] {0, 0, 0, 0};
            convertedOutputs[0] = ValueRangeMapping(Mathf.Clamp(previousOutput[0], 0, 1), 0, 1, 0, 45);
            convertedOutputs[1] = ValueRangeMapping(Mathf.Clamp(previousOutput[1], 0, 1), 0, 1, 0, 45);
            convertedOutputs[2] = ValueRangeMapping(Mathf.Clamp(previousOutput[2], 0, 1), 0, 1, 0f, 4f);
            convertedOutputs[3] = ValueRangeMapping(Mathf.Clamp(previousOutput[3], 0, 1), 0, 1, -45, 45);

            float LegFrequency = convertedOutputs[2] / UpdateFrequency;
            GaitBank((int)morphology, (int)direction, convertedOutputs[0], convertedOutputs[1], LegFrequency, convertedOutputs[3]);  
        }


        FrequencyCounter += Time.fixedDeltaTime; // Must be in the bottom
    }

    private float LastTerrainDifficultyObserved = 0;
    private float TerrainDifficultyDeterminer(Vector3[] LegPositions, float[] LegCollsions)
    {
        float highestAngleObserved = -100000;
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

        LastTerrainDifficultyObserved = Mathf.Clamp(highestAngleObserved, LastTerrainDifficultyObserved - 0f, LastTerrainDifficultyObserved + 0.0f);
        return LastTerrainDifficultyObserved;
    }

    private float[] RunningAverageActualCollisions;
    private float[] RunningAverageEstimatedHexCollisions;
    private float[] RunningAverageEstimatedQuadCollisions;
    private float[] RunningAverageEstimatedDamagedCollisions;

    private int RunningAverageCounter = 0;

    public int HexapodSurreness = 0;
    public int QuadrupedSurreness = 0;

    public int DebugMorphologyGuess;
    private float MorphologyIdentifier(float[] LegCollisions)
    {
        /*
        int NumberOfActualLegCollisions = 0;

        for(int i = 0; i < LegCollisions.Length; i++)
        {
            if(LegCollisions[i] == 1)
            {
                NumberOfActualLegCollisions++;
            }
        }

        DebugActualLegsInContact = NumberOfActualLegCollisions;
        float HexEstimatedCollisions = HexapodModel();
        float QuadEstimatedCollisions = QuadrupedModel();
        float DamagedHexEstimatedCollisions = DamagedHexapodModel();

        DebugEstimatedLegsInContactHex = HexEstimatedCollisions;
        DebugEstimatedLegsInContactQuad = QuadEstimatedCollisions;
        */

        /*
        if(Mathf.Abs(NumberOfActualLegCollisions - HexEstimatedCollisions) == Mathf.Abs(NumberOfActualLegCollisions - QuadEstimatedCollisions))
        {
            HexapodSurreness++;
            QuadrupedSurreness++;
        }
        else if (Mathf.Abs(NumberOfActualLegCollisions - QuadEstimatedCollisions) < Mathf.Abs(NumberOfActualLegCollisions - HexEstimatedCollisions))
        {
            QuadrupedSurreness++;
        }
        else
        {
            HexapodSurreness++;
        }

        if(QuadrupedSurreness >= HexapodSurreness)
        {
            DebugMorphologyGuess = 0;
        }
        else
        {
            DebugMorphologyGuess = 1;
        }
        */

        /*
        RunningAverageActualCollisions[RunningAverageCounter] = NumberOfActualLegCollisions;
        RunningAverageEstimatedHexCollisions[RunningAverageCounter] = HexEstimatedCollisions;
        RunningAverageEstimatedQuadCollisions[RunningAverageCounter] = QuadEstimatedCollisions;

        RunningAverageCounter++;
        if(RunningAverageCounter >= UpdateFrequency * 10)
        {
            RunningAverageCounter = 0;
        }

        // Calculate average
        float ActualCollisionSum = 0;
        float EstimatedHexSum = 0;
        float EstimatedQuadSum = 0;
        for(int i = 0; i < RunningAverageActualCollisions.Length; i++)
        {
            ActualCollisionSum += RunningAverageActualCollisions[i];
            EstimatedHexSum += RunningAverageEstimatedHexCollisions[i];
            EstimatedQuadSum += RunningAverageEstimatedQuadCollisions[i];
        }
        float AverageActualCollision = ActualCollisionSum / RunningAverageActualCollisions.Length;
        float AverageEstimatedHexCollision = EstimatedHexSum / RunningAverageEstimatedHexCollisions.Length;
        float AverageEstimatedQuadCollision = EstimatedQuadSum / RunningAverageEstimatedQuadCollisions.Length;

        if(Mathf.Abs(AverageActualCollision - AverageEstimatedQuadCollision) < Mathf.Abs(AverageActualCollision - AverageEstimatedHexCollision))
        {
            DebugMorphologyGuess = 0;
        }
        else
        {
            DebugMorphologyGuess = 1;
        }
        */

        // Running sureness
        /*
        if(Mathf.Abs(NumberOfActualLegCollisions - HexEstimatedCollisions) == Mathf.Abs(NumberOfActualLegCollisions - QuadEstimatedCollisions))
        {
            RunningAverageEstimatedHexCollisions[RunningAverageCounter] = 1;
            RunningAverageEstimatedQuadCollisions[RunningAverageCounter] = 1;
        }
        else if (Mathf.Abs(NumberOfActualLegCollisions - QuadEstimatedCollisions) < Mathf.Abs(NumberOfActualLegCollisions - HexEstimatedCollisions))
        {
            RunningAverageEstimatedHexCollisions[RunningAverageCounter] = 0;
            RunningAverageEstimatedQuadCollisions[RunningAverageCounter] = 1;
        }
        else
        {
            RunningAverageEstimatedHexCollisions[RunningAverageCounter] = 1;
            RunningAverageEstimatedQuadCollisions[RunningAverageCounter] = 0;
        }

        RunningAverageCounter++;
        if(RunningAverageCounter >= UpdateFrequency)
        {
            RunningAverageCounter = 0;
        }

        float EstimatedHexSum = 0;
        float EstimatedQuadSum = 0;
        for(int i = 0; i < RunningAverageActualCollisions.Length; i++)
        {
            EstimatedHexSum += RunningAverageEstimatedHexCollisions[i];
            EstimatedQuadSum += RunningAverageEstimatedQuadCollisions[i];
        }

        if(EstimatedQuadSum >= EstimatedHexSum)
        {
            // DebugMorphologyGuess = 0;
        }
        else
        {
            // DebugMorphologyGuess = 1;
        }
        */

        // The actual working code

        /*
        float SumHexEstDistance = HexapodModel(); // this is done using the Distance of each foot
        float SumQuadEstDistance = QuadrupedModel();
        float SumDamHexEstDistance = DamagedHexapodModel();
        float SumFootDistance = 0;
        for(int i = 0; i < controller.FootDetectors.Length; i++)
        {
            SumFootDistance += controller.FootDistanceToGround(i);
        }

        float AbsoluteDistanceHex = Mathf.Abs(SumFootDistance - SumHexEstDistance);
        float AbsoluteDistanceQuad = Mathf.Abs(SumFootDistance - SumQuadEstDistance);
        float AbsoluteDistanceDamHex = Mathf.Abs(SumFootDistance - SumDamHexEstDistance);

        if(AbsoluteDistanceQuad < AbsoluteDistanceHex && AbsoluteDistanceQuad < AbsoluteDistanceDamHex)
        {
            DebugMorphologyGuess = 0;
            return 0;
        }

        if(AbsoluteDistanceHex < AbsoluteDistanceQuad && AbsoluteDistanceHex < AbsoluteDistanceDamHex)
        {
            DebugMorphologyGuess = 1;
            return 1;
        }

        if(AbsoluteDistanceDamHex < AbsoluteDistanceQuad && AbsoluteDistanceDamHex < AbsoluteDistanceHex)
        {
            DebugMorphologyGuess = 2;
            return 2;
        }
        else
        {
            return 0; // Exception catch
        }
        */ 

        
        int NumberOfEstimatedFeet = 0;
        for(int i = 0; i < controller.foots.Length; i++)
        {
            if(controller.FootDetectors[i].IsResponding() == true)
            {
                NumberOfEstimatedFeet++;
            }
        }


        if(NumberOfEstimatedFeet == QuadrupedModelInt())
        {
            DebugMorphologyGuess = 0;
            return 0;
        }

        if(NumberOfEstimatedFeet == DamagedHexapodModelInt())
        {
            DebugMorphologyGuess = 2;
            return 2;
        }

        if(NumberOfEstimatedFeet == HexapodModelInt())
        {
            DebugMorphologyGuess = 1;
            return 1;
        }
        

        // This is not used I think.
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

        if(morphology == 2)
        {
            DamagedHexapodTrotGait(direction, VerticalStepSize, HorizontalStepSize, speed, height);
        }

        if(morphology > 2) // Exception catch
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
        if(controller.DamageHasHappend == true)
        {
            DamagedHexapod2TrotGait(direction, VerticalStepSize, HorizontalStepSize, speed, height);
            return;
        }


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



    private void DamagedHexapodTrotGait(int direction, float VerticalStepSize, float HorizontalStepSize, float speed, float height)
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
        controller.SetLegConfiguration(3, Group1UpperLegAxis1, legDir3 * Group1UpperLegAxis2, Group1UnderLegAxis1);

        controller.SetLegConfiguration(2, Group2UpperLegAxis1, -legDir2 * Group2UpperLegAxis2, Group2UnderLegAxis1);
        controller.SetLegConfiguration(4, Group2UpperLegAxis1, legDir4 * Group2UpperLegAxis2, Group2UnderLegAxis1);
        controller.SetLegConfiguration(5, Group2UpperLegAxis1, legDir4 * Group2UpperLegAxis2, Group2UnderLegAxis1);

        if (timeStep1 >= 1)
        {
            timeStep1 = 0;
        }

        if (timeStep2 >= 1)
        {
            timeStep2 = 0;
        }
    }



    private void DamagedHexapod2TrotGait(int direction, float VerticalStepSize, float HorizontalStepSize, float speed, float height)
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
        controller.SetLegConfiguration(3, Group1UpperLegAxis1, legDir3 * Group1UpperLegAxis2, Group1UnderLegAxis1);

        controller.SetLegConfiguration(2, Group2UpperLegAxis1, -legDir2 * Group2UpperLegAxis2, Group2UnderLegAxis1);
        controller.SetLegConfiguration(5, Group2UpperLegAxis1, legDir4 * Group2UpperLegAxis2, Group2UnderLegAxis1);

        if (timeStep1 >= 1)
        {
            timeStep1 = 0;
        }

        if (timeStep2 >= 1)
        {
            timeStep2 = 0;
        }
    }



    public float EstimatedGroundHeightHexapod = 0;
    public float EstimatedGroundHeightQuadruped = 0;
    public float EstimatedHeightMargin = 0.03f; 

    public int DebugActualLegsInContact = 0;
    public float DebugEstimatedLegsInContactHex = 0;
    public float DebugEstimatedLegsInContactQuad = 0;
    public int DebugEstimatedLegInContactDamagedHex = 0;

    private float HexapodModel() // This needs to be finished.
    {
        /*
        int EstimateLegsInContactWithGround = 0;
        for(int i = 0; i < 6; i++)
        {
            if(IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2]).y < EstimatedGroundHeightHexapod + EstimatedHeightMargin)
            {
                EstimateLegsInContactWithGround++;
            }
        }
        return EstimateLegsInContactWithGround;
        */

        float SumDistanceToGroundHexapod = 0;
        for(int i = 0; i < 6; i++)
        {
            SumDistanceToGroundHexapod += Mathf.Abs(IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2]).y - EstimatedGroundHeightHexapod);
        }

        return SumDistanceToGroundHexapod;



    }

    private int HexapodModelInt()
    {
        int NumberOfFunctionalFeetsExpectedForHexapod = 6;
        return NumberOfFunctionalFeetsExpectedForHexapod;
    }

    private float DamagedHexapodModel() // This needs to be finished.
    {
        /*
        int EstimateLegsInContactWithGround = 0;

        if(IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]).y < EstimatedGroundHeightHexapod + EstimatedHeightMargin)
        {
            EstimateLegsInContactWithGround++;
        }

        for(int i = 2; i < 6; i++)
        {
            if(IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2]).y < EstimatedGroundHeightHexapod + EstimatedHeightMargin)
            {
                EstimateLegsInContactWithGround++;
            }
        }
        return EstimateLegsInContactWithGround;
        */

        // int NumberOfFunctionalFeetsExpectedForDamagedHexapod = 5;
        // return NumberOfFunctionalFeetsExpectedForDamagedHexapod;

        float SumDistanceToGroundHexapod = 0;
        SumDistanceToGroundHexapod += Mathf.Abs(IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]).y - EstimatedGroundHeightHexapod);
        for(int i = 2; i < 4; i++)
        {
            SumDistanceToGroundHexapod += Mathf.Abs(IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2]).y - EstimatedGroundHeightHexapod);
        }

        return SumDistanceToGroundHexapod;

    }

    private int QuadrupedModelInt()
    {
        int NumberOfFunctionalFeetsExpectedForHexapod = 4;
        return NumberOfFunctionalFeetsExpectedForHexapod;
    }

    private int DamagedHexapodModelInt()
    {
        int NumberOfFunctionalFeetsExpectedForHexapod = 5;
        return NumberOfFunctionalFeetsExpectedForHexapod;
    }

    private float QuadrupedModel() // This needs to be finished.
    {
        /*
        int EstimateLegsInContactWithGround = 0;
        for(int i = 0; i < 4; i++)
        {
            if(IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2]).y < EstimatedGroundHeightHexapod + EstimatedHeightMargin)
            {
                EstimateLegsInContactWithGround++;
            }
        }
        return EstimateLegsInContactWithGround;
        */
        
        float SumDistanceToGroundHexapod = 0;
        for(int i = 0; i < 4; i++)
        {
            SumDistanceToGroundHexapod += Mathf.Abs(IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2]).y - EstimatedGroundHeightHexapod);
        }

        return SumDistanceToGroundHexapod;
    }

    /************************* EXTRA FUNCTIONALITY ************************************/
    private float ValueRangeMapping(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (value - oldMin) * (newMax - newMin) / (oldMax - oldMin); // Inspired from https://prime31.github.io/simple-value-mapping/
    }

    public void FitnessFunction()
    {
        Vector3 CoMWorldPosition = Vector3.zero;

        float distanceBetweenTargetAndRobot = Vector3.Distance(this.transform.position, Targets[TargetsReachedCounter].position);
        float reward = 0;

        reward = 50 * TargetsReachedCounter - distanceBetweenTargetAndRobot;

        network.fitness = reward;
    }

    private void Start()
    {
        EstimatedGroundHeightHexapod = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]).y;
        EstimatedGroundHeightQuadruped = EstimatedGroundHeightHexapod;

        RunningAverageActualCollisions = new float[(int)UpdateFrequency];
        RunningAverageEstimatedHexCollisions = new float[(int)UpdateFrequency];
        RunningAverageEstimatedQuadCollisions = new float[(int)UpdateFrequency];

        for(int i = 0; i < (int)UpdateFrequency; i++)
        {
            RunningAverageActualCollisions[i] = 0;
            RunningAverageEstimatedHexCollisions[i] = 0;
            RunningAverageEstimatedQuadCollisions[i] = 0;
        }
    }

    public float DebugTimestep = 0;
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        DebugTimestep = timeStep1;
        /*
        if(controller.IsStaticallyStable() || (!controller.IsStaticallyStable() && controller.StabilityMargin() < 0.1f) || ((timeStep1 <= 0.35 && timeStep1 >= 0.15) || (timeStep1 >= 0.65 && timeStep1 <= 0.85)))
        {
            stability = 1;
        } 
        */       

        Gizmos.color = Color.red;
        if(IsStable() == true)
        {
            Gizmos.color = Color.green;
        }

        for(int i = 0; i < FoundPoints.Length; i++)
        {
            Gizmos.DrawSphere(FoundPoints[i] + this.transform.position + new Vector3(5, 0, 0), 0.1f);
        }

        Gizmos.DrawSphere(IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]) + this.transform.position, 0.2f);
        Gizmos.DrawSphere(IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]) + this.transform.position, 0.2f);
        Gizmos.DrawSphere(IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]) + this.transform.position, 0.2f);
        Gizmos.DrawSphere(IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]) + this.transform.position, 0.2f);
        Gizmos.DrawSphere(IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]) + this.transform.position, 0.2f);
        Gizmos.DrawSphere(IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[4, 0], controller.Links[5, 1], controller.Links[5, 2]) + this.transform.position, 0.2f);

        Gizmos.color = Color.green;
        if(IsStable() == false)
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawSphere(GetCenterOfMass() + this.transform.position, 0.1f);
    }

    public Vector3[] FoundPoints;
    public Vector3 EstimatedCenterOfMass;

    // public Transform planeModel;

    public float AverageVelocity = 0;
    float VelocitySum = 0;
    float VelocityCalculations = 0;

    private void FixedUpdate()
    {
        Vector3 vel;

        vel = controller.Torso.velocity;
        VelocitySum += Vector3.Magnitude(vel);
        VelocityCalculations += 1;

        AverageVelocity = VelocitySum / VelocityCalculations;


        DebugIsStable = IsStable();

        DebugStaticStability = StaticStabilityMargin();
        DebugDynamicStability = DynamicStabilityMargin(-44 / 5, 44 / 5, -64 / 5, 64 / 5);

        // float robotRotationTest = controller.IMU.transform.eulerAngles.x;
        
        // DataAnalyser.FileWriteLine("StaticStabilityTest", robotRotationTest);
        // DataAnalyser.FileWriteLine("StaticStabilityTest", StaticStabilityMargin());

        // planeModel.transform.Rotate(new Vector3(0.1f, 0, 0));

        /*
        if(StaticStabilityMargin() > -0.001f)
        {
            planeModel.transform.Rotate(new Vector3(0.01f, 0, 0));
        }
        Debug.Log(StaticStabilityMargin());
        */

        /*
        if(controller.TorsoDetector.IsColliding == true)
        {
            DataAnalyser.CloseAllFiles();
        }
        */

        /*
        FoundPoints = new Vector3[6]{new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
        FoundPoints[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);     
        FoundPoints[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);     
        FoundPoints[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);     
        FoundPoints[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);     
        FoundPoints[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);     
        FoundPoints[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);     

        EstimatedCenterOfMass = GetCenterOfMass();

        DebugStaticStability = StaticStabilityMargin();
        DynamicMargin = DynamicStabilityMargin(-10, 10, -10, 10);
        */

        // Debug info
        /*
        Vector3[] FullLegRelativePositions = new Vector3[6] { new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1) };
        float[] FullLegCollisionStatus = new float[6] { -1, -1, -1, -1, -1, -1 };
        FullLegRelativePositions[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);
        FullLegRelativePositions[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);
        FullLegRelativePositions[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);
        FullLegRelativePositions[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);
        FullLegRelativePositions[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);
        FullLegRelativePositions[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);

        FullLegCollisionStatus[0] = controller.IndividualFootStatus(0);
        FullLegCollisionStatus[1] = controller.IndividualFootStatus(1);
        FullLegCollisionStatus[2] = controller.IndividualFootStatus(2);
        FullLegCollisionStatus[3] = controller.IndividualFootStatus(3);
        FullLegCollisionStatus[4] = controller.IndividualFootStatus(4);
        FullLegCollisionStatus[5] = controller.IndividualFootStatus(5);

        DebugStepness = TerrainDifficultyDeterminer(FullLegRelativePositions, FullLegCollisionStatus);
        */
    }


    public bool IsStaticallyStable()
    {
        float[] FeetCollisions = new float[6]{0, 0, 0, 0, 0, 0};
        Vector3[] FeetLocations = new Vector3[6]{new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)};

        FeetCollisions[0] = controller.IndividualFootStatus(0);
        FeetCollisions[1] = controller.IndividualFootStatus(1);
        FeetCollisions[2] = controller.IndividualFootStatus(2);
        FeetCollisions[3] = controller.IndividualFootStatus(3);
        FeetCollisions[4] = controller.IndividualFootStatus(4);
        FeetCollisions[5] = controller.IndividualFootStatus(5);

        FeetLocations[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);
        FeetLocations[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);
        FeetLocations[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);
        FeetLocations[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);
        FeetLocations[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);
        FeetLocations[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);

        ProjectedCenterOfMass = GetCenterOfMass();

        int PointIsOnLeftSide = 0; 
        int NumberOfConnections = 0;

        int FirstLegTouchingTheGround = -1;
        int LastLegTouchingTheGround = -1;
        for(int i = 0; i < FeetCollisions.Length - 1; i++)
        {
            for(int j = i + 1; j < FeetCollisions.Length; j++)
            {
                if(FeetCollisions[i] == 1 && FeetCollisions[j] == 1)
                {
                    LastLegTouchingTheGround = j;
                    if(FirstLegTouchingTheGround == -1)
                    {
                        FirstLegTouchingTheGround = i;
                    }

                    NumberOfConnections++;
                    // Comes from: https://inginious.org/course/competitive-programming/geometry-pointinconvex
                    Vector3 P1 = new Vector2(FeetLocations[j].x - FeetLocations[i].x, FeetLocations[j].z - FeetLocations[i].z);
                    Vector3 P2 = new Vector2(ProjectedCenterOfMass.x - FeetLocations[i].x, ProjectedCenterOfMass.z - FeetLocations[i].z);

                    if(Vector3.Cross(P1, P2).z >= 0)
                    {
                        PointIsOnLeftSide++;
                    }

                    break;
                }
            }
        }

        if(FirstLegTouchingTheGround != -1 && LastLegTouchingTheGround != -1)
        {
            if(FeetCollisions[FirstLegTouchingTheGround] == 1 && FeetCollisions[LastLegTouchingTheGround] == 1) // Only there as a catch
            {
                NumberOfConnections++;

                Vector3 P1 = new Vector2(FeetLocations[FirstLegTouchingTheGround].x - FeetLocations[LastLegTouchingTheGround].x, FeetLocations[FirstLegTouchingTheGround].z - FeetLocations[LastLegTouchingTheGround].z);
                Vector3 P2 = new Vector2(ProjectedCenterOfMass.x - FeetLocations[LastLegTouchingTheGround].x, ProjectedCenterOfMass.z - FeetLocations[LastLegTouchingTheGround].z);

                if(Vector3.Cross(P1, P2).z >= 0)
                {
                    PointIsOnLeftSide++;
                }
            }
        }        

        if(NumberOfConnections > 1 && PointIsOnLeftSide == NumberOfConnections)
        {
            return true;
        }
        return false;
    }


    public float LastStableTiltX = 0;
    public float LastStableTiltZ = 0;

    public float currentTiltX = 0;
    public float currentTiltZ = 0;  

    public float DynamicMargin = 0;

    private float DMarginX = 0;
    private float DMarginZ = 0; 
    public float DynamicStabilityMargin(float lowerLimitX, float upperLimitX, float lowerLimitZ, float upperLimitZ)
    {
        if(IsStaticallyStable())
        {
            LastStableTiltX = controller.IMU.CurrentOrientation.x;
            LastStableTiltZ = controller.IMU.CurrentOrientation.z;
        }

        currentTiltX = LastStableTiltX - controller.IMU.CurrentOrientation.x;
        currentTiltZ = LastStableTiltZ - controller.IMU.CurrentOrientation.z; 

        float currentLowerMarginX = lowerLimitX - currentTiltX;
        float currentUpperMarginX = upperLimitX - currentTiltX;

        float currentLowerMarginZ = lowerLimitZ - currentTiltZ;
        float currentUpperMarginZ = upperLimitZ - currentTiltZ;

        float MostNegativeValue = 0;
        float LeastPositiveValue = Mathf.Infinity;

        if(currentLowerMarginX > 0)
        {
            if(-Mathf.Abs(currentLowerMarginX) < MostNegativeValue)
            {
                MostNegativeValue = -Mathf.Abs(currentLowerMarginX);
            }
        }
        else
        {
            if(Mathf.Abs(currentLowerMarginX) < LeastPositiveValue)
            {
                LeastPositiveValue = Mathf.Abs(currentLowerMarginX);
            }
        }

        if(currentUpperMarginX < 0)
        {
            if(-Mathf.Abs(currentUpperMarginX) < MostNegativeValue)
            {
                MostNegativeValue = -Mathf.Abs(currentUpperMarginX);
            }
        }
        else
        {
            if(Mathf.Abs(currentUpperMarginX) < LeastPositiveValue)
            {
                LeastPositiveValue = Mathf.Abs(currentUpperMarginX);
            }
        }

        if(currentLowerMarginZ > 0)
        {
            if(-Mathf.Abs(currentLowerMarginZ) < MostNegativeValue)
            {
                MostNegativeValue = -Mathf.Abs(currentLowerMarginZ);
            }
        }
        else
        {
            if(Mathf.Abs(currentLowerMarginZ) < LeastPositiveValue)
            {
                LeastPositiveValue = Mathf.Abs(currentLowerMarginZ);
            }
        }

        if(currentUpperMarginZ < 0)
        {
            if(-Mathf.Abs(currentUpperMarginZ) < MostNegativeValue)
            {
                MostNegativeValue = -Mathf.Abs(currentUpperMarginZ);
            }
        }
        else
        {
            if(Mathf.Abs(currentUpperMarginZ) < LeastPositiveValue)
            {
                LeastPositiveValue = Mathf.Abs(currentUpperMarginZ);
            }
        }

        if(MostNegativeValue < 0)
        {
            return MostNegativeValue;
        }
        return LeastPositiveValue;
    }

    public bool IsDynamicallyStable(float lowerLimitX, float upperLimitX, float lowerLimitZ, float upperLimitZ)
    {
        if(DynamicStabilityMargin(lowerLimitX, upperLimitX, lowerLimitZ, upperLimitZ) < 0)
        {
            return false;
        }
        return true;
    }

    public List<float> PerpendicularDistances;
    public Vector3 ProjectedCenterOfMass;


    public float StabilityMargin()
    {
        float[] FeetCollisions = new float[6]{0, 0, 0, 0, 0, 0};
        Vector3[] FeetLocations = new Vector3[6]{new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)};

        FeetCollisions[0] = controller.IndividualFootStatus(0);
        FeetCollisions[1] = controller.IndividualFootStatus(1);
        FeetCollisions[2] = controller.IndividualFootStatus(2);
        FeetCollisions[3] = controller.IndividualFootStatus(3);
        FeetCollisions[4] = controller.IndividualFootStatus(4);
        FeetCollisions[5] = controller.IndividualFootStatus(5);

        FeetLocations[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);
        FeetLocations[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);
        FeetLocations[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);
        FeetLocations[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);
        FeetLocations[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);
        FeetLocations[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);

        ProjectedCenterOfMass = new Vector3(0, -0.5f, 0);
        PerpendicularDistances = new List<float>();

        int DistanceCounter = 0;

        int FirstLegTouchingTheGround = -1;
        int LastLegTouchingTheGround = -1;
        for(int i = 0; i < FeetCollisions.Length - 1; i++)
        {
            for(int j = i + 1; j < FeetCollisions.Length; j++)
            {
                if(FeetCollisions[i] == 1 && FeetCollisions[j] == 1)
                {
                    LastLegTouchingTheGround = j;
                    if(FirstLegTouchingTheGround == -1)
                    {
                        FirstLegTouchingTheGround = i;
                    }

                    // I think it has to be assumed that the polygon is convex
                    // Math inspired by https://hackaday.io/project/21904-hexapod-modelling-path-planning-and-control/log/62326-3-fundamentals-of-hexapod-robot                    
                    // Go counter clock wise from: https://www.mathwords.com/a/area_convex_polygon.htm
                    float ConvexArea = 0.5f * Mathf.Abs((FeetLocations[j].x - ProjectedCenterOfMass.x) * (FeetLocations[i].z - ProjectedCenterOfMass.z) - (FeetLocations[i].x - ProjectedCenterOfMass.x) * (FeetLocations[j].z - ProjectedCenterOfMass.z));
                    float DistanceBetweenContactPoints = Mathf.Sqrt(Mathf.Pow(FeetLocations[j].x - FeetLocations[i].x, 2) + Mathf.Pow(FeetLocations[j].z - FeetLocations[i].z, 2));
                    //float DistanceBetweenContactPoints = FeetLocations[j].x - FeetLocations[i].x + FeetLocations[j].z - FeetLocations[i].z;
                    float PerpendicularDistance = (2 * ConvexArea) / DistanceBetweenContactPoints;

                    PerpendicularDistances.Add(PerpendicularDistance);
                    DistanceCounter++;
                    break;
                }
            }
        }

        if(FirstLegTouchingTheGround != -1 && LastLegTouchingTheGround != -1)
        {
            if(FeetCollisions[FirstLegTouchingTheGround] == 1 && FeetCollisions[LastLegTouchingTheGround] == 1) // Only there as a catch
            {
                // Math inspired by https://hackaday.io/project/21904-hexapod-modelling-path-planning-and-control/log/62326-3-fundamentals-of-hexapod-robot                    
                // Go counter clock wise from: https://www.mathwords.com/a/area_convex_polygon.htm
                float ConvexArea = 0.5f * Mathf.Abs((FeetLocations[LastLegTouchingTheGround].x - ProjectedCenterOfMass.x) * (FeetLocations[FirstLegTouchingTheGround].z - ProjectedCenterOfMass.z) - (FeetLocations[FirstLegTouchingTheGround].x - ProjectedCenterOfMass.x) * (FeetLocations[LastLegTouchingTheGround].z - ProjectedCenterOfMass.z));
                float DistanceBetweenContactPoints = Mathf.Sqrt(Mathf.Pow(FeetLocations[LastLegTouchingTheGround].x - FeetLocations[FirstLegTouchingTheGround].x, 2) + Mathf.Pow(FeetLocations[LastLegTouchingTheGround].z - FeetLocations[FirstLegTouchingTheGround].z, 2));
                //float DistanceBetweenContactPoints = FeetLocations[LastLegTouchingTheGround].x - FeetLocations[FirstLegTouchingTheGround].x + FeetLocations[LastLegTouchingTheGround].z - FeetLocations[FirstLegTouchingTheGround].z;
                float PerpendicularDistance = (2 * ConvexArea) / DistanceBetweenContactPoints;

                PerpendicularDistances.Add(PerpendicularDistance);
                DistanceCounter++;
            }
        }        

        if(PerpendicularDistances.Count > 1)
        {
            return PerpendicularDistances.AsQueryable().Min();
        }

        int NumberOfCollisions = 0;

        if(controller.TorsoDetector.IsColliding == true)
        {
            NumberOfCollisions++;
        }

        for(int i = 0; i < controller.UpperLegDetectors.Length; i++)
        {
            if(controller.UpperLegDetectors[i].IsColliding == true || controller.UnderLegDetectors[i].IsColliding == true)
            {
                NumberOfCollisions++;
            }
        }

        if(NumberOfCollisions > 0)
        {
            return -1;
        }

        return 0;
    }

    public bool IsStable()
    {
        float StaticStability = StaticStabilityMargin();
        float DynamicStability = DynamicStabilityMargin(-44 / 2, 44 / 2, -64 / 2, 64 / 2);

        if(StaticStability <= 0)
        {
            if(DynamicStability < 0)
            {
                return false;
            }
        }
        return true;
    }

    public float StaticStabilityMargin()
    {
        float[] FeetCollisions = new float[6]{0, 0, 0, 0, 0, 0};
        Vector3[] FeetLocations = new Vector3[6]{new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)};

        FeetCollisions[0] = controller.IndividualFootStatus(0);
        FeetCollisions[1] = controller.IndividualFootStatus(1);
        FeetCollisions[2] = controller.IndividualFootStatus(2);
        FeetCollisions[3] = controller.IndividualFootStatus(3);
        FeetCollisions[4] = controller.IndividualFootStatus(4);
        FeetCollisions[5] = controller.IndividualFootStatus(5);

        FeetLocations[0] = IndividualLegForwardKinematics(0, controller.Angles[0,0], controller.Links[0, 0], controller.Links[0, 1], controller.Links[0, 2]);
        FeetLocations[1] = IndividualLegForwardKinematics(1, controller.Angles[1,0], controller.Links[1, 0], controller.Links[1, 1], controller.Links[1, 2]);
        FeetLocations[2] = IndividualLegForwardKinematics(2, controller.Angles[2,0], controller.Links[2, 0], controller.Links[2, 1], controller.Links[2, 2]);
        FeetLocations[3] = IndividualLegForwardKinematics(3, controller.Angles[3,0], controller.Links[3, 0], controller.Links[3, 1], controller.Links[3, 2]);
        FeetLocations[4] = IndividualLegForwardKinematics(4, controller.Angles[4,0], controller.Links[4, 0], controller.Links[4, 1], controller.Links[4, 2]);
        FeetLocations[5] = IndividualLegForwardKinematics(5, controller.Angles[5,0], controller.Links[5, 0], controller.Links[5, 1], controller.Links[5, 2]);

        ProjectedCenterOfMass = GetCenterOfMass();
        PerpendicularDistances = new List<float>();

        int DistanceCounter = 0;

        int FirstLegTouchingTheGround = -1;
        int LastLegTouchingTheGround = -1;
        for(int i = 0; i < FeetCollisions.Length - 1; i++)
        {
            for(int j = i + 1; j < FeetCollisions.Length; j++)
            {
                if(FeetCollisions[i] == 1 && FeetCollisions[j] == 1)
                {
                    LastLegTouchingTheGround = j;
                    if(FirstLegTouchingTheGround == -1)
                    {
                        FirstLegTouchingTheGround = i;
                    }

                    // I think it has to be assumed that the polygon is convex
                    // Math inspired by https://hackaday.io/project/21904-hexapod-modelling-path-planning-and-control/log/62326-3-fundamentals-of-hexapod-robot                    
                    // Go counter clock wise from: https://www.mathwords.com/a/area_convex_polygon.htm
                    float ConvexArea = 0.5f * Mathf.Abs((FeetLocations[j].x - ProjectedCenterOfMass.x) * (FeetLocations[i].z - ProjectedCenterOfMass.z) - (FeetLocations[i].x - ProjectedCenterOfMass.x) * (FeetLocations[j].z - ProjectedCenterOfMass.z));
                    float DistanceBetweenContactPoints = Mathf.Sqrt(Mathf.Pow(FeetLocations[j].x - FeetLocations[i].x, 2) + Mathf.Pow(FeetLocations[j].z - FeetLocations[i].z, 2));
                    //float DistanceBetweenContactPoints = FeetLocations[j].x - FeetLocations[i].x + FeetLocations[j].z - FeetLocations[i].z;
                    float PerpendicularDistance = (2 * ConvexArea) / DistanceBetweenContactPoints;

                    PerpendicularDistances.Add(PerpendicularDistance);
                    DistanceCounter++;
                    break;
                }
            }
        }

        if(FirstLegTouchingTheGround != -1 && LastLegTouchingTheGround != -1)
        {
            if(FeetCollisions[FirstLegTouchingTheGround] == 1 && FeetCollisions[LastLegTouchingTheGround] == 1) // Only there as a catch
            {
                // Math inspired by https://hackaday.io/project/21904-hexapod-modelling-path-planning-and-control/log/62326-3-fundamentals-of-hexapod-robot                    
                // Go counter clock wise from: https://www.mathwords.com/a/area_convex_polygon.htm
                float ConvexArea = 0.5f * Mathf.Abs((FeetLocations[LastLegTouchingTheGround].x - ProjectedCenterOfMass.x) * (FeetLocations[FirstLegTouchingTheGround].z - ProjectedCenterOfMass.z) - (FeetLocations[FirstLegTouchingTheGround].x - ProjectedCenterOfMass.x) * (FeetLocations[LastLegTouchingTheGround].z - ProjectedCenterOfMass.z));
                float DistanceBetweenContactPoints = Mathf.Sqrt(Mathf.Pow(FeetLocations[LastLegTouchingTheGround].x - FeetLocations[FirstLegTouchingTheGround].x, 2) + Mathf.Pow(FeetLocations[LastLegTouchingTheGround].z - FeetLocations[FirstLegTouchingTheGround].z, 2));
                //float DistanceBetweenContactPoints = FeetLocations[LastLegTouchingTheGround].x - FeetLocations[FirstLegTouchingTheGround].x + FeetLocations[LastLegTouchingTheGround].z - FeetLocations[FirstLegTouchingTheGround].z;
                float PerpendicularDistance = (2 * ConvexArea) / DistanceBetweenContactPoints;

                PerpendicularDistances.Add(PerpendicularDistance);
                DistanceCounter++;
            }
        }        

        /*
        if(PerpendicularDistances.Count == 1) 
        {
            float lowestMargin = PerpendicularDistances.AsQueryable().Min();
            if(lowestMargin < 0.1f) // This value needs to be remembered.
            {
                return lowestMargin;
            }
            else
            {
                return -lowestMargin;
            }
        }
        */

        if(PerpendicularDistances.Count > 1)
        {
            bool IsStableNow = IsStaticallyStable();
            float lowestMargin = PerpendicularDistances.AsQueryable().Min();
            if(IsStableNow == true || lowestMargin <= 0.05f)
            {
                return lowestMargin;
            }
            else
            {
                return -lowestMargin;
            }
        }

        return 0;
    }

    public Vector3 GetCenterOfMass()
    {
        Vector3 num = Vector3.zero;
    
        // CenterOfMassSum += Torso.mass;
        float denum = controller.Torso.mass;

        for(int i = 0; i < controller.UpperLegs.Length; i++)
        {
            num += controller.UpperLegs[i].mass * (IndividualShoulderForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0]) + controller.UpperLegs[i].centerOfMass);
            num += controller.UnderLegs[i].mass * (IndividualKneeForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1])  + controller.UnderLegs[i].centerOfMass);
            num += controller.foots[i].mass * (IndividualLegForwardKinematics(i, controller.Angles[i,0], controller.Links[i, 0], controller.Links[i, 1], controller.Links[i, 2])  + controller.UpperLegs[i].centerOfMass);

            denum += controller.UpperLegs[i].mass + controller.UnderLegs[i].mass + controller.foots[i].mass;
        }

        // Debug.Log(Torso.mass);
        return num / denum;
    }

    public Vector3 IndividualShoulderForwardKinematics(int LegNumber, float theta1, float link1)
    {
        if(LegNumber > 5)
        {
            Debug.Log("Trying to check a leg that is not possible for kinematics");
        }

        if(LegNumber > controller.UpperLegs.Length - 1)
        {
            return Vector3.zero;
        }

        Vector3 TorsoToLeg = new Vector3(0, 0, link1);

        // float Angle = 305 - 35 * (LegNumbering[LegNumber] - 1);
        float Angle = theta1;
        // Quaternion rotationTorso;
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        // if(LegNumber > UpperLegs.Length / 2 - 1)
        // {
        //    Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        // }
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.x, Angle + IMU.CurrentOrientation.y, IMU.CurrentOrientation.z);

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(controller.IMU.CurrentOrientation.x, controller.IMU.CurrentOrientation.y, controller.IMU.CurrentOrientation.z));
        // Matrix4x4 mB2 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB3 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB4 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB5 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB6 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));

        Quaternion rotationTorso = Quaternion.Euler(0, Angle, 0);

        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);


        // Vector3 EstimatedPosition = m1.MultiplyPoint3x4(TorsoToLeg);
        Vector3 EstimatedPosition =  mB1.MultiplyPoint3x4(m1.MultiplyPoint3x4(TorsoToLeg));

        return EstimatedPosition;
    }

    public Vector3 IndividualKneeForwardKinematics(int LegNumber, float theta1, float link1, float link2)
    {
        if(LegNumber > 5)
        {
            Debug.Log("Trying to check a leg that is not possible for kinematics");
        }

        if(LegNumber > controller.UpperLegs.Length - 1)
        {
            return Vector3.zero;
        }

        Vector3 TorsoToLeg = new Vector3(0, 0, link1);
        Vector3 UpperLegToKnee = new Vector3(0, 0, link2);

        // float Angle = 305 - 35 * (LegNumbering[LegNumber] - 1);
        float Angle = theta1;
        // Quaternion rotationTorso;
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        // if(LegNumber > UpperLegs.Length / 2 - 1)
        // {
        //    Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        // }
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.x, Angle + IMU.CurrentOrientation.y, IMU.CurrentOrientation.z);

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(controller.IMU.CurrentOrientation.x, controller.IMU.CurrentOrientation.y, controller.IMU.CurrentOrientation.z));
        // Matrix4x4 mB2 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB3 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB4 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB5 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB6 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));

        Quaternion rotationTorso = Quaternion.Euler(0, Angle, 0);
        Quaternion rotationUpperLeg = Quaternion.Euler(20 - controller.UpperLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg, Angle - controller.UpperLegs[LegNumber].jointPosition[1] * Mathf.Rad2Deg, 0);

        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);
        Matrix4x4 m2 = Matrix4x4.Rotate(rotationUpperLeg);

        // Vector3 EstimatedPosition = m1.MultiplyPoint3x4(TorsoToLeg);
        Vector3 EstimatedPosition =  mB1.MultiplyPoint3x4(m1.MultiplyPoint3x4(TorsoToLeg)) + mB1.MultiplyPoint3x4(m2.MultiplyPoint3x4(UpperLegToKnee));

        return EstimatedPosition;
    }


    public Vector3 IndividualLegForwardKinematics(int LegNumber, float theta1, float link1, float link2, float link3)
    {
        if(LegNumber > 5)
        {
            Debug.Log("Trying to check a leg that is not possible for kinematics");
        }

        if(LegNumber > controller.UpperLegs.Length - 1)
        {
            return Vector3.zero;
        }

        Vector3 TorsoToLeg = new Vector3(0, 0, link1);
        Vector3 UpperLegToKnee = new Vector3(0, 0, link2);
        Vector3 KneeToFoot = new Vector3(0, 0, link3);

        // float Angle = 305 - 35 * (LegNumbering[LegNumber] - 1);
        float Angle = theta1;
        // Quaternion rotationTorso;
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        // if(LegNumber > UpperLegs.Length / 2 - 1)
        // {
        //    Angle -= 75;
            //rotationTorso = Quaternion.Euler(-IMU.CurrentOrientation.z, Angle + IMU.CurrentOrientation.y, 0);
        // }
        //rotationTorso = Quaternion.Euler(IMU.CurrentOrientation.x, Angle + IMU.CurrentOrientation.y, IMU.CurrentOrientation.z);

        Matrix4x4 mB1 = Matrix4x4.Rotate(Quaternion.Euler(controller.IMU.CurrentOrientation.x, controller.IMU.CurrentOrientation.y, controller.IMU.CurrentOrientation.z));
        // Matrix4x4 mB2 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB3 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB4 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB5 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));
        // Matrix4x4 mB6 = Matrix4x4.Rotate(Quaternion.Euler(IMU.CurrentOrientation.x, IMU.CurrentOrientation.y, IMU.CurrentOrientation.z));

        Quaternion rotationTorso = Quaternion.Euler(0, Angle, 0);
        Quaternion rotationUpperLeg = Quaternion.Euler(20 - controller.UpperLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg, Angle - controller.UpperLegs[LegNumber].jointPosition[1] * Mathf.Rad2Deg, 0);
        Quaternion rotationUnderLeg = Quaternion.Euler(20 - controller.UpperLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg + 45 + controller.UnderLegs[LegNumber].jointPosition[0] * Mathf.Rad2Deg, Angle - controller.UpperLegs[LegNumber].jointPosition[1] * Mathf.Rad2Deg, 0);

        Matrix4x4 m1 = Matrix4x4.Rotate(rotationTorso);
        Matrix4x4 m2 = Matrix4x4.Rotate(rotationUpperLeg);
        Matrix4x4 m3 = Matrix4x4.Rotate(rotationUnderLeg);

        // Vector3 EstimatedPosition = m1.MultiplyPoint3x4(TorsoToLeg);
        Vector3 EstimatedPosition =  mB1.MultiplyPoint3x4(m1.MultiplyPoint3x4(TorsoToLeg)) + mB1.MultiplyPoint3x4(m2.MultiplyPoint3x4(UpperLegToKnee)) + mB1.MultiplyPoint3x4(m3.MultiplyPoint3x4(KneeToFoot));

        return EstimatedPosition;
    }


    public void TriggerDamage()
    {
        controller.DamageHappend();
    }
}