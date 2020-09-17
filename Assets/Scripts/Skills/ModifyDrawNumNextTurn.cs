using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyDrawNumNextTurn : SkillBase
{
    public int drawNum { get; private set; }
    public ModifyDrawNumNextTurn(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        drawNum = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        SkillCardManager.Instance.overrideDrawNum = drawNum;
        return base.Cast(targetPos, team);
    }
}
