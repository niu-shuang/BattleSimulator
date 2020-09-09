﻿using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : SkillBase
{
    public int hitRate { get; private set; }
    public int dodgeRate { get; private set; }
    public SmokeGrenade(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        hitRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        dodgeRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var targets = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
        foreach (var item in targets)
        {
            ModifyDodgeRate dodgeRateBuff = new ModifyDodgeRate(dodgeRate);
            dodgeRateBuff.Init(item, caster, GameDefine.BuffTickType.Turn, false, 1);
            item.AddBuff(dodgeRateBuff);
            ModifyHitRate hitRateBuff = new ModifyHitRate(hitRate);
            hitRateBuff.Init(item, caster, GameDefine.BuffTickType.Turn, false, 1);
            item.AddBuff(hitRateBuff);
        }
        return base.Cast(targetPos, team);
    }
}