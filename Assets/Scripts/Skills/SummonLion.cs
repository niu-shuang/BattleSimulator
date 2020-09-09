using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonLion : SkillBase
{
    public string droneName { get; private set; }
    public string droneIcon { get; private set; }
    public int droneHp { get; private set; }
    public int droneAtk { get; private set; }
    public int droneDef { get; private set; }

    public int aliveTime { get; private set; }
    public SummonLion(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        this.droneName = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetString();
        this.droneIcon = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetString();
        this.droneHp = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
        this.droneAtk = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 3).GetCell(1).GetInt();
        this.droneDef = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 4).GetCell(1).GetInt();
        this.aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 5).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var pos = caster.pos + new Vector2Int(0, -1);
        Lion drone = new Lion(-1, pos, droneName, droneHp, caster.team, droneAtk, droneDef, GameDefine.CharacterType.Biological, 0, aliveTime);
        GameManager.Instance.AddSummonCharacter(drone, droneIcon);
        return base.Cast(targetPos, team);
    }
}
