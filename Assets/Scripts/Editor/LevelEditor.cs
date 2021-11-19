using System;
using System.IO;
using UnityEditor;
using UnityEngine;

//TextEditorWindow
public class LevelEditor : EditorWindow
{
    public string description = ";x = wall tile\n" +
        ";p = player start (facing forward z by default)\n" +
        ";o = empty tile\n";
    string stringToEdit =
        "xxoxx\n" +
        "xooox\n" +
        "oopoo\n" +
        "xooox\n" +
        "xxoxx";
    string path = Application.streamingAssetsPath;
    string fileName = string.Empty;
    string errorMessage = string.Empty;

    string FilePath => path + "/" + fileName + ".txt";
    string RelativePath => "Assets/StreamingAssets/Levels/" + fileName + ".txt";
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/LevelEditor Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.Show();

    }
    void WriteFile()
    {
        StreamWriter writer;
        if (!File.Exists(FilePath))
            writer = File.CreateText(FilePath);
        else
            writer = new StreamWriter(FilePath, false);

        writer.WriteLine(stringToEdit);
        writer.Close();

        AssetDatabase.ImportAsset(RelativePath);
    }

    void OnGUI()
    {
        fileName = EditorGUILayout.TextField("File Name", fileName);
        GUILayout.Label("File Content", EditorStyles.boldLabel);
        stringToEdit = EditorGUILayout.TextArea(stringToEdit);
        if (GUILayout.Button("Save File"))
        {
            if (fileName != string.Empty)
            {
                string[] lines = stringToEdit.Split(new string[] { Environment.NewLine, "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    errorMessage = "Content must not be empty";
                }
                else
                {
                    string prevLine = lines[0];
                    bool isValid = true;
                    foreach (string line in lines)
                    {
                        if (line.Length != prevLine.Length)
                        {
                            errorMessage = "Lines must be even length";
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        errorMessage = string.Empty;
                        WriteFile();
                    }
                }
            }
            else
            {
                errorMessage = "FILE NAME MUST NOT BE EMPTY";
            }
        }
        GUILayout.Label(description, EditorStyles.boldLabel);
        GUILayout.Label(errorMessage, EditorStyles.boldLabel);
    }

}