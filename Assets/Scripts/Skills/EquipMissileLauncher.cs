using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMissileLauncher : SkillBase
{
    public int continuousTurn { get; private set; }
    public int atkPercentage { get; private set; }
    public int critRate { get; private set; }
    public int critDamage { get; private set; }
    public int hitRate { get; private set; }
    public EquipMissileLauncher(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        atkPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW+1).GetCell(1).GetInt();
        critRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
        critDamage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 3).GetCell(1).GetInt();
        hitRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 4).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        MissileLauncher buff = new MissileLauncher(atkPercentage, critRate, critDamage, hitRate);
        buff.Init(caster, caster, GameDefine.BuffTickType.Attack, false, continuousTurn);
        caster.AddBuff(buff);
        return base.Cast(targetPos, team);
    }
}
