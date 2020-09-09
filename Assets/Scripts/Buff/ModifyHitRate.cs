using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyHitRate : BuffBase
{
    public int hitRate { get; private set; }
    private Modifier.Item item;

    public ModifyHitRate(int hitRate)
    {
        this.hitRate = hitRate;
    }

    protected override void OnCast()
    {
        item = new Modifier.Item() { isMulti = true, value = hitRate + GameDefine.PERCENTAGE_MAX };
        GameLogger.AddLog($"{target.name} hit rate changed by {hitRate / 10}%");
        target.hitRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} hit rate modifier removed");
        target.hitRateModifier.RemoveItem(item);
    }
}
