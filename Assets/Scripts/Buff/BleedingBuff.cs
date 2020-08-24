using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BleedingBuff : BuffBase
{
    public int LifeDownRate { get; set; }

    protected override void OnCast()
    {
        if (target == null)
        {
            return;
        }
        GameLogger.AddLog($"{target.name} is bleeding now");
        target.isBleeding.Value = true;
    }

    protected override void EndBuff()
    {
        target.isBleeding.Value = false;
        if (target == null)
        {
            return;
        }
        GameLogger.AddLog($"{target.name} end bleeding");
        base.EndBuff();
    }

    protected override void OnTurnBegins()
    {
        base.OnTurnBegins();
        if (target == null)
        {
            return;
        }
        
        caster.Attack(target);
    }

    public void Excute()
    {
        Tick();
        OnTurnBegins();
    }
}
