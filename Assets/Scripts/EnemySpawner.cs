using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxEnemies = 6;

    private int activeEnemies = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (activeEnemies < maxEnemies)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = ObjectPool.Instance.Get(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
        if (patrol != null)
            patrol.OnDeath = OnEnemyDeath;

        activeEnemies++;
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        activeEnemies--;
        ObjectPool.Instance.Return(enemy);
    }
}
