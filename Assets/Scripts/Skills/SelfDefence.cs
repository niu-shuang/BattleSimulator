using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDefence : SkillBase
{
    public int continuousTurn { get; private set; }
    public int damageCutRate { get; private set; }
    public int atkDownRate { get; private set; }
    public SelfDefence(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        damageCutRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
        atkDownRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        DamageCut buff1 = new DamageCut(damageCutRate);
        buff1.Init(caster, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        caster.AddBuff(buff1);
        ModifyAttackRate buff2 = new ModifyAttackRate(atkDownRate);
        buff2.Init(caster, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        caster.AddBuff(buff2);
        return base.Cast(targetPos, team);
    }
}
