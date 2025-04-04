using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IOnTheTruck
{
    public Transform topPosTf;
    public Transform bottomPosTf;

    public IOnTheTruck Above { get; set; }
    public IOnTheTruck Below { get; set; }

    public Transform TopPosTf => topPosTf;
    public Transform BottomPosTf => bottomPosTf;

    public Vector3 LocalPos => transform.localPosition;

    public float YLegnth => 1f;

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
        Vector3 targetPos = LocalPos + Vector3.down * Below.YLegnth;

        transform.localPosition = targetPos;
    }

    public async UniTask MoveUpTo()
    {
        if (Below == null)
            return;
        Vector3 targetPos = LocalPos + Vector3.up * Below.YLegnth;
        Tween tween = transform.DOLocalMove(targetPos, 0.1f);
        await tween.AsyncWaitForCompletion();
    }
}
