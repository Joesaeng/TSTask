using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Box : MonoBehaviour, IOnTheTruck
{
    public Transform topPosTf;
    public Transform bottomPosTf;
    public Collider2D col;

    public IOnTheTruck Above { get; set; }
    public IOnTheTruck Below { get; set; }
    public Transform TopPosTf { get => topPosTf; }
    public Transform BottomPosTf { get => bottomPosTf; }

    public Vector3 LocalPos => transform.localPosition;

    public float YLegnth => col.bounds.size.y;


    public void Activate(IOnTheTruck above, IOnTheTruck below)
    {
        Below = below;
        Above = above;

        if (Below != null)
        {
            Below.Above = this;
        }

        if (Above != null)
        {
            Above.Below = this;
        }
        MoveUpToChain().Forget();
    }

    public void OnRemoved()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        Above.MoveDownToChain();
        if (Below != null)
            Below.Above = Above;
        if (Above != null)
            Above.Below = Below;
        ObjectManager.Ins.Kill(gameObject);
    }

    public async UniTask MoveUpToChain()
    {
        await MoveUpTo();
        if (Above != null)
        {
            Above.MoveUpToChain().Forget();
        }
    }

    public void MoveDownToChain()
    {
        MoveDownTo();
        if (Above != null)
        {
            Above.MoveDownToChain();
        }
    }

    public void MoveDownTo()
    {
        if (Below == null)
            return;
        Vector3 targetPos = LocalPos + Vector3.down * YLegnth;

        transform.DOShakeScale(0.1f, 0.1f, 1);
        transform.localPosition = targetPos;
    }

    public async UniTask MoveUpTo()
    {
        if (Below == null)
            return;
        Vector3 targetPos = LocalPos + Vector3.up * YLegnth;
        Tween tween = transform.DOLocalMove(targetPos, 0.1f);
        await tween.AsyncWaitForCompletion();
    }
}
