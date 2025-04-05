using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Box : OnTheTruck, IDamageable
{
    public SpriteRenderer boxSpr;
    public Damageable damageable;
    public GameObject hpPanel;
    public Slider hpSlider;

    public int maxHp;
    public int MaxHp => maxHp;

    public void OnEnable()
    {
        hpPanel.SetActive(false);
        InitDamageable();
    }

    public override void OnRemoved()
    {
        boxSpr.enabled = false;
        Above.MoveDownToChain();
        if (Below != null)
            Below.Above = Above;
        if (Above != null)
            Above.Below = Below;

        Clear();
        ObjectManager.Ins.Kill(gameObject);
    }

    public override void MoveDownTo()
    {
        base.MoveDownTo();
        transform.DOShakeScale(0.1f, 0.1f, 1);
    }

    public override async UniTask MoveUpTo()
    {
        transform.DOShakeScale(0.1f, 0.1f, 1);
        await base.MoveUpTo();
    }

    public void InitDamageable()
    {
        damageable.Init(this);
        damageable.OnDamage += OnDamageHandler;
        damageable.OnDead += OnDeadHandler;
    }

    public void TakeDamage(int damage)
    {
        damageable.TakeDamage(damage);
    }

    public void OnDamageHandler(int damage)
    {
        if (!hpPanel.activeSelf)
            hpPanel.SetActive(true);

        hpSlider.value = (float)damageable.CurHp / MaxHp;
    }

    public void OnDeadHandler()
    {
        OnRemoved();
    }

    private void Clear()
    {
        damageable.OnDamage -= OnDamageHandler;
        damageable.OnDead -= OnDeadHandler;
    }
}
