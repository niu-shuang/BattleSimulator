using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfCounterAttack : SkillBase
{
    public int counterRate { get; private set; }
    public int aliveTime { get; private set; }
    public SelfCounterAttack(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        counterRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        CounterAttack buff = new CounterAttack(counterRate);
        buff.Init(caster, caster, GameDefine.BuffTickType.Damage, false, aliveTime);
        return base.Cast(targetPos, team);
    }
}
