using NPOI.SS.UserModel;
using UniRx;
using UnityEngine;

public abstract class SkillBase
{
    public int id { get; private set; }
    public string skillName { get; private set; }
    /// <summary>
    /// 是否需要选取目标
    /// </summary>
    public bool selectable { get; private set; }
    /// <summary>
    /// 施法者
    /// </summary>
    public CharacterLogic caster { get; private set; }

    public SkillType skillType { get; private set; }

    public enum SkillType
    {
        SelectableAttack,
        UnSelectableAttack,
        Summon,
        Buff,
        Heal
    }

    public string description { get; protected set; }

    public int cost { get; private set; }
    private int baseCost;
    private int castTurn;

    private SkillCardView view;
    public bool casted { get; private set; }

    public CompositeDisposable disposable;

    public bool canCast => GameManager.Instance.mana[(int)caster.team].Value - cost >= 0 && !caster.isStun;

    public SkillBase(int id, string skillName, SkillType skillType , int cost, bool selectable, CharacterLogic caster, string description)
    {
        this.id = id;
        this.skillName = skillName;
        this.skillType = skillType;
        this.selectable = selectable;
        this.caster = caster;
        this.cost = cost;
        this.description = description;
        this.casted = false;
        baseCost = cost;
        disposable = new CompositeDisposable();
        castTurn = -1;
    }

    public virtual bool Cast(Vector2Int targetPos, Team team)
    {
        castTurn = GameManager.Instance.turn;
        GameManager.Instance.mana[(int)caster.team].Value -= cost;
        casted = true;
        return true;
    }

    public void SetCostFree()
    {
        cost = 0;
    }

    public void Reset()
    {
        casted = false;
        cost = baseCost;
        disposable.Dispose();
        disposable = new CompositeDisposable();
    }

    public void SetView(SkillCardView view)
    {
        this.view = view;
    }

    public void ClearView()
    {
        view?.SetEmpty();
    }

    public abstract void LoadCustomProperty(ISheet sheet);

    public SkillBase Clone()
    {
        return this.MemberwiseClone() as SkillBase;
    }
}
