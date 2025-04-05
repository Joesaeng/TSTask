
using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] zombiePrefabs;
    [SerializeField] Transform[] zombieSpawnPoints;

    HashSet<Monster> monsters = new();

    Queue<Action> killQ = new();

    private float spawnTimeCapture;

    private float spawnDelay;
    public float SpawnDelay
    {
        get => spawnDelay;
        set
        {
            spawnDelay = Mathf.Max(0.15f, value);
        }
    }

    private void Update()
    {
        while (killQ.Count > 0)
            killQ.Dequeue().Invoke();

        if (spawnTimeCapture <= Time.time - SpawnDelay)
        {
            spawnTimeCapture = Time.time;
            SpawnDelay -= 0.05f;
            SpawnMonster();
        }
    }

    public void Init(float firstSpawnDelay)
    {
        spawnTimeCapture = Time.time;
        spawnDelay = firstSpawnDelay;
    }

    public void ClearZombies()
    {
        foreach (var zombie in monsters)
        {
            KillMonster(zombie);
        }
        while (killQ.Count > 0)
            killQ.Dequeue().Invoke();
    }

    public void EnqueueKill(Action action)
    {
        killQ.Enqueue(action);
    }

    public void KillMonster(Monster monster)
    {
        EnqueueKill(() =>
        {
            monster.Clear();
            monsters.Remove(monster);
            ObjectManager.Ins.Kill(monster.gameObject);
        });
    }

    private void SpawnMonster()
    {
        int randZombieIndex = UnityEngine.Random.Range(0,zombiePrefabs.Length);

        var newMonsterSetter = NewMonsterSetter(out var randInt);
        var obj = ObjectManager.Ins.Spawn(zombiePrefabs[randZombieIndex], zombieSpawnPoints[randInt].position,Quaternion.identity);

        var comp = obj.GetComponent<MeleeZombie>();
        comp.Init(newMonsterSetter);

        monsters.Add(comp);
    }

    private MonsterSetter NewMonsterSetter(out int randInt)
    {
        randInt = UnityEngine.Random.Range(0, ReadonlyDatas.RoadCount);
        LayerMask monsterLayer = randInt switch
        {
            0 => LayerMask.GetMask(ReadonlyDatas.Zombie_0_Layer_String),
            1 => LayerMask.GetMask(ReadonlyDatas.Zombie_1_Layer_String),
            2 => LayerMask.GetMask(ReadonlyDatas.Zombie_2_Layer_String),
            _=> LayerMask.GetMask(ReadonlyDatas.Zombie_0_Layer_String)
        };
        LayerMask roadLayer = randInt switch
        {
            0 => LayerMask.GetMask(ReadonlyDatas.Road_0_Layer_String),
            1 => LayerMask.GetMask(ReadonlyDatas.Road_1_Layer_String),
            2 => LayerMask.GetMask(ReadonlyDatas.Road_2_Layer_String),
            _=> LayerMask.GetMask(ReadonlyDatas.Road_0_Layer_String)
        };

        int sortingOrder = randInt switch
        {
            0 => ReadonlyDatas.Zombie_0_SortingOrder,
            1 => ReadonlyDatas.Zombie_1_SortingOrder,
            2 => ReadonlyDatas.Zombie_2_SortingOrder,
            _=> 0,
        };

        MonsterSetter setter = new()
        {
            monsterLayer = monsterLayer,
            roadLayer = roadLayer,
            playerLayer = LayerMask.GetMask(ReadonlyDatas.Player_Layer_String),
            sortingOrder = sortingOrder,
        };

        return setter;
    }
}
