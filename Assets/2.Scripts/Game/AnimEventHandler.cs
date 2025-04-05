using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
    public Monster owner;

    public void OnAttack()
    {
        owner.AttackImpact();
    }
}