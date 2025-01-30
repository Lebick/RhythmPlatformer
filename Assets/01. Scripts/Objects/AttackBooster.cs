using System.Linq;
using UnityEngine;

public class AttackBooster : MonoBehaviour
{
    public float strength = 3f;

    private void Start()
    {
        GamePlayManager.instance.player.calculateEvents.AddListener(() => AddVelocity(GamePlayManager.instance.player));
    }

    private void AddVelocity(PlayerPosition player)
    {
        //대쉬 시로 변경
        //if (player.isAttack)
        //{
        //    Vector3 pos = player.positions[^1];
        //
        //    Transform range = player.attackRange;
        //
        //    if(Physics2D.OverlapBoxAll(pos + range.localPosition, range.lossyScale, 0).Contains(gameObject.GetComponent<Collider2D>()))
        //    {
        //        Vector2 dir = player.positions[^1] - (Vector2)transform.position;
        //        Vector2 value = dir * strength;
        //
        //        player.isDash = false; //대쉬 취소
        //        player.velocity = value;
        //    }
        //}
        //else
        //{
        //
        //}
    }
}
