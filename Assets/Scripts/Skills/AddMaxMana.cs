using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMaxMana : SkillBase
{
    public AddMaxMana(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        GameManager.Instance.maxMana[(int)caster.team].Value += 1;
        return base.Cast(targetPos, team);
    }
}
