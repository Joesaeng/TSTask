using UnityEngine;

public static class MonsterUtil
{
    /// <summary>
    /// 위쪽에 다른 몬스터가 있는지 체크
    /// </summary>
    public static bool HasOtherMonsterUpSide(Monster owner)
    {
        var hits = Physics2D.RaycastAll((Vector2)owner.rayCenterPoint.position, Vector2.up, owner.config.upRayLength, owner.config.monsterLayer);
#if UNITY_EDITOR
        Debug.DrawRay((Vector2)owner.rayCenterPoint.position, Vector2.up * owner.config.upRayLength, Color.green);
#endif
        foreach (var hit in hits)
        {
            if (hit.collider == owner.col)
                continue;

            return true;
        }
        return false;
    }

    /// <summary>
    /// 앞(Vector2.left)에 있는 Collider 체크
    /// </summary>
    public static Collider2D CheckFront(Monster owner, LayerMask checkLayer)
    {
        var hits = Physics2D.RaycastAll((Vector2)owner.rayCenterPoint.position, Vector2.left, owner.config.frontRayLength, checkLayer);
#if UNITY_EDITOR
        Debug.DrawRay((Vector2)owner.rayCenterPoint.position, Vector2.left * owner.config.frontRayLength, Color.cyan);
#endif
        foreach (var hit in hits)
        {
            if (hit.collider == owner.col)
                continue;

            return hit.collider;
        }
        return null;
    }
}

public abstract class MonsterState
{
    public abstract void Enter(Monster owner);
    public abstract void Execute(Monster owner);
    public abstract void Exit(Monster owner);
}


/// <summary>
/// Spawn : 몬스터가 생성 시 호출됩니다
/// </summary>
public class SpawnState : MonsterState
{
    public override void Enter(Monster owner)
    {
        owner.bodies.SetActive(true);
        owner.deadHead.SetActive(false);

        owner.transform.rotation = Quaternion.identity;
        owner.col.enabled = true;
        owner.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void Execute(Monster owner)
    {
        owner.SetState(new IdleState());
    }

    public override void Exit(Monster owner)
    {
        
    }
}

/// <summary>
/// Idle : 앞의 Player 혹은 Monster를 체크하여 Climb, Move 상태 전이 담당
/// 아래의 있는 몬스터의 Pushed상태로의 전이도 함
/// </summary>
public class IdleState : MonsterState
{
    private float jumpElapsed;

    public override void Enter(Monster owner)
    {
        jumpElapsed = 0f;
    }

    public override void Execute(Monster owner)
    {
        CheckPlayerHit(owner);
        CheckFrontHit(owner);
    }

    private void CheckFrontHit(Monster owner)
    {
        // 전방 몬스터 체크
        Collider2D frontCol = MonsterUtil.CheckFront(owner,owner.config.monsterLayer);

        // 전방에 몬스터가 없다면 MoveState로 전이
        if (frontCol == null)
        {
            owner.SetState(new MoveState());
            return;
        }
        // 전방에 몬스터가 있을 때, Delay를 주고 전방 몬스터 등반을 시도
        jumpElapsed += Time.fixedDeltaTime;
        if (jumpElapsed > owner.config.jumpDelay)
        {
            jumpElapsed = 0f;
            if (TryClimb(owner, frontCol))
                return;
        }
    }

    private bool TryClimb(Monster owner, Collider2D frontCol)
    {
        // 현재 몬스터의 위에도 몬스터가 있는지 체크하여 있다면 등반하지 않는다
        // 위에 몬스터가 있을 때도 등반을 한다면 등반을 위한 힘도 부족할 것이고,
        // 보기에 뭔가 별로라 막음
        if (MonsterUtil.HasOtherMonsterUpSide(owner))
        {
            return false;
        }

        if (IsClimbAllowed(owner, frontCol))
        {
            // 너무 잦은 상태 전이를 막기 위한 장치
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                // 테스트용 등반/점프 플래그
                if (owner.config.isClimb)
                    owner.SetState(new ClimbState(frontCol));
                else
                    owner.SetState(new JumpState());
                return true;
            }
        }
        return false;
    }

    private bool IsClimbAllowed(Monster owner, Collider2D frontCol)
    {
        var frontComp = frontCol.GetComponent<Monster>();
        if (frontComp == null)
            return false;

        // 앞에 있는 몬스터의 속도가 자기자신보다 빠른 경우 등반 불가
        if (Mathf.Abs(frontComp.rb.velocity.x) > Mathf.Abs(owner.rb.velocity.x))
            return false;
        return true;
    }

    private void CheckPlayerHit(Monster owner)
    {
        // 전방에 플레이어의 객체가 있는지를 체크한다
        var playerHit = MonsterUtil.CheckFront(owner,owner.config.playerLayer);
        if(playerHit == null) 
            return;
        {
            // 전방에 플레이어가 체크됬을 때, 바로 밑에있는 몬스터를 체크
            var bottomHit = Physics2D.CircleCastAll((Vector2)owner.transform.position,0.25f,Vector2.down,owner.config.downRayLength,owner.config.monsterLayer);
            foreach (var hit in bottomHit)
            {
                if (hit.collider == owner.col)
                    continue;

                // 아래에 있는 몬스터가 이미 Pushed 상태인 경우에는 넘어감
                var comp = hit.collider.GetComponent<Monster>();
                if (comp.stateEnum == MonsterStateEnum.Pushed)
                    continue;
                // 아래에 있는 몬스터를 Pushed 상태로 전이시킨다.
                // * 쌓여있는 몬스터를 순환시키는 장치 *
                hit.collider.GetComponent<Monster>().SetState(new PushedState());
                //owner.SetState(new DownState());
                return;
            }
        }
    }

    public override void Exit(Monster owner)
    {

    }
}

/// <summary>
/// Move : 앞으로 이동하며, 앞에 있는 Player 혹은 Monster를 체크하여 Idle상태로의 전이를 함
/// </summary>
public class MoveState : MonsterState
{
    private Vector2 moveDir;

    public override void Enter(Monster owner)
    {
        moveDir = Vector2.left;
    }

    public override void Execute(Monster owner)
    {
        // 플레이어 혹은 다른 몬스터와 닿으면 IdleState로 변경
        var hit = MonsterUtil.CheckFront(owner,owner.config.monsterLayer | owner.config.playerLayer);
        if(hit != null)
        {
            owner.SetState(new IdleState());
        }

        owner.rb.AddForce(moveDir * owner.config.moveSpeed * owner.config.moveAccelation);

        if (owner.rb.velocity.x <= -owner.config.moveSpeed)
            owner.rb.velocity = new Vector2(moveDir.x * owner.config.moveSpeed, owner.rb.velocity.y);
    }

    public override void Exit(Monster owner)
    {

    }
}

/// <summary>
/// Jump : Climb와 택일하는 Jump 상태, Climb가 좀 더 맘에 드는 이동이 나와서 안씀
/// </summary>
public class JumpState : MonsterState
{
    public override void Enter(Monster owner)
    {
        owner.rb.AddForce(Vector2.up * owner.config.jumpPower, ForceMode2D.Impulse);
    }

    public override void Execute(Monster owner)
    {
        owner.rb.AddForce(Vector2.left * owner.config.moveSpeed);
        if (owner.rb.velocity.y <= 0)
        {
            owner.SetState(new MoveState());
            return;
        }
    }

    public override void Exit(Monster owner)
    {

    }
}

/// <summary>
/// Climb : 앞에 있는 몬스터를 등반한다, 등반 종료 후 Idle상태로 전이
/// </summary>
public class ClimbState : MonsterState
{
    private Collider2D climbTarget;
    private Vector2 moveDir;
    private Vector2 rayDir;
    private float climbThreshold = 0.95f; // 등반의 임계값

    // 등반할 타겟을 생성자로 받아준다
    public ClimbState(Collider2D climbTarget)
    {
        this.climbTarget = climbTarget;
    }

    public override void Enter(Monster owner)
    {

    }

    public override void Execute(Monster owner)
    {
        ClimbByRayCast(owner);
    }

    // 레이캐스트를 통한 등반
    private void ClimbByRayCast(Monster owner)
    {
        // 등반시점과 동일하게 위에도 다른 몬스터가 있다면 등반을 취소한다
        if(MonsterUtil.HasOtherMonsterUpSide(owner))
        {
            owner.SetState(new IdleState());
            return;
        }

        // 등반할 타겟이 등반하고있는 상태라면 등반을 취소하는 장치
        if (climbTarget.GetComponent<Monster>().stateEnum == MonsterStateEnum.Climb)
        {
            owner.SetState(new IdleState());
            return;
        }

        // 타겟방향으로 raycast
        rayDir = (climbTarget.transform.position - owner.transform.position).normalized;
        var hits = Physics2D.RaycastAll(owner.rayCenterPoint.position, rayDir, owner.config.climbRayLength, owner.config.monsterLayer);

        bool validHit = false;
        foreach (var hit in hits)
        {
            if (hit.collider == owner.col)
                continue;
            // ray의 normal 벡터를 통해 이동방향을 잡는다
            var normal = hit.normal;
            moveDir = new Vector2(-normal.y, normal.x).normalized;

            // 등반 임계값을 넘어간 경우 Idle상태로 전이
            if (Mathf.Abs(normal.y) >= climbThreshold)
            {
                owner.SetState(new IdleState());
                return;
            }
            validHit = true;

#if UNITY_EDITOR
            Debug.DrawRay(owner.rayCenterPoint.position, rayDir * owner.config.climbRayLength, Color.red);
            Debug.DrawRay(hit.point, moveDir, Color.blue);
#endif
            break;
        }

        // 타겟을 놓친 경우에도 Idle상태로 전이한다
        if (!validHit)
        {
            owner.SetState(new IdleState());
            return;
        }

        // rigidbody의 velocity를 강제로 조정하여 중력을 무시하고 등반을 하게 한다
        owner.rb.velocity = Vector2.Lerp(owner.rb.velocity, moveDir * owner.config.moveSpeed, Time.fixedDeltaTime * owner.config.climbAccelation);
        //owner.rb.AddForce(moveDir * moveSpeed * 1.5f);
    }

    public override void Exit(Monster owner)
    {

    }
}

/// <summary>
/// Pushed : 뒤에있는 몬스터를 밀며, PushedLength 이상 밀게 되면 Idle상태로 전이함
/// </summary>
public class PushedState : MonsterState
{
    private float curMoveSpeedX;
    public override void Enter(Monster owner)
    {
        // 뒤에있는 몬스터들을 밀기 위한 무게 조정
        owner.rb.mass = owner.config.pushedMass;
        curMoveSpeedX = 0f;
    }

    public override void Execute(Monster owner)
    {
        // float의 Lerp를 사용하여 좀더 자연스러운 이동을 구현
        curMoveSpeedX = Mathf.Lerp(curMoveSpeedX, owner.config.moveSpeed, Time.fixedDeltaTime * owner.config.pushedAccelation);
        // 뒤에 있는 몬스터들을 밀기 위해 현재 몬스터 rigidbody의 velocity를 강제조정하여 밀 수 있게 한다
        owner.rb.velocity = new Vector2(curMoveSpeedX, owner.rb.velocity.y);

        // Pushed상태로 전이되는 조건이, Player와 맞닿아있는 몬스터가 아래로 raycast하여 호출해줘서 전이되는데,
        // 그럼 이 몬스터도 Player와 인접한 상태라는게 성립한다.
        // 그렇기에 Player 방향(Vector2.left)로 raycast하여 미리 정한 pushedLength만큼 떨어지게끔 한다.
        var hit = MonsterUtil.CheckFront(owner,owner.config.playerLayer);
        if (hit == null)
        {
            owner.SetState(new IdleState());
            return;
        }
    }

    public override void Exit(Monster owner)
    {
        // 무게 초기화
        owner.rb.mass = 1f;
    }
}

/// <summary>
/// Hit : 피격 시 호출됩니다.
/// </summary>
public class HitState : MonsterState
{
    private float hitRecoveryDelay = 0.2f;
    private float timeCapture;

    public override void Enter(Monster owner)
    {
        timeCapture = Time.time;
    }

    public override void Execute(Monster owner)
    {
        if(timeCapture <= Time.time - hitRecoveryDelay)
        {
            owner.SetState(new IdleState());
            return;
        }
    }

    public override void Exit(Monster owner)
    {
        
    }
}

/// <summary>
/// Dead : 사망 시 호출됩니다
/// </summary>
public class DeadState : MonsterState
{
    private float deadTime = 1.5f;
    private float timeCapture;
    public override void Enter(Monster owner)
    {
        owner.bodies.SetActive(false);
        owner.deadHead.SetActive(true);

        owner.Clear();
        owner.col.enabled = false;
        owner.rb.velocity = Vector2.zero;
        owner.rb.constraints = RigidbodyConstraints2D.None;
        owner.rb.AddForce(Vector2.up * 6.6f, ForceMode2D.Impulse);
        timeCapture = Time.time;
    }

    public override void Execute(Monster owner)
    {
        if(timeCapture < Time.time - deadTime)
        {
            GameController.Ins.KillMonster(owner);
            return;
        }
    }

    public override void Exit(Monster owner)
    {
        
    }
}