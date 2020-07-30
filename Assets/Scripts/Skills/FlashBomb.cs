using Boo.Lang;
using NPOI.SS.UserModel;
using UnityEngine;

public class FlashBomb : SkillBase
{
    public int aliveTime { get; private set; }
    public FlashBomb(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();   
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return false;
        var targets = GameManager.Instance.GetCharactersInOneCol(targetPos.x, team);
        if (targets.Count == 0) return false;
        foreach (var target in targets)
        {
            HitRateDown buff = new HitRateDown();
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, aliveTime);
            target.AddBuff(buff);
        }
        return base.Cast(targetPos, team);
    }
}
