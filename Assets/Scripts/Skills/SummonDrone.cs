﻿using NPOI.SS.UserModel;
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

    public SummonDrone(int id, string skillName, SkillType skillType,int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {

    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var info = CreateCharacterInfo();
        GameManager.Instance.AddSummonCharacter(caster.team, caster.pos + new Vector2Int(0, -1), info, aliveTime);
        return base.Cast(targetPos, team);
    }

    protected CharacterInfo CreateCharacterInfo()
    {
        CharacterInfo info = new CharacterInfo()
        {
            characterId = -1,
            characterName = droneName,
            icon = droneIcon,
            hp = (int)(caster.baseHP * droneHp / 1000f),
            atk = droneAtk,
            def = (int)(caster.baseDef * droneDef / 1000f),
            characterType = GameDefine.CharacterType.Mechanical.ToString(),
            dodgeRate = caster.baseDodgeRate
        };
        return info;
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        this.droneName = sheet.GetRow(6).GetCell(1).GetString();
        this.droneIcon = sheet.GetRow(7).GetCell(1).GetString();
        this.droneHp = sheet.GetRow(8).GetCell(1).GetInt();
        this.droneAtk = sheet.GetRow(9).GetCell(1).GetInt();
        this.droneDef = sheet.GetRow(10).GetCell(1).GetInt();
        this.aliveTime = sheet.GetRow(11).GetCell(1).GetInt();
    }
}
