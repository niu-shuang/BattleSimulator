using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyDodgeRate : BuffBase
{
    public int rate { get; private set; }
    private Modifier.Item item;

    public ModifyDodgeRate(int rate)
    {
        this.rate = rate;
    }

    protected override void OnCast()
    {
        item = new Modifier.Item() { isMulti = true, value = rate + GameDefine.PERCENTAGE_MAX };
        GameLogger.AddLog($"{target.name} dodge rate changed by {rate/10}%");
        target.dodgeRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} dodge rate modifier removed");
        target.dodgeRateModifier.RemoveItem(item);
    }
}
