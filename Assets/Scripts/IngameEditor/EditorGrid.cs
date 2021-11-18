using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class EditorGrid : MonoBehaviour
{

    //TODO: Move non grid related stuff to LevelEditorUI (such as exit button functionality) etc
    [SerializeField] private TMP_Text gridSizeDisplay;
    [SerializeField] private List<GridObject> gridObjects;
    [SerializeField] private GridObject gridObjectPrefab;
    [SerializeField] Slider sizeSlider;
    [SerializeField] private TMP_InputField fileName;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;

    int sideLength = 3;

    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(GenerateGrid);
        saveButton.onClick.AddListener(WriteLevelToFile);
        exitButton.onClick.AddListener(ExitLevelEditor);
    }
    private void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveListener(GenerateGrid);
        saveButton.onClick.RemoveListener(WriteLevelToFile);
        exitButton.onClick.RemoveListener(ExitLevelEditor);
    }

    private void Start()
    {
        GenerateGrid(sizeSlider.minValue);
    }

    void ExitLevelEditor()
    {
        SceneManager.LoadScene(0);
    }

    void WriteLevelToFile()
    {
        string path = Application.streamingAssetsPath + "/Levels/" + fileName.text + ".txt";
        using (StreamWriter sw = new StreamWriter(path))
        {
            for (int i = 0; i < gridObjects.Count; i++)
            {
                if (i % sideLength == 0 && i > 0)
                {
                    sw.Write(Environment.NewLine);
                }
                char c = ' ';
                switch(gridObjects[i].ContentType)
                {
                    case SnakeUtilities.ContentType.NONE:
                        c = 'o';
                        break;
                    case SnakeUtilities.ContentType.WALL:
                        c = 'x';
                        break;
                    case SnakeUtilities.ContentType.SNAKE:
                        c = 'p';
                        break;
                    case SnakeUtilities.ContentType.APPLE:
                        c = 'a';
                        break;
                }
                sw.Write(c);
            }
        }
    }

    private void GenerateGrid(float size)
    {
        sideLength = (int)size;
        gridSizeDisplay.text = $"{sideLength}x{sideLength}";
        if (gridObjects != null)
        {
            for (int i = gridObjects.Count - 1; i >= 0; i--)
            {
                Destroy(gridObjects[i].gameObject);
            }
            gridObjects.Clear();
            //foreach (Transform child in transform)
            //{
            //    Destroy(child.gameObject);
            //}
            //gridObjects.Clear();
        }
        else
        {
            gridObjects = new List<GridObject>();
        }
        
        int initialOffset = sideLength / 2;

        for (int row = 0; row < sideLength; row++)
        {
            for (int col = 0; col < sideLength; col++)
            {
                int yPos = initialOffset*100 - row * 100;
                int xPos = -initialOffset*100 + col * 100;
                GridObject instance = Instantiate(gridObjectPrefab, transform);
                instance.SetPosition(xPos, yPos);
                gridObjects.Add(instance);
            }
        }
    }
}
