﻿using System.Collections.Generic;
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

    public ReactiveProperty<bool> isDead;

    public Subject<string> info;

    public Subject<AttackInfo> beforeAttackSubject;
    public Subject<AttackInfo> afterAttackSubject;

    public Subject<DamageInfo> beforeDamageSubject;
    public Subject<DamageInfo> afterDamageSubject;

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
        this.isDead = new ReactiveProperty<bool>(false);
        this.info = new Subject<string>();
        this.skills = new List<SkillBase>();
        this.buffs = new List<BuffBase>();
        this.characterType = characterType;
        disposable = new CompositeDisposable();

        beforeAttackSubject = new Subject<AttackInfo>();
        afterAttackSubject = new Subject<AttackInfo>();
        beforeDamageSubject = new Subject<DamageInfo>();
        afterDamageSubject = new Subject<DamageInfo>();
        disposable.Add(Hp.Subscribe(hp =>
        {
            if (hp <= 0)
            {
                isDead.Value = true;
                info.OnCompleted();
                beforeAttackSubject.OnCompleted();
                afterAttackSubject.OnCompleted();
                beforeDamageSubject.OnCompleted();
                afterDamageSubject.OnCompleted();
                disposable.Dispose();
            }
        }));
    }

    public void AddSkill(SkillBase skill)
    {
        skills.Add(skill);
    }

    public void AddBuff(BuffBase buff, CharacterLogic caster)
    {
        buffs.Add(buff);
        buff.Init(this, caster);
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
        Hp.Value -= finalDamage;
        GameLogger.AddLog($"{name}(Id{characterId}) receive {finalDamage} damage");
        info.OnNext($"-{finalDamage}");
    }
}

public enum Team
{
    Team1,
    Team2
}
