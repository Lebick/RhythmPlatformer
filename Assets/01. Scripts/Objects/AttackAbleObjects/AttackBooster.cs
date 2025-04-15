using System.Linq;
using UnityEngine;

public class AttackBooster : AttackAbleObject
{
    public float strength = 3f;

    private PlayerPosition player;

    public override void Interaction(int index)
    {
        base.Interaction(index);

        AddVelocity(GamePlayManager.instance.player);
    }

    private void AddVelocity(PlayerPosition player)
    {
        Vector2 dir = (player.positions[^1] - (Vector2)transform.position).normalized;
        Vector2 value = dir * strength;

        player.isDash = false; //대쉬 취소
        player.velocity = value;
    }
}
