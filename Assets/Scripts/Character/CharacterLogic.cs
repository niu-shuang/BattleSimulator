using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CharacterLogic
{
    public int characterId { get; private set; }
    public Vector2Int pos { get; private set; }
    public string name;
    public int baseHP { get; private set; }
    public ReactiveProperty<int> Hp;
    public ReactiveProperty<int> maxHp;
    public Team team { get; private set; }
    public int baseATK { get; private set; }
    public ReactiveProperty<int> atk;
    public int baseDef { get; private set; }
    public ReactiveProperty<int> def;
    public ReactiveProperty<bool> isDead;

    public Subject<string> info;

    public List<SkillBase> skills { get; private set; }

    public List<BuffBase> buffs { get; private set; }

    public GameDefine.CharacterType characterType { get; private set; }

    private CompositeDisposable disposable;

    public CharacterLogic(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType)
    {
        this.characterId = characterId;
        this.pos = pos;
        this.name = name;
        this.team = team;
        this.baseHP = maxHp;
        this.maxHp = new ReactiveProperty<int>(maxHp);
        this.Hp = new ReactiveProperty<int>(maxHp);
        this.baseATK = atk;
        this.atk = new ReactiveProperty<int>(atk);
        this.baseDef = def;
        this.def = new ReactiveProperty<int>(def);
        this.isDead = new ReactiveProperty<bool>(false);
        this.info = new Subject<string>();
        this.skills = new List<SkillBase>();
        this.buffs = new List<BuffBase>();
        this.characterType = characterType;
        disposable = new CompositeDisposable();

        disposable.Add(Hp.Subscribe(hp =>
        {
            if (hp <= 0)
            {
                isDead.Value = true;
                info.OnCompleted();
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

    public void Attack(CharacterLogic target, int percentage)
    {
        int finalATK = (int)(atk.Value * GameDefine.ATKMap[Mathf.Abs(target.pos.x - pos.x)] * percentage / 100f);
        GameLogger.AddLog($"{name}(Id{characterId}) attack {target.name}(Id{target.characterId}) deal {finalATK} damage");
        target.Damage(finalATK);
    }

    public void Damage(int damage)
    {
        int finalDamage = (int)(damage * (1 - GameDefine.GetDefPercentage(def.Value)));
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
