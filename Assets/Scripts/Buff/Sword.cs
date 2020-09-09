using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : BuffBase
{
    public int atkPercentage { get; private set; }
    public int bleedingPercentage { get; private set; }

    public int bleedingContinuousTurn { get; private set; }
    
    public Sword(int atkPercentage, int bleedingPercentage, int bleedingContinuousTurn)
    {
        this.atkPercentage = atkPercentage;
        this.bleedingPercentage = bleedingPercentage;
        this.bleedingContinuousTurn = bleedingContinuousTurn;
    }

    protected override void OnBeforeAttack(AttackInfo info)
    {
        info.finalAtk = info.finalAtk * atkPercentage / GameDefine.PERCENTAGE_MAX;
        Bleeding buff = new Bleeding(info.finalAtk, bleedingPercentage);
        buff.Init(info.target, caster, GameDefine.BuffTickType.Turn, false, bleedingContinuousTurn);
        info.target.AddBuff(buff);
    }
}
