using System;
using UnityEngine;

public interface IDamageable
{
    public int MaxHp { get; }
    public void InitDamageable();
    public void TakeDamage(int damage);
    public void OnDamageHandler(int damage);
    public void OnDeadHandler();
}

public class Damageable : MonoBehaviour
{
    public int curHp;
    public int maxHp;

    public Action<int> OnDamage;
    public Action OnDead;

    public bool hitRecovery;

    public void Init(IDamageable owner)
    {
        maxHp = owner.MaxHp;
        curHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;
        OnDamage?.Invoke(damage);

        if(curHp < 0)
        {
            curHp = 0;
            OnDead?.Invoke();
        }
    }
}

