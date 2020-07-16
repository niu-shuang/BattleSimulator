using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealInfo
{
    public CharacterLogic caster { get; private set; }
    public CharacterLogic target { get; private set; }

    public int baseHeal;
    public int finalHeal;

    private Action<HealInfo> onHeal;

    public HealInfo(CharacterLogic caster, CharacterLogic target, int baseHeal)
    {
        this.caster = caster;
        this.target = target;
        this.baseHeal = baseHeal;
        this.finalHeal = baseHeal;
    }

    public HealInfo SetOnHeal(Action<HealInfo> onHeal)
    {
        this.onHeal += onHeal;
        return this;
    }

    public void DoHeal()
    {
        caster.beforeHealSubject?.OnNext(this);
        onHeal?.Invoke(this);
        target.Heal(this);
        caster.afterHealSubject?.OnNext(this);
    }
}
