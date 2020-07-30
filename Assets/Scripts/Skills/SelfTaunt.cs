using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTaunt : SkillBase
{
    public SelfTaunt(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        Taunt buff = new Taunt();
        buff.Init(caster, caster, GameDefine.BuffTickType.Turn, false, 1);
        return base.Cast(targetPos, team);
    }
}
