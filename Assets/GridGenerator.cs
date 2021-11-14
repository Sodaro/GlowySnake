using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject apple;

    [SerializeField]
    private int rows = 10;
    [SerializeField]
    private int columns = 10;


    private float initialOffset = 5;

    private float tile_max_x = 0;
    private float tile_max_z = 0;
    private float tile_min_x = 0;
    private float tile_min_z = 0;

    private Vector3[,] gridPositions;
    
    private GridTile[,] tiles;

    public enum Direction{UP = 0, RIGHT = 90, DOWN = 180, LEFT = 270};
    struct GridTile
    {
        public Vector3 position;
        public GameObject content;
    }

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
    private void Awake()
    {
        gridPositions = new Vector3[rows+1, columns+1];
        tiles = new GridTile[rows, columns];
        for (int y = 0; y < rows+1; y++)
        {
            for (int x = 0; x < columns+1; x++)
            {
                gridPositions[y, x] = new Vector3(x - initialOffset, 0, y - initialOffset);

                if (y < rows && x < columns)
                {
                    GridTile tile = new GridTile();

                    Vector3 tilePos = new Vector3(x + 0.5f - initialOffset, 0.5f, y + 0.5f - initialOffset);

                    tile.position = tilePos;
                    tile.content = null;

                    tiles[y, x] = tile;
                }
            }
        }

        tile_min_x = -initialOffset + 0.5f;
        tile_min_z = -initialOffset + 0.5f;

        tile_max_x = tile_min_x + tiles.GetLength(0)-1;
        tile_max_z = tile_min_z + tiles.GetLength(0)-1;
    }

    public Vector3 GetPositionOnGridInDirection(Vector3 position, Direction direction)
    {
        GetTile(position, out int row, out int col);
        switch(direction)
        {
            case Direction.UP:
                row += 1;
                break;
            case Direction.DOWN:
                row -= 1;
                break;
            case Direction.LEFT:
                col -= 1;
                break;
            case Direction.RIGHT:
                col += 1;
                break;
        }

        if (row < 0)
            row = tiles.GetLength(0) - 1;
        if (row >= tiles.GetLength(0))
            row = 0;

        if (col < 0)
            col = tiles.GetLength(1) - 1;
        if (col >= tiles.GetLength(1))
            col = 0;

        return tiles[row, col].position;
    }

    public bool IsPointInsideBounds(Vector3 position)
    {
        return (position.x <= tile_max_x && position.x >= tile_min_x) && (position.z <= tile_max_z && position.z >= tile_min_z);
    }

    private GridTile GetRandomTile(out int row, out int col)
    {
        do
        {
            row = UnityEngine.Random.Range(0, tiles.GetLength(0));
            col = UnityEngine.Random.Range(0, tiles.GetLength(0));
        } while (tiles[row, col].content != null);

        return tiles[row, col];
    }

    private GridTile? GetTile(Vector3 position, out int row, out int col)
    {
        row = BinarySearchZPosition(position.z, 0, rows - 1);
        col = BinarySearchXPosition(position.x, 0, columns - 1);
        if (row == -1 || col == -1)
            return null;

        return tiles[row, col];
    }

    public void SetContent(Vector3 position, GameObject gameObj)
    {
        int rowIndex = BinarySearchZPosition(position.z, 0, rows-1);
        int colIndex = BinarySearchXPosition(position.x, 0, columns-1);
        if (rowIndex == -1 || colIndex == -1)
            return;

        tiles[rowIndex, colIndex].content = gameObj;
    }

    public void ClearTile(Vector3 position)
    {
        int rowIndex = BinarySearchZPosition(position.z, 0, rows - 1);
        int colIndex = BinarySearchXPosition(position.x, 0, columns - 1);
        if (rowIndex == -1 || colIndex == -1)
            return;

        tiles[rowIndex, colIndex].content = null;
    }

    private int BinarySearchZPosition(float zPos, int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            if (Mathf.Approximately(zPos, tiles[mid,0].position.z))
            {
                return mid;
            }
            else if (zPos > tiles[mid, 0].position.z)
            {
                return BinarySearchZPosition(zPos, mid + 1, right);
            }
            else
            {
                return BinarySearchZPosition(zPos, left, mid - 1);
            }
        }

        if (Mathf.Approximately(tiles[left,0].position.z, zPos))
            return left;
        else
            return -1;

    }

    private int BinarySearchXPosition(float xPos, int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            if (Mathf.Approximately(xPos, tiles[0, mid].position.x))
            {
                return mid;
            }
            else if (xPos > tiles[0, mid].position.x)
            {
                return BinarySearchXPosition(xPos, mid + 1, right);
            }
            else
            {
                return BinarySearchXPosition(xPos, left, mid - 1);
            }
        }

        if (Mathf.Approximately(tiles[0, left].position.x, xPos))
            return left;
        else
            return -1;

    }

    private void Start()
    {
        StartCoroutine(SpawnApples());
    }

    IEnumerator SpawnApples()
    {
        while (true)
        {
            GridTile tile = GetRandomTile(out int row, out int col);
            GameObject instance = Instantiate(apple, tile.position, Quaternion.identity);
            tile.content = instance;
            tiles[row, col] = tile;
            yield return new WaitForSeconds(2.5f);
        }
    }

    public bool HandleHeadCollisions(Snake snake)
    {
        GridTile? nullabletile = GetTile(snake.transform.position, out int row, out int col);
        if (nullabletile == null)
            return false;

        GridTile tile = (GridTile)nullabletile;
        if (tile.content == null)
            return false;

        if (tile.content.TryGetComponent(out BodyPart part))
        {
            return true;
        }
        else
        {
            Destroy(tile.content);
            tile.content = null;
            tiles[row, col] = tile;
            snake.CreateSegment();
            return false;
        }
    }

    //Draw the grid and center positions
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        GL.Color(Color.white);
        for (int y = 0; y < gridPositions.GetLength(0); y++)
        {
            GL.Vertex(gridPositions[y, 0]);
            GL.Vertex(gridPositions[y, gridPositions.GetLength(0) - 1]);
        }
        for (int x = 0; x < gridPositions.GetLength(0); x++)
        {
            GL.Vertex(gridPositions[0, x]);
            GL.Vertex(gridPositions[gridPositions.GetLength(0) - 1, x]);
        }

        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                Color color = tiles[y, x].content != null ? Color.red : Color.green;
                GL.Color(color);

                GL.Vertex(tiles[y, x].position - new Vector3(0.1f, 0, 0f));
                GL.Vertex(tiles[y, x].position + new Vector3(0.1f, 0, 0f));

                GL.Vertex(tiles[y, x].position - new Vector3(0f, 0, 0.1f));
                GL.Vertex(tiles[y, x].position + new Vector3(0f, 0, 0.1f));
            }
        }

        GL.End();

        GL.PopMatrix();
    }
}
