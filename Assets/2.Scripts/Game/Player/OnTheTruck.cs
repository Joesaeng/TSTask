using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public abstract class OnTheTruck : MonoBehaviour, IOnTheTruck
{
    public Collider2D col;

    public IOnTheTruck Above { get; set; }
    public IOnTheTruck Below { get; set; }

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

    public abstract void OnRemoved();

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

    public virtual void MoveDownTo()
    {
        if (Below == null)
            return;
        Vector3 targetPos = LocalPos + Vector3.down * Below.YLegnth;

        transform.localPosition = targetPos;
    }

    public virtual async UniTask MoveUpTo()
    {
        if (Below == null)
            return;
        Vector3 targetPos = LocalPos + Vector3.up * Below.YLegnth;
        Tween tween = transform.DOLocalMove(targetPos, 0.1f);
        await tween.AsyncWaitForCompletion();
    }
}
