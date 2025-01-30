using UnityEngine;

public class DashOnlyGround : MonoBehaviour
{
    private int waitFrame; //�뽬 ������ ���� Ÿ�̹� ���� ��Ȱ�� ���¸� �����ϱ� ����
    private int requireWaitFrame = 50;

    private void Start()
    {
        GamePlayManager.instance.player.calculateEvents.AddListener(() => ChangeState(GamePlayManager.instance.player));
    }

    private void ChangeState(PlayerPosition player)
    {
        if (player.isDash) //�뽬 ������ ��
        {
            waitFrame = 0;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }    
        else
        {
            waitFrame += player.oneCalculateInterval;
            if(waitFrame >= requireWaitFrame)
                gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }
}
