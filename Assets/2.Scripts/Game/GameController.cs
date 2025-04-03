using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterConfig
{
    public int maxHp;
    public float frontRayLength;
    public float upRayLength;
    public float downRayLength;
    public float climbRayLength;
    public float jumpDelay;
    public float jumpPower;
    public float climbAccelation;
    public float moveSpeed;
    public float moveAccelation;
    public float pushedMass;
    public float pushedLength;
    public float pushedAccelation;
    public LayerMask monsterLayer;
    public LayerMask playerLayer;
    public LayerMask roadLayer;

    public bool isClimb;
}

[Serializable]
public struct MonsterLayerSetter
{
    public LayerMask monsterLayer;
    public LayerMask playerLayer;
    public LayerMask roadLayer;
    public int sortingOrder;
}

public class GameController : Singleton<GameController>
{
    [SerializeField] GameObject[] zombiePrefabs;
    [SerializeField] Transform[] zombieSpawnPoints;

    HashSet<Monster> monsters = new();

    Queue<Action> killQ = new();

    private void Update()
    {
        while (killQ.Count > 0)
            killQ.Dequeue().Invoke();
    }

    public void ClickSpawn()
    {
        SpawnZombie();
    }

    public void ClickClear()
    {
        ClearZombies();
    }

    private void ClearZombies()
    {
        foreach(var zombie in monsters)
        {
            Destroy(zombie.gameObject);
        }
        monsters.Clear();
    }

    private void SpawnZombie()
    {
        int randInt = UnityEngine.Random.Range(0,3);
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

        MonsterLayerSetter setter = new()
        {
            monsterLayer = monsterLayer,
            roadLayer = roadLayer,
            playerLayer = LayerMask.GetMask(ReadonlyDatas.Player_Layer_String),
            sortingOrder = sortingOrder,
        };

        int randZombieIndex = UnityEngine.Random.Range(0,zombiePrefabs.Length);
        // var obj = Instantiate(zombiePrefab, zombieSpawnPoints[randInt].position,Quaternion.identity);
        var obj = ObjectManager.Ins.Spawn(zombiePrefabs[randZombieIndex], zombieSpawnPoints[randInt].position,Quaternion.identity);

        var comp = obj.GetComponent<MeleeZombie>();

        comp.Init(setter);
        monsters.Add(comp);
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
}
