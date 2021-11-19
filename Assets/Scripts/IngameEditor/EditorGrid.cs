using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorGrid : MonoBehaviour
{
    [SerializeField] private List<GridObject> gridObjects;
    [SerializeField] private GridObject gridObjectPrefab;

    private int sideLength = 3;

    private void Start()
    {
        GenerateGrid(sideLength);
    }

    public string ConvertGridToString()
    {
        string output = string.Empty;
        for (int i = 0; i < gridObjects.Count; i++)
        {
            if (i % sideLength == 0 && i > 0)
            {
                output += Environment.NewLine;
            }
            char c = ' ';
            switch (gridObjects[i].ContentType)
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
            output += c;
        }
        return output;

    }

    public void GenerateGrid(int size)
    {
        sideLength = size;
        if (gridObjects != null)
        {
            for (int i = gridObjects.Count - 1; i >= 0; i--)
            {
                Destroy(gridObjects[i].gameObject);
            }
            gridObjects.Clear();
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
                int yPos = initialOffset * 100 - row * 100;
                int xPos = -initialOffset * 100 + col * 100;
                GridObject instance = Instantiate(gridObjectPrefab, transform);
                instance.SetPosition(xPos, yPos);
                gridObjects.Add(instance);
            }
        }
    }
}
