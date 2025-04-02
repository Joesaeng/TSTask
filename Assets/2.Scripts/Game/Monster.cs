
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum MonsterStateEnum
{
    Idle,Move, Climb, Pushed, Jump
}

public class Monster : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;
    public SortingGroup sortingGroup;
    public Transform rayCenterPoint;

    public MonsterConfig config;

    public LayerMask currentLayer;
    public LayerMask roadLayer;
    public LayerMask playerLayer;

    public MonsterState currentState;
    public MonsterStateEnum stateEnum;

    [Header("등반할지, 점프할지")]
    public bool isClimb;

    Queue<Action> changeStateQ = new();

    public void Init(MonsterConfig config,int sortingOrder)
    {
        this.config = config;
        
        gameObject.layer = (int)Mathf.Log(config.monsterLayer.value, 2);
        sortingGroup.sortingOrder = sortingOrder;

        SetState(new IdleState());
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

        }
    }

    private void FixedUpdate()
    {
        while (changeStateQ.Count > 0)
        {
            changeStateQ.Dequeue().Invoke();
        }
        currentState?.Execute(this);
    }
}

