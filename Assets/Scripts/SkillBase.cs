using NPOI.SS.UserModel;
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

    public int coolDown { get; private set; }
    private int castTurn;

    public bool canCast => castTurn == -1 || (castTurn != -1 && GameManager.Instance.turn - castTurn >= coolDown);

    public SkillBase(int id, string skillName, int coolDown, bool selectable, CharacterLogic caster)
    {
        this.id = id;
        this.skillName = skillName;
        this.selectable = selectable;
        this.caster = caster;
        this.coolDown = coolDown;
        castTurn = -1;
    }

    public virtual void Cast(Vector2Int targetPos)
    {
        castTurn = GameManager.Instance.turn;
    }

    public abstract void LoadCustomProperty(ISheet sheet);
}
