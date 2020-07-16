using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyDodge : SkillBase
{
    public int dodgeRate { get; private set; }
    public EmergencyDodge(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        dodgeRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override void Cast(Vector2Int targetPos, Team team)
    {
        base.Cast(targetPos, team);
        PerfectDodgeOnce buff = new PerfectDodgeOnce();
        buff.Init(caster, caster, GameDefine.BuffTickType.Damage, false, 1);
    }
}
