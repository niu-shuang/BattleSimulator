using NPOI.SS.UserModel;
using NPOI.Util;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courage : SkillBase
{
    public Courage(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var targets = GameManager.Instance.GetCharacters(caster.team);
        foreach (var target in targets)
        {
            IncreaseAttack buff = new IncreaseAttack();
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, 1);
            target.AddBuff(buff);
        }
        return base.Cast(targetPos, team);
    }
}
