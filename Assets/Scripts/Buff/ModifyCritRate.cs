using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyCritRate : BuffBase
{
    public int percentage { get; private set; }
    private Modifier.Item item;
    
    public ModifyCritRate(int percentage)
    {
        this.percentage = percentage;
    }

    protected override void OnCast()
    {
        GameLogger.AddLog($"{target.name} crit rate up by {percentage / 10}%");
        item = new Modifier.Item() { isMulti = false, value = percentage };
        target.critRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} crit rate modifier removed");
        target.critRateModifier.RemoveItem(item);
    }
}
