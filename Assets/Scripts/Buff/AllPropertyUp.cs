using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AllPropertyUp : BuffBase
{
    private Modifier.Item atkUp;
    private Modifier.Item defUp;
    private Modifier.Item dodgeRateUp;
    public int upgradePercentage { get; private set; }

    public AllPropertyUp(int upgradePercentage)
    {
        this.upgradePercentage = upgradePercentage;
    }
    protected override void OnCast()
    {
        atkUp = new Modifier.Item() { isMulti = true, value = upgradePercentage };
        defUp = new Modifier.Item() { isMulti = true, value = upgradePercentage };
        dodgeRateUp = new Modifier.Item() { isMulti = true, value = upgradePercentage };
        target.atkModifier.AddItem(atkUp);
        target.defModifier.AddItem(defUp);
        target.dodgeRateModifier.AddItem(dodgeRateUp);
        GameLogger.AddLog($"{target.name} all property up");
    }

    protected override void EndBuff()
    {
        target.atkModifier.RemoveItem(atkUp);
        target.defModifier.RemoveItem(defUp);
        target.dodgeRateModifier.RemoveItem(dodgeRateUp);
        GameLogger.AddLog($"{target.name} all property modifier removed");
        base.EndBuff();
    }
}
