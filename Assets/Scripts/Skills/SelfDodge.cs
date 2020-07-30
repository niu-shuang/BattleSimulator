using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDodge : SkillBase
{
    public int aliveTime { get; private set; }
    public SelfDodge(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        UnPerfectDodge buff = new UnPerfectDodge();
        buff.Init(caster, caster, GameDefine.BuffTickType.Damage, false, aliveTime);
        return base.Cast(targetPos, team);
    }
}
