using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyCritDamage : BuffBase
{
    public int percentage { get; private set; }
    private Modifier.Item item;

    public ModifyCritDamage(int percentage)
    {
        this.percentage = percentage;
    }

    protected override void OnCast()
    {
        GameLogger.AddLog($"{target.name} crit damage up by {percentage / 10}%");
        item = new Modifier.Item() { isMulti = true, value = percentage };
        target.critDamageRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} crit damage modifier removed");
        target.critRateModifier.RemoveItem(item);
    }
}
