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

        Excute();
        Tick();
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

        Excute();
    }

    public void Excute()
    {
        int atk = (int)(caster.atkModifier.finalValue.Value * LifeDownRate / 1000f);
        AttackInfo attackInfo = new AttackInfo(caster, target, atk, GameDefine.DamageType.Physical);
        attackInfo.SetOnAttack(info => info.finalAtk = (int)(info.finalAtk * GameDefine.ATKMap[Mathf.Abs(target.pos.x - caster.pos.x)]));
        attackInfo.DoDamage();
    }
}
