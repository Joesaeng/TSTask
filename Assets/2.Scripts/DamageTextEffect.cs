using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextEffect : MonoBehaviour
{
    public TextMeshPro text;

    public void Effect(int damage)
    {
        transform.localScale = Vector3.one;
        text.alpha = 1f;

        text.text = damage.ToString();

        DOTween.Sequence().OnComplete(()=> ObjectManager.Ins.Kill(gameObject))
            .Append(transform.DOJump(transform.position + Vector3.down, 1.5f, 1, 0.6f))
            .Join(text.DOFade(0, 0.6f));
    }
}
