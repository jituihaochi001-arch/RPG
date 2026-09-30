using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LootPool : MonoBehaviour
{
    public static LootPool Instance;

    private ObjectPool<GameObject> lootPool;

    [Header("Pool Settings")]
    [SerializeField] private GameObject lootPrefab;           // 掉落物预制体
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 30;

    [Header("Drop Settings")]
    [SerializeField] private List<LootDropChance> dropTable = new List<LootDropChance>();  // 掉落表
    [SerializeField] private int maxLootInScene = 15;        // 场景中最大掉落物数量

    [SerializeField] private Vector2 spawnRangeX = new Vector2(-8f, 35f);
    [SerializeField] private Vector2 spawnRangeY = new Vector2(-8f, 4f);

    private int activeLootCount = 0; //当前场景中掉落物数量

    public float lootInterval = 15f;
    private float spawnTimer;

    private void Awake()
    {
        Instance = this;

        lootPool = new ObjectPool<GameObject>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, defaultCapacity, maxSize);
    }
    //对象池方法
    GameObject createFunc()
    {
        return Instantiate(lootPrefab, transform);
    }
    void actionOnGet(GameObject loot)
    {
        loot.SetActive(true);
        activeLootCount++;
    }
    void actionOnRelease(GameObject loot)
    {
        loot.SetActive(false);
        activeLootCount--;
    }
    void actionOnDestroy(GameObject loot)
    {
        Destroy(loot);
    }
    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= lootInterval && activeLootCount < maxLootInScene)
        {
            spawnTimer -= lootInterval;
            SpawnLootAtRandomPosition();
        }
    }

    // 在随机位置生成掉落物
    public void SpawnLootAtRandomPosition()
    {
        if (activeLootCount >= maxLootInScene) return;

        float x = Random.Range(spawnRangeX.x, spawnRangeX.y);
        float y = Random.Range(spawnRangeY.x, spawnRangeY.y);
        Vector3 position = new Vector3(x, y, 0f);

        SpawnLoot(position);
    }
    // ===== 生成掉落物 =====
    public void SpawnLoot(Vector3 position)
    {
        if (activeLootCount >= maxLootInScene) return;

        GameObject lootObj = lootPool.Get();
        lootObj.transform.position = position;

        // 随机获取掉落物品
        ItemSO randomItem = GetRandomLootItem();
        if (randomItem != null)
        {
            Loot lootComponent = lootObj.GetComponent<Loot>();
            if (lootComponent != null)
            {
                lootComponent.Instantiate(randomItem, 1);
            }
        }
    }
    // ===== 根据掉落表随机获取物品 =====
    private ItemSO GetRandomLootItem()
    {
        if (dropTable.Count == 0) return null;

        // 计算总权重
        float totalWeight = 0f;
        foreach (var drop in dropTable)
        {
            totalWeight += drop.dropChance;
        }

        // 随机选择
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var drop in dropTable)
        {
            cumulative += drop.dropChance;
            if (randomValue <= cumulative)
            {
                return drop.itemSO;
            }
        }

        return dropTable[dropTable.Count - 1].itemSO;
    }
    // ===== 放回池中 =====
    public void ReleaseLoot(GameObject loot)
    {
        lootPool.Release(loot);
    }
}

// ===== 掉落表条目 =====
[System.Serializable]
public class LootDropChance
{
    public ItemSO itemSO;
    [Range(0f, 100f)]
    public float dropChance = 10f;  // 掉落权重
}