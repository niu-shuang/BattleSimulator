using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public CharacterLogic caster { get; private set; }
    public CharacterLogic target { get; private set; }

    public GameDefine.DamageType damageType { get; private set; }
    public int damage;
    public bool isCrit;

    private Action<DamageInfo> onDamage;

    public DamageInfo(AttackInfo attackInfo)
    {
        caster = attackInfo.caster;
        target = attackInfo.target;
        damageType = attackInfo.damageType;
        damage = attackInfo.finalAtk;
        isCrit = false;
    }

    public DamageInfo SetBeforeDamage(Action<DamageInfo> onDamageAction)
    {
        onDamage += onDamageAction;
        return this;
    }

    public void InvokeDamageAction()
    {
        onDamage?.Invoke(this);
    }
}
