using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeRateUp : SkillBase
{
    public int upRate { get; private set; }
    public int aliveTime { get; private set; }
    public DodgeRateUp(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        upRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        ModifyDodgeRate buff = new ModifyDodgeRate(upRate);
        return base.Cast(targetPos, team);
    }
}
