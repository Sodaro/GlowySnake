using SnakeUtilities;
using UnityEngine;
using System.IO;
using System;

public class SnakeGrid : MonoBehaviour
{
    [SerializeField] private SnakeHead playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject applePrefab;

    //Make Grid Singleton
    private static SnakeGrid _instance;
    public static SnakeGrid Instance => _instance;

    private int rows = 10;
    private int columns = 10;

    private float initialOffset = 5;

    private Vector3[,] gridPositions;
    
    private GridTile[,] tiles;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void GenerateLevelFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"COULD NOT FIND FILE AT PATH: {path}");
            Application.Quit();
            return;
        }
        if (gridPositions != null)
        {
            Array.Clear(gridPositions, 0, gridPositions.Length);
        }
        if (tiles != null)
        {
            foreach(GridTile tile in tiles)
            {
                if (tile.content != null)
                {
                    Destroy(tile.content);
                }
            }
            Array.Clear(tiles, 0, tiles.Length);
        }
        

        string[] lines = File.ReadAllLines(path);
        rows = lines.Length;
        columns = lines[0].Length;
        initialOffset = rows / 2;

        //grid lines is 1 greater than the tiles centered between lines
        gridPositions = new Vector3[rows + 1, columns + 1];
        tiles = new GridTile[rows, columns];
        for (int row = 0; row < rows + 1; row++)
        {
            for (int col = 0; col < columns + 1; col++)
            {
                gridPositions[row, col] = new Vector3(col - initialOffset, 0, initialOffset - row);
                if (row < rows && col < columns)
                {
                    //row 0 col 0 should be top left -> x value should be smallest, z value largest
                    //row 5 col 5 should be bot right -> x value should be largest, z value smallest
                    float zPos = initialOffset - row - 0.5f;
                    float xPos = -(initialOffset - col - 0.5f);
                    Vector3 tilePos = new Vector3(xPos, 0.5f, zPos);
                    GameObject instance = null;
                    ContentType contentType = ContentType.NONE;
                    switch (lines[row][col])
                    {
                        case 'x':
                            instance = Instantiate(wallPrefab, tilePos, Quaternion.identity);
                            contentType = ContentType.WALL;
                            break;
                        case 'a':
                            instance = Instantiate(applePrefab, tilePos, Quaternion.identity);
                            contentType = ContentType.APPLE;
                            break;
                        case 'p':
                            instance = Instantiate(playerPrefab.gameObject, tilePos, Quaternion.identity);
                            contentType = ContentType.SNAKE;
                            break;
                        default:
                            break;
                    }

                    GridTile tile = new GridTile(tilePos, contentType, instance);
                    tiles[row, col] = tile;
                }
            }
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public Vector3 GetPositionOnGridInDirection(Vector3 position, Direction direction)
    {
        //get the row and column indices of the snake on the grid
        GetTileFromPosition(position, out int row, out int col);
        switch(direction)
        {
            case Direction.UP:
                row -= 1;
                break;
            case Direction.DOWN:
                row += 1;
                break;
            case Direction.LEFT:
                col -= 1;
                break;
            case Direction.RIGHT:
                col += 1;
                break;
        }

        //wrap the index around if player leaves grid on sides
        if (row < 0)
            row = tiles.GetLength(0) - 1;
        else if (row >= tiles.GetLength(0))
            row = 0;

        if (col < 0)
            col = tiles.GetLength(1) - 1;
        else if (col >= tiles.GetLength(1))
            col = 0;

        return tiles[row, col].position;
    }

    public Vector3? GetRandomFreeTilePosition(out int row, out int col)
    {
        //in case we couldn't find a free tile in 1000 iterations, we return null and don't spawn apple this time
        int maxLoopCount = 1000;
        int i = 0;
        do
        {
            row = UnityEngine.Random.Range(0, tiles.GetLength(0));
            col = UnityEngine.Random.Range(0, tiles.GetLength(1));
            i++;
        } while (tiles[row, col].contentType != ContentType.NONE && i < maxLoopCount);

        return tiles[row, col].position;
    }

    private GridTile? GetTileFromPosition(Vector3 position, out int row, out int col)
    {
        row = BinarySearchZPosition(position.z, 0, rows);
        col = BinarySearchXPosition(position.x, 0, columns);
        if (row == -1 || col == -1)
            return null;

        return tiles[row, col];
    }

    public void SetTileContent(ContentType contentType, GameObject content, int row, int col)
    {
        if (row < 0 || row > tiles.GetLength(0) || col < 0 || col > tiles.GetLength(1))
            return;

        tiles[row, col].contentType = contentType;
        tiles[row, col].content = content;
    }

    public void SetTileContent(Vector3 position, ContentType contentType)
    {
        int rowIndex = BinarySearchZPosition(position.z, 0, rows-1);
        int colIndex = BinarySearchXPosition(position.x, 0, columns-1);
        if (rowIndex == -1 || colIndex == -1)
            return;

        tiles[rowIndex, colIndex].contentType = contentType;
    }

    public void ClearTile(Vector3 position)
    {
        int rowIndex = BinarySearchZPosition(position.z, 0, rows - 1);
        int colIndex = BinarySearchXPosition(position.x, 0, columns - 1);
        if (rowIndex == -1 || colIndex == -1)
            return;

        tiles[rowIndex, colIndex].contentType = ContentType.NONE;
        tiles[rowIndex, colIndex].content = null;
    }

    private int BinarySearchZPosition(float zPos, int left, int right)
    {
        //index[0,0] has the highest z value and lowest x value
        if (left < right)
        {
            int mid = (left + right) / 2;
            if (Mathf.Approximately(zPos, tiles[mid,0].position.z))
            {
                return mid;
            }
            else if (zPos > tiles[mid, 0].position.z)
            {
                return BinarySearchZPosition(zPos, left, mid - 1);
            }
            else
            {
                return BinarySearchZPosition(zPos, mid + 1, right);
            }
        }

        if (Mathf.Approximately(tiles[left,0].position.z, zPos))
            return left;
        else
            return -1;

    }

    private int BinarySearchXPosition(float xPos, int left, int right)
    {
        //index[length-1,length-1] has the highest x value and lowest z value
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

    public bool HandleHeadCollisions(SnakeHead snake)
    {
        GridTile? nullabletile = GetTileFromPosition(snake.transform.position, out int row, out int col);
        if (nullabletile == null)
            return false;

        GridTile tile = (GridTile)nullabletile;

        if (tile.contentType == ContentType.WALL || tile.contentType == ContentType.SNAKE)
            return true;

        if (tile.contentType == ContentType.APPLE)
        {
            tile.contentType = ContentType.NONE;
            snake.CreateSegment();
            Destroy(tile.content);
            tile.content = null;
            tiles[row, col] = tile;
        }

        return false;
    }

    //Draw the gridlines and x in center positions
    public void OnRenderObject()
    {
        CreateLineMaterial();
        if (gridPositions == null)
            return;

        if (gridPositions.Length == 0)
            return;
        GL.PushMatrix();
        lineMaterial.SetPass(0);
        //GL.LoadOrtho();
        //GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.QUADS);
        GL.Color(Color.white);

        DrawGridLines();
        DrawTileCenter();

        GL.End();
        GL.PopMatrix();

        void DrawGridLines()
        {
            float endPosX = gridPositions[0, gridPositions.GetLength(1) - 1].x;
            float endPosZ = gridPositions[gridPositions.GetLength(0) - 1, 0].z;
            for (int row = 0; row < gridPositions.GetLength(0); row++)
            {
                Vector3 pos1 = gridPositions[row, 0];
                GL.Vertex(new Vector3(pos1.x, pos1.y, pos1.z - 0.05f));
                GL.Vertex(new Vector3(pos1.x, pos1.y, pos1.z + 0.05f));
                GL.Vertex(new Vector3(endPosX, pos1.y, pos1.z + 0.05f));
                GL.Vertex(new Vector3(endPosX, pos1.y, pos1.z - 0.05f));

            }
            for (int x = 0; x < gridPositions.GetLength(1); x++)
            {
                Vector3 pos1 = gridPositions[0, x];
                GL.Vertex(new Vector3(pos1.x - 0.05f, pos1.y, pos1.z));
                GL.Vertex(new Vector3(pos1.x - 0.05f, pos1.y, endPosZ));
                GL.Vertex(new Vector3(pos1.x + 0.05f, pos1.y, endPosZ));
                GL.Vertex(new Vector3(pos1.x + 0.05f, pos1.y, pos1.z));
            }
        }

        void DrawTileCenter()
        {
            for (int row = 0; row < tiles.GetLength(0); row++)
            {
                for (int col = 0; col < tiles.GetLength(1); col++)
                {
                    Color color = tiles[row, col].contentType != ContentType.NONE ? Color.red : Color.green;
                    GL.Color(color);
                    GL.Vertex(tiles[row, col].position + new Vector3(0, 0, 0.1f));
                    GL.Vertex(tiles[row, col].position + new Vector3(0.1f, 0, 0f));
                    GL.Vertex(tiles[row, col].position + new Vector3(0f, 0, -0.1f));
                    GL.Vertex(tiles[row, col].position + new Vector3(-0.1f, 0, 0f));
                }
            }
        }
    }
}
