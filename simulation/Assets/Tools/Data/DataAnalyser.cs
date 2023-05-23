using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.IO;
using System;
using System.Text;
using UnityEditorInternal;

public static class DataAnalyser
{
    static private bool AllowDataRecording = true;

    static List<string> nameRegistry = new List<string>();
    static List<StreamWriter> fileRegistry = new List<StreamWriter>();

    static public void ToggleRecording()
    {
        AllowDataRecording = !AllowDataRecording;
    }

    static public void OpenFile(string fileName)
    {
        foreach(string name in nameRegistry)
        {
            if(name == fileName)
            {
                Debug.Log("File with name " + fileName + " has already been oppened this loop.");
                return;
            }
        }

        StreamWriter file = new StreamWriter("GeneratedData/" + fileName + ".csv");
        nameRegistry.Add(fileName);
        fileRegistry.Add(file);
    }

    static public void FileWriteLine(string fileName, double data)
    {
        // Debug check
        if(nameRegistry.Count != fileRegistry.Count)
        {
            Debug.Log("Name and file registry are not of equal size, this will cause errors.");
        }

        for(int i = 0; i < fileRegistry.Count; i++)
        {
            if(fileName == nameRegistry[i])
            {
                fileRegistry[i].WriteLine(data.ToString().Replace(",","."));
                return;
            }
        }

        Debug.Log("Did not find a matching file name to write line");
    }

    static public void CloseAllFiles()
    {
        foreach(string fileName in nameRegistry)
        {
            CloseFile(fileName);
        }
    }

    static public void CloseFile(string fileName)
    {
        for(int i = 0; i < fileRegistry.Count; i++)
        {
            if(fileName == nameRegistry[i])
            {
                fileRegistry[i].Close();
            }
        }
    }
}
