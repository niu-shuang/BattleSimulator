using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDrone : SkillBase
{
    public string droneName { get; private set; }
    public string droneIcon { get; private set; }
    public int droneHp { get; private set; }
    public int droneAtk { get; private set; }
    public int droneDef { get; private set; }

    public int aliveTime { get; private set; }

    public SummonDrone(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {

    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var pos = caster.pos + new Vector2Int(0, -1);
        var hp = caster.baseHP * droneHp / GameDefine.PERCENTAGE_MAX;
        var atk = caster.baseATK * droneAtk / GameDefine.PERCENTAGE_MAX;
        var def = caster.baseDef * droneDef / GameDefine.PERCENTAGE_MAX;

        Drone drone = new Drone(-1, pos, droneName, hp, caster.team, atk, def, GameDefine.CharacterType.Mechanical, caster.baseDodgeRate, aliveTime);
        GameManager.Instance.AddSummonCharacter(drone, droneIcon);
        return base.Cast(targetPos, team);
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
}
