using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyAttackRate : BuffBase
{
    public int rate { get; private set; }
    Modifier.Item item;

    public ModifyAttackRate(int rate)
    {
        this.rate = rate;
    }
    protected override void OnCast()
    {
        GameLogger.AddLog($"{target.name} attack modified by {rate / 10}%");
        item = new Modifier.Item() { isMulti = true, value = GameDefine.PERCENTAGE_MAX + rate };
        target.atkModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} attack modifier removed");
        target.atkModifier.RemoveItem(item);
        base.EndBuff();
    }
}
