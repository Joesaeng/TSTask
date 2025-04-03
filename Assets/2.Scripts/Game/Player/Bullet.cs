using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public TrailRenderer trail;
    public Rigidbody2D rb;
    public float firePower;
    public float bulletLifetime;
    public float timeCapture;
    public int damage;

    public int maxHitCount = 1;
    private int hitCount;

    private bool isDestory;

    private void OnEnable()
    {
        isDestory = false;
        timeCapture = Time.time;
        hitCount = 0;
    }

    public void Fire(Vector2 fireDir)
    {
        float randFirePower = Random.Range(firePower * 0.95f, firePower * 1.05f);
        rb.AddForce(fireDir * randFirePower, ForceMode2D.Impulse);
    }

    public void Clear()
    {
        trail.Clear();
        rb.velocity = Vector2.zero;
    }

    private void Update()
    {
        if(timeCapture <= Time.time - bulletLifetime)
        {
            DestoryBullet();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(hitCount < maxHitCount && collision.collider.TryGetComponent<IDamageable>(out var damageable))
        {
            hitCount++;
            damageable.TakeDamage(damage);
        }
        DestoryBullet();
    }

    private void DestoryBullet()
    {
        if (isDestory)
            return;
        isDestory = true;
        GameController.Ins.EnqueueKill(() =>
        {
            Clear();
            ObjectManager.Ins.Kill(gameObject);
        });
    }
}
