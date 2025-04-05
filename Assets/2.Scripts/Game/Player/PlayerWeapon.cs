using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gunTf;
    public Transform firePos;
    public LayerMask monsterLayer;

    public float sensingRadius;

    public float fireDelay;
    public float fireAngle;
    public int bulletCount;

    public Vector2 fireDir;

    private float fireCapture;

    public Collider2D closestTarget;

    private void Update()
    {
        WeaponRotateToClosetTarget();
        SensingMonster();
        if (fireCapture <= Time.time - fireDelay)
        {
            fireCapture = Time.time;
            Fire();
        }
    }

    private void WeaponRotateToClosetTarget()
    {
        if (closestTarget == null)
            fireDir = new Vector2(1, -1);
        else
            //fireDir = (closestTarget.transform.position - firePos.position).normalized;
            fireDir = (closestTarget.bounds.center - gunTf.position).normalized;

        gunTf.rotation = Util.GetTargetRotation(fireDir);
    }

    private void Fire()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // var bullet = Instantiate(bulletPrefab,firePos.position,Quaternion.identity);
            var bullet = ObjectManager.Ins.Spawn(bulletPrefab,firePos.position,Quaternion.identity);
            var comp = bullet.GetComponent<Bullet>();

            // 샷건 스프레드
            float randAngle = Random.Range(-fireAngle * 0.5f, fireAngle * 0.5f);
            Vector2 randDir = Quaternion.Euler(0,0,randAngle) * fireDir;

            comp.Fire(randDir);
        }
    }

    private void SensingMonster()
    {
        if (closestTarget == null || closestTarget.GetComponent<Monster>().stateEnum == MonsterStateEnum.Dead
            || Vector3.Distance(firePos.position,closestTarget.transform.position) > sensingRadius)
        {
            float mindistance = float.MaxValue;
            var hits = Physics2D.OverlapCircleAll(firePos.position,sensingRadius,monsterLayer);

            Collider2D closest = null;
            foreach (var hit in hits)
            {
                
                float distance = (gunTf.position - hit.transform.position).sqrMagnitude;
                if (distance < mindistance)
                {
                    mindistance = distance;
                    closest = hit;
                }
            }
            closestTarget = closest;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(firePos.position, sensingRadius);
    }
}
