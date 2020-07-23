using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charm : SkillBase
{
    public Charm(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var targetList = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
        if(targetList.Count > 1 )
        {
            var rand = Random.Range(0, targetList.Count);
            var target = targetList[rand];
            targetList.Remove(target);
            var randAttackTarget = Random.Range(0, targetList.Count);
            var attackTarget = targetList[randAttackTarget];
            target.Attack(attackTarget);
            return base.Cast(targetPos, team);
        }
        else
        {
            return false;
        }
    }
}
