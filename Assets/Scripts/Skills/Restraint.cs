using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restraint : SkillBase
{
    public int continuousTurn { get; private set; }
    public int atkRate { get; private set; }
    public Restraint(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        atkRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target == null) return false;
        ModifyAttackRate buff = new ModifyAttackRate(atkRate);
        buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        target.AddBuff(buff);
        return base.Cast(targetPos, team);
    }
}
