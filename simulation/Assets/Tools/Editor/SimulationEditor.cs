using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

public class SimulationEditor : EditorWindow
{
    bool AllowRecordingData; // Unused in practice
    float timeSliderValue;
    bool timeControlActive;

    [MenuItem("Simulation Tools/Simulation Editor")]
    static void Init()
    {
        SimulationEditor window = (SimulationEditor)EditorWindow.GetWindow(typeof(SimulationEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Timescale Controls", EditorStyles.largeLabel);

        GUILayout.BeginHorizontal("box");

        if(GUILayout.Button("Pause timescale"))
        {
            Time.timeScale = 0;
            timeControlActive = false;
        }

        if (GUILayout.Button("Start timescale"))
        {
            Time.timeScale = timeSliderValue;
            timeControlActive = true;
        }

        if(Time.timeScale == 0)
        {
            timeControlActive = false;
        }

        GUILayout.Toggle(timeControlActive, "time active");

        GUILayout.TextField(Time.timeScale.ToString());

        GUILayout.EndHorizontal();

        // ********************************************************************************

        GUILayout.BeginHorizontal("box");

        timeSliderValue = GUILayout.HorizontalScrollbar(timeSliderValue, 0.0f, 0.0f, 1.0f);

        GUILayout.TextField(timeSliderValue.ToString());

        GUILayout.EndHorizontal();

        // ********************************************************************************

        GUILayout.Label("Data Aquacition Controls", EditorStyles.largeLabel);

        GUILayout.BeginHorizontal("box");

        if(GUILayout.Button("Toggle Record Data"))
        {
            //DataAnalyser.AllowDataRecording = !DataAnalyser.AllowDataRecording;
            AllowRecordingData = !AllowRecordingData;
        }

        GUILayout.Toggle(AllowRecordingData, "Activate Record Data");

        if (GUILayout.Button("Save data"))
        {
            DataAnalyser.CloseAllFiles();
            DataAnalyser.ToggleRecording();
            Debug.Log("Saved recorded data");
        }

        GUILayout.EndHorizontal();
    }
}
