using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipShield : SkillBase
{
    public int continuousTurn { get; private set; }
    public int damageCutRate { get; private set; }
    public int dodgeRate { get; private set; }
    public EquipShield(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        damageCutRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
        dodgeRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        DamageCut damageCutBuff = new DamageCut(damageCutRate);
        damageCutBuff.Init(caster, caster, GameDefine.BuffTickType.Damage, false, continuousTurn);
        caster.AddBuff(damageCutBuff);
        ModifyDodgeRate dodgeRateBuff = new ModifyDodgeRate(dodgeRate);
        dodgeRateBuff.Init(caster, caster, GameDefine.BuffTickType.Damage, false, continuousTurn);
        caster.AddBuff(dodgeRateBuff);
        return base.Cast(targetPos, team);
    }
}
