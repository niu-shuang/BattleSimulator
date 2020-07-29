using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyDef : BuffBase
{
    public int rate { get; private set; }
    private Modifier.Item item;

    public ModifyDef(int rate)
    {
        this.rate = rate;
    }
    protected override void OnCast()
    {
        GameLogger.AddLog($"{target.name} defence rate changed by {rate/10}%");
        item = new Modifier.Item() { isMulti = true, value = rate };
        target.defModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} defence rate modifier removed");
        target.defModifier.RemoveItem(item);
    }
}
