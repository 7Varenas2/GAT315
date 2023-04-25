using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject wizardPrefab;
    [SerializeField] public float timeInterval = 5.5f;

    void Start()
    {
        StartCoroutine(spawnEnemy(timeInterval, wizardPrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        // Waits for how long the next enemy will be spawned
        yield return new WaitForSeconds(interval);
        // Spawns the next object at random location
        GameObject newEnemy = Instantiate(enemy, transform.position, transform.rotation);
        StartCoroutine(spawnEnemy(interval, newEnemy));
    }
}
