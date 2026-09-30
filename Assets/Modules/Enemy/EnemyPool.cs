using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    private ObjectPool<GameObject> enemyPool;

    [SerializeField] private GameObject enemyprefab;

    [SerializeField] private int maxEnemies = 3;//当前场景中最多敌人数量
    private int activeEnemyCount = 0;//当前场景中敌人数量

    public float enemyInterval;
    private float spawnTimer;
      
    private void Awake()
    {
        Instance = this;

        enemyPool = new ObjectPool<GameObject>(createFunc,actionOnGet,actionOnRelease, actionOnDestroy,true,5,5);
    }

    GameObject createFunc()
    {
        return Instantiate(enemyprefab, transform);
    }
    void actionOnGet(GameObject enemy)
    {      
       enemy.SetActive(true);
        activeEnemyCount++;
    }
    void actionOnRelease(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemyCount--;
    }
    void actionOnDestroy(GameObject enemy)
    {
        Destroy(enemy);
    }
    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= enemyInterval && activeEnemyCount < maxEnemies)
        {
            spawnTimer -= enemyInterval;
            Spawn();
        }
    }
    //何时生成敌人
    private void Spawn()
    {
        GameObject enemy = enemyPool.Get();
        enemy.transform.position = new Vector3(Random.Range(18,35), Random.Range(-19,-10));
    }
    //敌人死亡后放回池中
    public void Release(GameObject enemy)
    {
        enemyPool.Release(enemy);
    }
}
