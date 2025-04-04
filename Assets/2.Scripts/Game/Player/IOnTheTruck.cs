
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IOnTheTruck
{
    // 상위 객체
    IOnTheTruck Above { get; set; }
    // 하위 객체
    IOnTheTruck Below { get; set; }
    // 현재 객체의 탑 위치
    Transform TopPosTf { get; }
    // 현재 객체의 바닥 위치
    Transform BottomPosTf { get; }
    // 현재 객체의 로컬포지션
    Vector3 LocalPos { get; }

    public float YLegnth { get; }

    /// <summary>
    /// 객체를 생성할 때 호출
    /// </summary>
    /// <param name="above">상위 객체</param>
    /// <param name="below">하위 객체</param>
    void Activate(IOnTheTruck above, IOnTheTruck below);

    /// <summary>
    /// 객체가 제거될 때 호출
    /// </summary>
    void OnRemoved();

    UniTask MoveUpToChain();
    UniTask MoveUpTo();

    void MoveDownToChain();
    void MoveDownTo();
}
