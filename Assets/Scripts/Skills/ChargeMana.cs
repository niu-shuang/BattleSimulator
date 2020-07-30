using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMana : SkillBase
{
    public ChargeMana(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        GameManager.Instance.mana[(int)caster.team].Value = GameManager.Instance.maxMana[(int)caster.team].Value + cost;
        return base.Cast(targetPos, team);
    }
}
