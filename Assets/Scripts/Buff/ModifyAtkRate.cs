using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyAtkRate : BuffBase
{
    public int atkPercentage { get; private set; }

    public ModifyAtkRate(int atkPercentage)
    {
        this.atkPercentage = atkPercentage;
    }
    protected override void OnBeforeAttack(AttackInfo info)
    {
        GameLogger.AddLog($"{target.name} cause {atkPercentage / 10}% damage");
        info.finalAtk = (int)(info.finalAtk * (float)atkPercentage / GameDefine.PERCENTAGE_MAX);
    }
}
