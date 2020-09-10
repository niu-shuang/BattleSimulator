using System;

public class AttackInfo
{
    public CharacterLogic caster { get; private set; }
    public CharacterLogic target { get; private set; }

    public int baseAtk { get; private set; }
    public int finalAtk;
    public int critRate;
    public int critDamageRate;

    public int hitRate;

    public GameDefine.DamageType damageType { get; private set; }

    private Action<AttackInfo> onAttack;

    public AttackInfo(CharacterLogic caster, CharacterLogic target, int baseAtk, GameDefine.DamageType damageType, int critRate = 0, int critDamage = 1000, int hitRate = 1000)
    {
        this.caster = caster;
        this.target = target;
        this.baseAtk = baseAtk;
        this.finalAtk = baseAtk;
        this.damageType = damageType;
        this.critRate = critRate;
        this.critDamageRate = critDamage;
        this.hitRate = hitRate;
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
        if(isCrit())
        {
            damageInfo.isCrit = true;
            damageInfo.damage = critDamageRate * caster.critDamageRateModifier.finalValue.Value / GameDefine.PERCENTAGE_MAX;
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

    public void DoDamageSilent()
    {
        GameLogger.AddLog($"{caster.name}(id : {caster.characterId}) deal {this.finalAtk} atk to {target.name}");
        DamageInfo damageInfo = new DamageInfo(this);

        if (this.damageType == GameDefine.DamageType.Physical)
        {
            if (!isHit())
            {
                GameLogger.AddLog($"{target.name}(id : {target.characterId}) dodged");
                target.afterDamageSubject?.OnNext(damageInfo);
                return;
            }
        }
        if (isCrit())
        {
            damageInfo.isCrit = true;
            damageInfo.damage = critDamageRate * caster.critDamageRateModifier.finalValue.Value / GameDefine.PERCENTAGE_MAX;
        }
        damageInfo.SetBeforeDamage(damage =>
        {
            if (damage.damageType == GameDefine.DamageType.Physical)
            {
                damageInfo.damage = (int)GameDefine.GetAttackFix(damage.damage, damageInfo.target.def.Value);
            }
        });
        damageInfo.InvokeDamageAction();
        target.beforeDamageSubject?.OnNext(damageInfo);
        target.Damage(damageInfo);
        target.afterDamageSubject?.OnNext(damageInfo);
    }

    private bool isHit()
    {
        int hitRateRoll = UnityEngine.Random.Range(0, GameDefine.PERCENTAGE_MAX);
        int missRate = GameDefine.PERCENTAGE_MAX - (caster.hitRateModifier.finalValue.Value * hitRate / GameDefine.PERCENTAGE_MAX);
        return hitRateRoll - target.dodgeRate.Value - missRate > 0;
    }

    /// <summary>
    /// 技能自带的暴击率 * 人物的暴击率 >= roll点
    /// </summary>
    /// <returns></returns>
    private bool isCrit()
    {
        int critRateRoll = UnityEngine.Random.Range(0, GameDefine.PERCENTAGE_MAX);
        var modifiedCritRate = critRate * caster.critRateModifier.finalValue.Value / GameDefine.PERCENTAGE_MAX;
        return modifiedCritRate - critRateRoll >= 0;
    }
}
