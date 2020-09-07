using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CharacterLogic
{
    public int characterId { get; private set; }
    public Vector2Int pos { get; private set; }
    public string name;
    public int baseHP { get; private set; }
    public Modifier hpModifier { get; private set; }
    public ReactiveProperty<int> Hp => hpModifier.finalValue;
    public Modifier maxHpModifier { get; private set; }
    public ReactiveProperty<int> maxHp => maxHpModifier.finalValue;
    public Team team { get; private set; }
    public int baseATK { get; private set; }
    public Modifier atkModifier { get; private set; }
    public ReactiveProperty<int> atk => atkModifier.finalValue;
    public int baseDef { get; private set; }
    public Modifier defModifier { get; private set; }
    public ReactiveProperty<int> def => defModifier.finalValue;

    public int baseDodgeRate { get; private set; }
    public Modifier dodgeRateModifier { get; private set; }
    public ReactiveProperty<int> dodgeRate => dodgeRateModifier.finalValue;

    public Modifier hitRateModifier { get; private set; }
    public Modifier critRateModifier { get; private set; }

    public ReactiveProperty<bool> isDead;

    public Subject<CharacterLog> info;

    public Subject<AttackInfo> beforeAttackSubject;
    public Subject<AttackInfo> afterAttackSubject;

    public Subject<DamageInfo> beforeDamageSubject;
    public Subject<DamageInfo> afterDamageSubject;

    public Subject<HealInfo> beforeHealSubject;
    public Subject<HealInfo> afterHealSubject;

    public ReactiveProperty<bool> isTaunt;
    public ReactiveProperty<bool> isBleeding;

    public bool isStun;

    public List<SkillBase> skills { get; private set; }

    public List<BuffBase> buffs { get; private set; }

    public GameDefine.CharacterType characterType { get; private set; }

    private CompositeDisposable disposable;


    public CharacterLogic(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int dodgeRate)
    {
        this.characterId = characterId;
        this.pos = pos;
        this.name = name;
        this.team = team;
        this.baseHP = maxHp;
        this.hpModifier = new Modifier(maxHp);
        this.maxHpModifier = new Modifier(maxHp);
        this.baseATK = atk;
        this.atkModifier = new Modifier(atk);
        this.baseDef = def;
        this.defModifier = new Modifier(def);
        this.baseDodgeRate = dodgeRate;
        this.dodgeRateModifier = new Modifier(dodgeRate);
        this.hitRateModifier = new Modifier(GameDefine.PERCENTAGE_MAX);
        this.critRateModifier = new Modifier(GameDefine.PERCENTAGE_MAX);
        this.isDead = new ReactiveProperty<bool>(false);
        this.info = new Subject<CharacterLog>();
        this.skills = new List<SkillBase>();
        this.buffs = new List<BuffBase>();
        this.characterType = characterType;
        this.isTaunt = new ReactiveProperty<bool>(false);
        this.isBleeding = new ReactiveProperty<bool>(false);
        this.isStun = false;
        disposable = new CompositeDisposable();

        beforeAttackSubject = new Subject<AttackInfo>();
        afterAttackSubject = new Subject<AttackInfo>();
        beforeDamageSubject = new Subject<DamageInfo>();
        afterDamageSubject = new Subject<DamageInfo>();
        beforeHealSubject = new Subject<HealInfo>();
        afterHealSubject = new Subject<HealInfo>();
        disposable.Add(isTaunt.Subscribe(taunt =>
        {
            if (taunt)
            {
                GameManager.Instance.RegisterTauntUnit(this);
            }
            else
            {
                GameManager.Instance.UnRegisterTauntUnit(this);
            }
        }));
        disposable.Add(Hp.Subscribe(hp =>
        {
            if (hp <= 0)
            {
                isDead.Value = true;
                isTaunt.Value = false;
                info.OnCompleted();
                beforeAttackSubject.OnCompleted();
                afterAttackSubject.OnCompleted();
                beforeDamageSubject.OnCompleted();
                afterDamageSubject.OnCompleted();
                beforeHealSubject.OnCompleted();
                afterHealSubject.OnCompleted();
                SkillCardManager.Instance.OnCharacterDead(this);
                foreach (var item in buffs)
                {
                    item.ForceEndBuff();
                }
                disposable.Dispose();
                Debug.Log($"{name} dead");
            }
        }));
    }

    public void AddSkill(SkillBase skill)
    {
        if(!skills.Contains(skill))
            skills.Add(skill);
    }

    public void AddBuff(BuffBase buff)
    {
        buffs.Add(buff);
    }

    public void RemoveBuff(BuffBase buff)
    {
        buffs.Remove(buff);
    }

    public void Attack(CharacterLogic target)
    {
        AttackInfo attackInfo = new AttackInfo(this, target, atkModifier.finalValue.Value, GameDefine.DamageType.Physical);
        attackInfo.SetOnAttack(info => info.finalAtk = (int)(info.finalAtk * GameDefine.ATKMap[Mathf.Abs(target.pos.x - pos.x)]));
        attackInfo.DoDamage();
    }

    public void Damage(DamageInfo damageInfo)
    {
        int finalDamage = damageInfo.damage;
        if (finalDamage < 0) finalDamage = 0;
        if (finalDamage > Hp.Value) finalDamage = Hp.Value;
        hpModifier.AddValueDirectly(-finalDamage);
        if(damageInfo.isCrit)
            GameLogger.AddLog($"{name}(Id{characterId}) receive {finalDamage}(Critical) damage");
        else
            GameLogger.AddLog($"{name}(Id{characterId}) receive {finalDamage} damage");
        CharacterLog log = new CharacterLog() { type = CharacterLogType.Damage, value = finalDamage, info = string.Empty };
        info.OnNext(log);
    }

    public void Heal(HealInfo healInfo)
    {
        if (isDead.Value == true) return;
        int finalHeal = healInfo.finalHeal;

        if (finalHeal + Hp.Value > maxHp.Value)
        {
            finalHeal = maxHp.Value - Hp.Value;
        }
        hpModifier.AddValueDirectly(finalHeal);
        GameLogger.AddLog($"{name}(Id{characterId}) get {finalHeal} heal");
        CharacterLog log = new CharacterLog() { type = CharacterLogType.Heal, value = finalHeal, info = string.Empty };
        info.OnNext(log);
    }
}

public enum Team
{
    Team1,
    Team2
}

public enum CharacterLogType
{
    Damage,
    Heal,
}

public struct CharacterLog
{
    public CharacterLogType type;
    public int value;
    public string info;
}