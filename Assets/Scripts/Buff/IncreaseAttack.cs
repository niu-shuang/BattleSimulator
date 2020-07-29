using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttack : BuffBase
{
    private const int atkPercentage = 1500;
    protected override void OnBeforeAttack(AttackInfo info)
    {
        GameLogger.AddLog($"{target.name} cause {atkPercentage / 10}% damage");
        info.finalAtk = (int)(info.finalAtk * (float)atkPercentage / GameDefine.PERCENTAGE_MAX);
    }
}
