using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corruption : SkillBase
{
    public int defDownRate { get; private set; }
    public int aliveTime { get; private set; }
    public Corruption(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        defDownRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if(target.team != caster.team && target.characterType == GameDefine.CharacterType.Mechanical)
        {
            ModifyDef buff = new ModifyDef(GameDefine.PERCENTAGE_MAX - defDownRate);
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, aliveTime);
            target.AddBuff(buff);
            GameLogger.AddLog($"corrupt {target.name} defDown { defDownRate / 10 }% for { aliveTime } turns");
            return base.Cast(targetPos, team);
        }
        else
        {
            return false;
        }
    }
}
