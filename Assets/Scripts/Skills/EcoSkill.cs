using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EcoSkill : SkillBase
{
    public EcoSkill(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        GameManager.Instance.castSkillSubject
            .Take(1)
            .Subscribe(skillInfo =>
            {
                var clone = skillInfo.skill.Clone();
                clone.SetCostFree();
                clone.Cast(skillInfo.targetPos, skillInfo.team);

            });
        return base.Cast(targetPos, team);
    }
}
