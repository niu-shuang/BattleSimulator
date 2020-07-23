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
        item = new Modifier.Item() { isMulti = false, value = rate };
        target.dodgeRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        target.dodgeRateModifier.RemoveItem(item);
    }
}
