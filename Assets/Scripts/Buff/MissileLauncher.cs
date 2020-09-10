using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MissileLauncher : BuffBase
{
    public int atkPercentage { get; private set; }
    public int critRate { get; private set; }
    public int critDamage { get; private set; }
    public int hitRate { get; private set; }

    public MissileLauncher(int atkPercentage, int critRate, int critDamage, int hitRate)
    {
        this.atkPercentage = atkPercentage;
        this.critRate = critRate;
        this.critDamage = critDamage;
        this.hitRate = hitRate;
    }

    protected override void OnBeforeAttack(AttackInfo info)
    {
        info.critDamageRate = critDamage;
        info.critRate = critRate;
        info.hitRate = hitRate;
        info.finalAtk = info.finalAtk * atkPercentage / GameDefine.PERCENTAGE_MAX;

        var targets = GameManager.Instance.GetCharacters(info.target.team);
        foreach (var item in targets)
        {
            if (item == info.target) continue;
            var atk = caster.atkModifier.finalValue.Value * atkPercentage / GameDefine.PERCENTAGE_MAX;
            var critAtk = caster.atkModifier.finalValue.Value * critDamage / GameDefine.PERCENTAGE_MAX;
            AttackInfo attackInfo = new AttackInfo(caster, item, atk, GameDefine.DamageType.Physical, critRate, critAtk, hitRate);
            attackInfo.DoDamageSilent();
        }
    }
}
