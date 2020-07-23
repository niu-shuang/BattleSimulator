using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttack : BuffBase
{
    private const int atkPercentage = 1500;
    protected override void OnBeforeAttack(AttackInfo info)
    {
        info.finalAtk = (int)(atkPercentage * (float)atkPercentage / GameDefine.PERCENTAGE_MAX);
    }
}
