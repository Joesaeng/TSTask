using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterConfig
{
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

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] Transform[] zombieSpawnPoints;

    [SerializeField] MonsterConfig monsterConfig;

    HashSet<MeleeZombie> zombies = new();

    private int count = 0;

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
        foreach(var zombie in zombies)
        {
            Destroy(zombie.gameObject);
        }
        count = 0;
        zombies.Clear();
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

        var randConfig = monsterConfig;
        randConfig.monsterLayer = monsterLayer;
        randConfig.roadLayer = roadLayer;

        var obj = Instantiate(zombiePrefab, zombieSpawnPoints[randInt].position,Quaternion.identity);
        obj.name = $"Zombie_{randInt}_{count++}";
        var comp = obj.GetComponent<MeleeZombie>();

        comp.Init(randConfig,sortingOrder);
        zombies.Add(comp);
    }

}
