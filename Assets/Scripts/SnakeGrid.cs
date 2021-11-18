using SnakeUtilities;
using UnityEngine;
using System.IO;
public class SnakeGrid : MonoBehaviour
{
    //[SerializeField] Material lineMaterial;
    //Make Grid Singleton
    [SerializeField] private SnakeHead playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    private static SnakeGrid _instance;
    public static SnakeGrid Instance => _instance;

    //[SerializeField]
    private int rows = 10;
    //[SerializeField]
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
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        string path = Application.streamingAssetsPath + "/level.txt";
        if (!File.Exists(path))
        {
            Debug.LogError($"COULD NOT FIND FILE AT PATH: {path}");
            Application.Quit();
        }

        string[] lines = File.ReadAllLines(path);
        rows = lines.Length;
        columns = lines[0].Length;

        //grid lines is 1 greater than the tiles centered between lines
        gridPositions = new Vector3[rows + 1, columns + 1];
        tiles = new GridTile[rows, columns];
        for (int y = 0; y < rows + 1; y++)
        {
            for (int x = 0; x < columns + 1; x++)
            {
                gridPositions[y, x] = new Vector3(x - initialOffset, 0, y - initialOffset);
                if (y < rows && x < columns)
                {
                    Vector3 tilePos = new Vector3(x + 0.5f - initialOffset, 0.5f, y + 0.5f - initialOffset);
                    GameObject instance = null;
                    ContentType contentType = ContentType.NONE;
                    switch (lines[y][x])
                    {
                        case 'x':
                            instance = Instantiate(wallPrefab, tilePos, Quaternion.identity);
                            contentType = ContentType.WALL;
                            break;
                        case 'p':
                            instance = Instantiate(playerPrefab.gameObject, tilePos, Quaternion.identity);
                            contentType = ContentType.SNAKE;
                            break;
                        default:
                            break;
                    }
                    
                    GridTile tile = new GridTile(tilePos, contentType, instance);
                    tiles[y, x] = tile;
                }
            }
        }

        
        //foreach (string line in lines)
        //{
        //    for (int i = 0; i < line.Length; i++)
        //    {

        //    }
        //}


    }

    public Vector3 GetPositionOnGridInDirection(Vector3 position, Direction direction)
    {
        //get the row and column indices of the snake on the grid
        GetTileFromPosition(position, out int row, out int col);
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

        //wrap the index around if player leaves grid on sides
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

    public Vector3? GetRandomFreeTilePosition(out int row, out int col)
    {
        //in case we couldn't find a free tile in 1000 iterations, we return null and don't spawn apple this time
        int maxLoopCount = 1000;
        int i = 0;
        do
        {
            row = Random.Range(0, tiles.GetLength(0));
            col = Random.Range(0, tiles.GetLength(1));
            i++;
        } while (tiles[row, col].contentType != ContentType.NONE && i < maxLoopCount);

        return tiles[row, col].position;
    }

    private GridTile? GetTileFromPosition(Vector3 position, out int row, out int col)
    {
        row = BinarySearchZPosition(position.z, 0, rows - 1);
        col = BinarySearchXPosition(position.x, 0, columns - 1);
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
            for (int y = 0; y < gridPositions.GetLength(0); y++)
            {
                Vector3 pos1 = gridPositions[y, 0];
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
            for (int y = 0; y < tiles.GetLength(0); y++)
            {
                for (int x = 0; x < tiles.GetLength(1); x++)
                {
                    Color color = tiles[y, x].contentType != ContentType.NONE ? Color.red : Color.green;
                    GL.Color(color);
                    GL.Vertex(tiles[y, x].position + new Vector3(0, 0, 0.1f));
                    GL.Vertex(tiles[y, x].position + new Vector3(0.1f, 0, 0f));
                    GL.Vertex(tiles[y, x].position + new Vector3(0f, 0, -0.1f));
                    GL.Vertex(tiles[y, x].position + new Vector3(-0.1f, 0, 0f));
                }
            }
        }
    }
}
