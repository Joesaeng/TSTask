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

    [Header("���Ͱ� �� ���͸� ������ ��, ����� �õ��� Ȯ��"),Range(0f,1f)]
    public float climbChance = 0.5f;

    [Header("���� ���� �� ������ ���� ������, ���� �� ������ \n 0.05�� �� �پ��� 0.15�ʱ��� �پ��ϴ�")]
    public float firstSpawnDelay;

    [Header("Ʈ���� �ִ� �ӵ�")]
    public float truckMaxSpeed;

    [Header("Ʈ���� �׼�")]
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
