using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public Distraction enemyPrefab;

    public Sprite[] sprites;

    private int enemiesLeft;
    private Action OnWaveEnd;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnEnemy()
    {
        Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];

        float spawnX = Random.Range(-16, -10.5f);
        float duration = Random.Range(3, 10);

        var newEnemy = Instantiate(enemyPrefab);
        newEnemy.Initialize(randomSprite, duration, new Vector2(spawnX, -2));
    }

    public void StartWave(int spawns, Action onEnd = null)
    {
        StartCoroutine(SpawnWave(spawns));
        OnWaveEnd = onEnd;

        enemiesLeft = spawns;
    }

    private IEnumerator SpawnWave(int spawns)
    {
        int spawned = 0;
        while (spawned < spawns)
        {
            spawned++;
            SpawnEnemy();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    public void ReportDeadEnemy()
    {
        enemiesLeft--;
        if (enemiesLeft <= 0)
        {
            OnWaveEnd?.Invoke();
            OnWaveEnd = null;
        }
    }
}
