using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AttackInfo
{
    public CharacterLogic caster { get; private set; }
    public CharacterLogic target { get; private set; }

    public int finalAtk;

    public GameDefine.DamageType damageType { get; private set; }

    private Action<AttackInfo> onAttack;

    public AttackInfo(CharacterLogic caster, CharacterLogic target, int finalAttack, GameDefine.DamageType damageType)
    {
        this.caster = caster;
        this.target = target;
        this.finalAtk = finalAttack;
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
        caster.afterAttackSubject?.OnNext(this);
        GameLogger.AddLog($"{caster.name}(id : {caster.characterId}) deal {this.finalAtk} atk to {target.name}");
        DamageInfo damageInfo = new DamageInfo(this);
        target.beforeDamageSubject?.OnNext(damageInfo);
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
                float defPercentage = 1 - GameDefine.GetDefPercentage(target.def.Value);
                damageInfo.damage = (int)(damage.damage * defPercentage);
            }
        });
        damageInfo.InvokeDamageAction();
        target.Damage(damageInfo);
        target.afterDamageSubject?.OnNext(damageInfo);
    }

    private bool isHit()
    {
        int hitRate = UnityEngine.Random.Range(0, 1000);
        return hitRate - target.dodgeRate.Value > 0;
    }
}
