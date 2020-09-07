using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Concentration : SkillBase
{
    public int continuousTurn { get; private set; }
    public int critRateUp { get; private set; }
    public Concentration(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
        
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        critRateUp = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        ModifyCritRate buff = new ModifyCritRate(critRateUp);
        buff.Init(caster, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        caster.AddBuff(buff);
        return base.Cast(targetPos, team);
    }

}
