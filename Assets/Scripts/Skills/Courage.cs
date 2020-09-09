using NPOI.SS.UserModel;
using NPOI.Util;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courage : SkillBase
{
    public int continuousTurn { get; private set; }
    public int percentage { get; private set; }
    public Courage(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        percentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target == null || target.team != caster.team) return false;
        ModifyAttackRate buff = new ModifyAttackRate(percentage);
        buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, 3);
        target.AddBuff(buff);
        return base.Cast(targetPos, team);
    }
}
