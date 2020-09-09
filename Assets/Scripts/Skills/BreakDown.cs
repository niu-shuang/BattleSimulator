using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakDown : SkillBase
{
    public int continuousTurn { get; private set; }
    public int defRate { get; private set; }
    public BreakDown(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        defRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target == null || target.team == caster.team) return false;
        ModifyDef buff = new ModifyDef(defRate);
        buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        target.AddBuff(buff);
        return base.Cast(targetPos, team);
    }
}
