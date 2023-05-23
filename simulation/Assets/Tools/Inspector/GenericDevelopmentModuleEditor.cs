using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenericDevelopmentModuleManager))]
public class GenericDevelopmentModuleEditor : Editor
{
    bool ShowTrainingDebugInformation = false;

    public enum ControlType
    {
        StateMachine,
        NetworkBased
    }

    private void OnEnable()
    {

    }

    int tab;
    public override void OnInspectorGUI()
    {
        GenericDevelopmentModuleManager GDM = (GenericDevelopmentModuleManager)target;

        //EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GenericDevelopmentModuleManager)target), typeof(GenericDevelopmentModuleManager), true);

        using (new EditorGUI.DisabledScope(true))
             EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false); // This line comes from: https://answers.unity.com/questions/1223009/how-to-show-the-standard-script-line-with-a-custom.html

        tab = GUILayout.Toolbar (tab, new string[] {"Modes", "Terrain", "Variables"});

        switch(tab)
        {
            case 0:
                ModeInspector(GDM);
                break;

            case 1:
                TerrainInspector(GDM);
                break;

            case 2:
                VariablesInspector(GDM);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ModeInspector(GenericDevelopmentModuleManager GDM)
    {       
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentMode") );

        switch( GDM.currentMode )
        {
            case SimulationMode.Training:


                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentRobotTrain") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTrainingType") );

                switch((int)GDM.currentTrainingType)
                {
                    case 0: // suprevised

                        GUILayout.Label("\nNot working in this scene", EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("SaveParameterNetwork") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ParameterNetworkName") );

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("SuprevisedNetworkTopology") );

                        if(GUILayout.Button("Test network"))
                        {
                            GDM.TestSuprevisedNetwork();
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("TestInput") );

                        // using (new EditorGUI.DisabledScope(true))
                        // EditorGUILayout.PropertyField(serializedObject.FindProperty("TestOutput") );

                        break;

                    case 1: // Genetic Algorithm

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("TrainingNetworkTopology") );

                        GUILayout.Label("\nContinue Training on a previous network", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadNetwork") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadedNetworkName") );

                        GUILayout.Label("\nHow to save the best network from training", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("SaveBestNetworkLastGen") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("BestNetworkLastGenName") );

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("SaveBestNetworkOverall") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("BestNetworkOverallName") );

                        GUILayout.Label("\nGenetic Algorithm Settings", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("populationSize") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("EvaluationTime") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("MutationChance") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("MutationEffect") );

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseGenerationCriteria") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("GenerationCriteria") );
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseFitnessCriteria") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("FitnessCriteria") );
                        GUILayout.EndHorizontal();

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RandomInstanstiationRotation") );

                        // Time scale controls
                        GDM.UpdateCurrentTimeScale();

                        GUILayout.Label("\nTime scale contol", EditorStyles.boldLabel);
                        using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTimeScale") );
                        
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredTimeScale") );
                        if(GUILayout.Button("Set time scale"))
                        {
                            GDM.SetTimeScale();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Label("\nRecord Training Data", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RecordTrainingData") );

                        // Debug information
                        ShowTrainingDebugInformation = EditorGUILayout.BeginFoldoutHeaderGroup(ShowTrainingDebugInformation, "Show debug information");
                        EditorGUILayout.EndFoldoutHeaderGroup();

                        if(ShowTrainingDebugInformation)
                        {
                            using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGenerationNumber") );

                            using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("population") );

                            using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("DebugAllFitness") );

                            using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("DebugAverageFitness") );

                            using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("DebugBestFitnessOverall") );
                        }

                        break;
                }



                break;

            case SimulationMode.Demonstration:

                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentRobotSim") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentControlType") );

                switch((int)GDM.currentControlType)
                {
                    case 0: // StateMachine based control
                        break;

                    case 1: // Braintenberg
                        break;

                    case 2: // Closed loop control
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadNetwork") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("CreateRandomNetwork") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadedNetworkName") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("DemonstrationNetworkTopology") );
                        break;

                    case 3: // Network Based Control
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadNetwork") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("CreateRandomNetwork") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadedNetworkName") );
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("DemonstrationNetworkTopology") );
                        break;
                }

                //GUILayout.Label("Choose neural network related settings");

                //GUILayout.Label("Demonstration run time settings");
                
                // Time scale controls
                
                GDM.UpdateCurrentTimeScale();
                GUILayout.Label("\nTime scale contol", EditorStyles.boldLabel);
                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTimeScale") );
                
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredTimeScale") );
                if(GUILayout.Button("Set time scale"))
                {
                    GDM.SetTimeScale();
                }
                GUILayout.EndHorizontal();
                
                /*
                GDM.UpdateCurrentTimeScale();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredTimeScale") );
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTimeScale") );

                if(GUILayout.Button("Set time scale"))
                {
                    GDM.SetTimeScale();
                }
                GUILayout.EndHorizontal();
                */

                GUILayout.Label("\nRobot settings", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RandomInstanstiationRotation") );

                if(GUILayout.Button("Reset Robots"))
                {
                    GDM.ResetRobots(true); // This is not ideal
                }
                GUILayout.EndHorizontal();


                GUILayout.Label("\nData Recording Settings", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RecordData") );

                if(GUILayout.Button("Save Recording"))
                {
                    GDM.EndRecording();
                }
                GUILayout.EndHorizontal();

                break;
        
            case SimulationMode.Parameters:

                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentRobotSim") );

                GUILayout.Label("\nParameter Testing Configuration", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LowerVerticalStep") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UpperVerticalStep") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VerticalStepDelta") );

                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentVerticalStep") );

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LowerHorizontalStep") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UpperHorizontalStep") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalStepDelta") );

                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentHorizontalStep") );

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LowerSpeed") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UpperSpeed") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SpeedDelta") );

                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentSpeed") );

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LowerHeight") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UpperHeight") );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HeightDelta") );

                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentHeight") );

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ParameterEvaluationTime") );

                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ParameterTimePassed") );


                GDM.UpdateCurrentTimeScale();
                GUILayout.Label("\nTime scale contol", EditorStyles.boldLabel);
                using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTimeScale") );
                
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("desiredTimeScale") );
                if(GUILayout.Button("Set time scale"))
                {
                    GDM.SetTimeScale();
                }
                GUILayout.EndHorizontal();

                GUILayout.Label("\nData Recording Settings", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RecordData") );

                if(GUILayout.Button("Save Recording"))
                {
                    GDM.EndRecording();
                }
                GUILayout.EndHorizontal();


                break;
        }
    }

    private void TerrainInspector(GenericDevelopmentModuleManager GDM)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTerrainType") );
        
        if(GUILayout.Button("Update Terrain"))
        {
            GDM.ChangeTerrain();
        }
    }

    private void VariablesInspector(GenericDevelopmentModuleManager GDM)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("HexController") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("QuadController") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FlatTerrain") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DifficultTerrain") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RobotCamera") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FlatTargetList") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DifficultTargetList") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TrainingUI") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("GenCounterUI") );
        EditorGUILayout.PropertyField(serializedObject.FindProperty("HexapodAgent") );
    }
}
