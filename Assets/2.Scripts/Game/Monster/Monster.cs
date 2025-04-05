
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum MonsterStateEnum
{
    Spawn, Idle, Move, Climb, Pushed, Jump, Hit, Dead
}

public class Monster : MonoBehaviour, IDamageable
{
    public Animator animator;
    public Rigidbody2D rb;
    public Collider2D col;
    public SortingGroup sortingGroup;
    public Transform rayCenterPoint;
    public Damageable damageable;
    public Transform attackPoint;

    public MonsterConfig config;

    public MonsterState currentState;
    public MonsterStateEnum stateEnum;

    public GameObject deadHead;
    public GameObject bodies;

    [Header("등반할지, 점프할지")]
    public bool isClimb;

    Queue<Action> changeStateQ = new();

    public int MaxHp { get => config.maxHp;}

    private int animIDIsAttacking = Animator.StringToHash("IsAttacking");
    private float attackTimeCapture;

    public void Init(MonsterSetter setter)
    {
        config.playerLayer = setter.playerLayer;
        config.monsterLayer = setter.monsterLayer;
        config.roadLayer = setter.roadLayer;

        gameObject.layer = (int)Mathf.Log(setter.monsterLayer.value, 2);
        sortingGroup.sortingOrder = setter.sortingOrder;

        InitDamageable();
        SetState(new SpawnState());
        attackTimeCapture = Time.time;
    }

    public void InitDamageable()
    {
        damageable.Init(this);
        damageable.OnDamage += OnDamageHandler;
        damageable.OnDead += OnDeadHandler;
    }

    public void Clear()
    {
        damageable.OnDamage -= OnDamageHandler;
        damageable.OnDead -= OnDeadHandler;
    }

    public void SetState(MonsterState nextState)
    {
        changeStateQ.Enqueue(() =>
        {
            SetStateEnum(nextState);
            currentState?.Exit(this);
            currentState = nextState;
            currentState.Enter(this);
        });
    }

    private void SetStateEnum(MonsterState nextState)
    {
        switch (nextState)
        {
            case SpawnState:
                stateEnum = MonsterStateEnum.Spawn; 
                break;
            case IdleState:
                stateEnum = MonsterStateEnum.Idle;
                break;
            case MoveState:
                stateEnum = MonsterStateEnum.Move;
                break;
            case ClimbState:
                stateEnum = MonsterStateEnum.Climb;
                break;
            case PushedState:
                stateEnum = MonsterStateEnum.Pushed;
                break;
            case JumpState:
                stateEnum = MonsterStateEnum.Jump;
                break;
            case HitState:
                stateEnum = MonsterStateEnum.Hit;
                break;
            case DeadState:
                stateEnum = MonsterStateEnum.Dead;
                break;

        }
    }

    private void FixedUpdate()
    {
        while (changeStateQ.Count > 0)
        {
            changeStateQ.Dequeue().Invoke();
        }
        currentState?.Execute(this);
        AttackUpdate();
    }

    private void AttackUpdate()
    {
        if(attackTimeCapture <= Time.time - config.attackDelay)
        {
            attackTimeCapture = Time.time;
            TryAttack();
        }
    }

    private void TryAttack()
    {
        var hit = Physics2D.OverlapPoint(attackPoint.position,config.playerLayer);
        if (hit)
        {
            animator.SetBool(animIDIsAttacking, true);
        }
    }

    public void AttackImpact()
    {
        var hit = Physics2D.OverlapPoint(attackPoint.position,config.playerLayer);
        if (hit)
        {
            if(hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(config.damage);
            }
        }
        animator.SetBool(animIDIsAttacking, false);
    }

    public void OnDamageHandler(int damage)
    {
        // 데미지 표기
        GameController.Ins.DamageTextEffect(transform.position, damage);

        if (stateEnum == MonsterStateEnum.Hit || stateEnum == MonsterStateEnum.Dead)
            return;
        SetState(new HitState());
    }

    public void OnDeadHandler()
    {
        SetState(new DeadState());   
    }

    public void TakeDamage(int damage)
    {
        if (stateEnum == MonsterStateEnum.Dead)
            return;

        damageable.TakeDamage(damage);
    }
}

