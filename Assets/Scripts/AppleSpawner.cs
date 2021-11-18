using System.Collections;
using UnityEngine;
using SnakeUtilities;

public class AppleSpawner : MonoBehaviour
{
    [SerializeField] GameObject applePrefab;
    [SerializeField] SnakeGrid grid; 
    private void Start()
    {
        StartCoroutine(SpawnApples());
    }

    IEnumerator SpawnApples()
    {
        while (true)
        {
            Vector3? pos = grid.GetRandomFreeTilePosition(out int row, out int col);
            if (pos != null)
            {
                GameObject instance = Instantiate(applePrefab, (Vector3)pos, Quaternion.identity);
                grid.SetTileContent(ContentType.APPLE, instance, row, col);
            }
            yield return new WaitForSeconds(2.5f);
        }
    }

}
