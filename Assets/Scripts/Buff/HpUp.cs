using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUp : BuffBase
{
    Modifier.Item item;
    public int upRate { get; private set; }

    public HpUp(int upRate)
    {
        this.upRate = upRate;
    }

    protected override void OnCast()
    {
        item = new Modifier.Item() { isMulti = true, value = GameDefine.PERCENTAGE_MAX + upRate };
        target.maxHpModifier.AddItem(item);
        var addHpValue = target.maxHp.Value * (GameDefine.PERCENTAGE_MAX + upRate) / GameDefine.PERCENTAGE_MAX;
        target.hpModifier.AddValueDirectly(addHpValue);
    }

    protected override void EndBuff()
    {
        target.maxHpModifier.RemoveItem(item);
        if(target.Hp.Value > target.maxHp.Value)
        {
            var hpDiff = target.maxHp.Value - target.Hp.Value;
            target.hpModifier.AddValueDirectly(hpDiff);
        }
        base.EndBuff();
    }
}
