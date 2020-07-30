using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AttackInfo
{
    public CharacterLogic caster { get; private set; }
    public CharacterLogic target { get; private set; }

    public int baseAtk { get; private set; }
    public int finalAtk;

    public GameDefine.DamageType damageType { get; private set; }

    private Action<AttackInfo> onAttack;

    public AttackInfo(CharacterLogic caster, CharacterLogic target, int baseAtk, GameDefine.DamageType damageType)
    {
        this.caster = caster;
        this.target = target;
        this.baseAtk = baseAtk;
        this.finalAtk = baseAtk;
        this.damageType = damageType;
    }

    public AttackInfo SetOnAttack(Action<AttackInfo> onAttack)
    {
        this.onAttack += onAttack;
        return this;
    }

    public void DoDamage()
    {
        caster.beforeAttackSubject?.OnNext(this);
        onAttack?.Invoke(this);
        GameLogger.AddLog($"{caster.name}(id : {caster.characterId}) deal {this.finalAtk} atk to {target.name}");
        DamageInfo damageInfo = new DamageInfo(this);
        
        if(this.damageType == GameDefine.DamageType.Physical)
        {
            if(!isHit())
            {
                GameLogger.AddLog($"{target.name}(id : {target.characterId}) dodged");
                target.afterDamageSubject?.OnNext(damageInfo);
                return;
            }
        }
        damageInfo.SetBeforeDamage(damage =>
        {
            if(damage.damageType == GameDefine.DamageType.Physical)
            {
                damageInfo.damage = (int)GameDefine.GetAttackFix(damage.damage, damageInfo.target.def.Value);
            }
        });
        damageInfo.InvokeDamageAction();
        target.beforeDamageSubject?.OnNext(damageInfo);
        target.Damage(damageInfo);
        caster.afterAttackSubject?.OnNext(this);
        target.afterDamageSubject?.OnNext(damageInfo);
    }

    private bool isHit()
    {
        int hitRateRoll = UnityEngine.Random.Range(0, GameDefine.PERCENTAGE_MAX);
        int missRate = 1000 - caster.hitRateModifier.finalValue.Value;
        return hitRateRoll - target.dodgeRate.Value - missRate > 0;
    }
}
