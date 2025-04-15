using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : AttackAbleObject
{
    public int hp;

    private int decreaseHP;

    public GameObject getDamageEffect;

    private List<float> getDamageTiming = new();

    public List<int> usedDamageAnimIndex = new();

    public override void Interaction(int index)
    {
        base.Interaction(index);

        getDamageTiming.Add(index);
    }

    private void Update()
    {
        decreaseHP = 0;

        for(int i=0; i<getDamageTiming.Count; i++)
        {
            if (GamePlayManager.instance.progress >= getDamageTiming[i])
            {
                if (!usedDamageAnimIndex.Contains(i))
                {
                    usedDamageAnimIndex.Add(i);
                    GetDamage();
                }

                decreaseHP++;
            }
            else
            {
                usedDamageAnimIndex.Remove(i);
            }
        }

        if (hp - decreaseHP <= 0)
        {
            Death();
        }
    }

    public virtual void GetDamage()
    {
        //히트 시 하얀색이 되었다가 서서히 돌아와야함.

        GameObject effect = PoolingManager.instance.GetPooling(getDamageEffect);
        effect.GetComponent<PoolingObject>().Initialize(getDamageEffect);

        Vector3 dir = (GamePlayManager.instance.player.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.Euler(0, 0, angle);

        effect.transform.SetPositionAndRotation(transform.position, rot);
    }

    public virtual void Death()
    {

    }
}
