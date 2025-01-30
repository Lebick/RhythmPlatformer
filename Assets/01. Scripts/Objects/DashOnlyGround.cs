using UnityEngine;

public class DashOnlyGround : MonoBehaviour
{
    private int waitFrame; //대쉬 끝나고 일정 타이밍 동안 비활성 상태를 유지하기 위함
    private int requireWaitFrame = 50;

    private void Start()
    {
        GamePlayManager.instance.player.calculateEvents.AddListener(() => ChangeState(GamePlayManager.instance.player));
    }

    private void ChangeState(PlayerPosition player)
    {
        if (player.isDash) //대쉬 상태일 시
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
