using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleeding : BuffBase
{
    public int damage { get; private set; }

    public Bleeding(int initDamage, int atkPercentage)
    {
        damage = initDamage * atkPercentage / GameDefine.PERCENTAGE_MAX;
    }

    protected override void OnTurnBegins()
    {
        AttackInfo info = new AttackInfo(caster, target, damage, GameDefine.DamageType.Magical);
        info.DoDamage();
    }
}
