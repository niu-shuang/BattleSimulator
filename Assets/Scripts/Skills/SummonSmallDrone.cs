using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSmallDrone : SkillBase
{
    public int aliveTime { get; private set; }
    public string name { get; private set; }

    public string icon { get; private set; }

    public int inheritHpRate { get; private set; }
    public int inheriteAtkRate { get; private set; }

    public SummonSmallDrone(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
        
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        name = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetString();
        icon = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetString();
        inheritHpRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 3).GetCell(1).GetInt();
        inheriteAtkRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 4).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var hp = caster.baseHP * inheritHpRate / GameDefine.PERCENTAGE_MAX;
        var atk = caster.baseATK * inheriteAtkRate / GameDefine.PERCENTAGE_MAX;
        SmallDrone character = new SmallDrone(-1, new Vector2Int(caster.pos.x, 0), name, hp , caster.team, atk, caster.baseDef, GameDefine.CharacterType.Biological, caster.baseDodgeRate, aliveTime);
        GameManager.Instance.AddSummonCharacter(character, icon);
        return base.Cast(targetPos, team);
    }
}
