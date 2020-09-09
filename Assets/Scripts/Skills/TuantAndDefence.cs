using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuantAndDefence : SkillBase
{
    public int continuousTurn { get; private set; }
    public int damageCutRate { get; private set; }
    public TuantAndDefence(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        damageCutRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        Taunt tuant = new Taunt();
        tuant.Init(caster, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        caster.AddBuff(tuant);
        DamageCut damageCut = new DamageCut(damageCutRate);
        damageCut.Init(caster, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        caster.AddBuff(damageCut);
        return base.Cast(targetPos, team);
    }
}
