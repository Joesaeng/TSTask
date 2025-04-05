using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float attackDelay;
    public int damage;
    public bool isClimb;
}

[Serializable]
public struct MonsterSetter
{
    public LayerMask monsterLayer;
    public LayerMask playerLayer;
    public LayerMask roadLayer;
    public int sortingOrder;
}

public class GameController : Singleton<GameController>
{
    [SerializeField] Button[] gameStartedDisableButtons;
    
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] MonsterSpawner monsterSpawner;
    [SerializeField] PlayerVehicle player;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] CinemachineVirtualCamera virCam;

    [Header("몬스터가 앞 몬스터를 만났을 때, 등반을 시도할 확률"),Range(0f,1f)]
    public float climbChance = 0.5f;

    [Header("게임 시작 시 몬스터의 스폰 딜레이, 스폰 할 때마다 \n 0.05초 씩 줄어들며 0.15초까지 줄어듭니다")]
    public float firstSpawnDelay;

    [Header("트럭의 최대 속도")]
    public float truckMaxSpeed;

    [Header("트럭의 액셀")]
    public float truckAccelation;

    List<Util.WeightedItem<bool>> climbWeighted;

    private void Start()
    {
        SetClimbWeighted();
    }

    private void SetClimbWeighted()
    {
        climbWeighted = new()
        {
            new Util.WeightedItem<bool>(){item = true, weight = climbChance},
            new Util.WeightedItem<bool>(){item = false, weight = (1f - climbChance)},
        };
    }

    public void ClickGameStart()
    {
        GameStart();
    }

    public void GameReset()
    {
        monsterSpawner.ClearZombies();
        monsterSpawner.gameObject.SetActive(false);

        virCam.Follow = null;
        ObjectManager.Ins.Kill(player.gameObject);
        var newPlayerObj = ObjectManager.Ins.Spawn(playerPrefab,new Vector3(-2,-2.24f,0f));
        virCam.Follow = newPlayerObj.transform;

        player = newPlayerObj.GetComponent<PlayerVehicle>();

        foreach (var button in gameStartedDisableButtons)
            button.gameObject.SetActive(true);
    }

    private void GameStart()
    {
        player.moveSpeed = truckMaxSpeed;
        player.accelation = truckAccelation;

        monsterSpawner.gameObject.SetActive(true);
        monsterSpawner.Init(firstSpawnDelay);

        foreach (var button in gameStartedDisableButtons)
            button.gameObject.SetActive(false);

        player.GetComponentInChildren<PlayerWeapon>().enabled = true;
    }

    public void NewBox()
    {
        player.NewBox();
    }

    public bool IsTryClimb()
    {
        return Util.WeightedRandomUtility.GetWeightedRandom(climbWeighted);
    }

    public void KillMonster(Monster monster)
    {
        monsterSpawner.KillMonster(monster);
    }

    public void ClickClear()
    {
        monsterSpawner.ClearZombies();
    }

    public void DamageTextEffect(Vector3 spawnPos,int damage)
    {
        var obj = ObjectManager.Ins.Spawn(damageTextPrefab, spawnPos + Vector3.up * 0.5f);
        var comp = obj.GetComponent<DamageTextEffect>();
        comp.Effect(damage);
    }
}
