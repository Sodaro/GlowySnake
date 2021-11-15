using System.Collections;
using UnityEngine;
using MyUtilities;

public class AppleSpawner : MonoBehaviour
{
    [SerializeField] GameObject applePrefab;
    [SerializeField] GridGenerator grid; 
    private void Start()
    {
        StartCoroutine(SpawnApples());
    }

    IEnumerator SpawnApples()
    {
        while (true)
        {
            Vector3 pos = grid.GetRandomFreeTilePosition(out int row, out int col);
            GameObject instance = Instantiate(applePrefab, pos, Quaternion.identity);
            grid.SetTileContent(instance, row, col);

            yield return new WaitForSeconds(2.5f);
        }
    }

}
