using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterAttack : BuffBase
{
    public int counterRate { get; private set; }

    public CounterAttack(int counterRate)
    {
        this.counterRate = counterRate;
    }
    protected override void OnAfterDamage(DamageInfo info)
    {
        var rand = Random.Range(0, GameDefine.PERCENTAGE_MAX);
        if(rand < counterRate)
        {
            target.Attack(info.caster);
        }
    }
}
