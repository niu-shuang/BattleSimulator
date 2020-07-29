using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalShield : BuffBase
{
    public int percentage { get; private set; }

    public ImmortalShield(int percentage)
    {
        this.percentage = percentage;
    }
    protected override void OnBeforeDamage(DamageInfo info)
    {
        var maxDamage = target.maxHp.Value * percentage / GameDefine.PERCENTAGE_MAX;
        if (info.damage > maxDamage)
        {
            GameLogger.AddLog($"{target.name} immortal shield active, cut damage from {info.damage} to {maxDamage}");
            info.damage = maxDamage;
        }
    }
}
