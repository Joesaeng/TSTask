
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IOnTheTruck
{
    // ���� ��ü
    IOnTheTruck Above { get; set; }
    // ���� ��ü
    IOnTheTruck Below { get; set; }
    // ���� ��ü�� ž ��ġ
    Transform TopPosTf { get; }
    // ���� ��ü�� �ٴ� ��ġ
    Transform BottomPosTf { get; }
    // ���� ��ü�� ����������
    Vector3 LocalPos { get; }

    public float YLegnth { get; }

    /// <summary>
    /// ��ü�� ������ �� ȣ��
    /// </summary>
    /// <param name="above">���� ��ü</param>
    /// <param name="below">���� ��ü</param>
    void Activate(IOnTheTruck above, IOnTheTruck below);

    /// <summary>
    /// ��ü�� ���ŵ� �� ȣ��
    /// </summary>
    void OnRemoved();

    UniTask MoveUpToChain();
    UniTask MoveUpTo();

    void MoveDownToChain();
    void MoveDownTo();
}
