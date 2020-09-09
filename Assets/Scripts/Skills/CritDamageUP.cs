using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritDamageUP : SkillBase
{
    public int continuousTurn { get; private set; }
    public int critDamageUpRate { get; private set; }
    public CritDamageUP(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        continuousTurn = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        critDamageUpRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        ModifyCritDamage buff = new ModifyCritDamage(critDamageUpRate);
        buff.Init(caster, caster, GameDefine.BuffTickType.Turn, false, continuousTurn);
        caster.AddBuff(buff);
        return base.Cast(targetPos, team);
    }

}
