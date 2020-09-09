using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterAttack : BuffBase
{
    public int atkPercentage { get; private set; }
    public int hitRate { get; private set; }
    public int critRate { get; private set; }
    public int critDamageRate { get; private set; }

    public CounterAttack(int atkPercentage, int hitRate, int critRate, int critDamageRate)
    {
        this.atkPercentage = atkPercentage;
        this.hitRate = hitRate;
        this.critRate = critRate;
        this.critDamageRate = critDamageRate;
    }
    protected override void OnAfterDamage(DamageInfo info)
    {
        GameLogger.AddLog($"{target.name} counter attack");
        var atk = target.atkModifier.finalValue.Value * atkPercentage / GameDefine.PERCENTAGE_MAX;
        var critAtk = target.atkModifier.finalValue.Value * critDamageRate / GameDefine.PERCENTAGE_MAX;
        AttackInfo atkInfo = new AttackInfo(target, info.caster, atk, GameDefine.DamageType.Physical, critRate, critAtk, hitRate);
    }
}
