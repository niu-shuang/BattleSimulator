using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicalDisturbance : SkillBase
{
    public ElectronicalDisturbance(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target.team != caster.team && target.characterType == GameDefine.CharacterType.Mechanical)
        {
            ModifyAttackRate buff = new ModifyAttackRate(-250);
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, 1);
            target.AddBuff(buff);
            return base.Cast(targetPos, team);
        }
        else
            return false;
    }
}
