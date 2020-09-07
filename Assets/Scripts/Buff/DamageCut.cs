using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCut : BuffBase
{
    public int percentage { get; private set; }

    public DamageCut(int percentage)
    {
        this.percentage = percentage;
    }

    protected override void OnBeforeDamage(DamageInfo info)
    {
        var finalDamage = info.damage * (GameDefine.PERCENTAGE_MAX - percentage) / GameDefine.PERCENTAGE_MAX;
        GameLogger.AddLog($"{target.name} cut damage from {info.damage} to {finalDamage}");
        info.damage = finalDamage;
    }
}
