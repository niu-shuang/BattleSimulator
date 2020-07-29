using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeAttack : SkillBase
{
    public FreeAttack(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var targets = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
        if(targets.Count > 0)
        {
            List<int> colHasCharas = new List<int>();
            foreach (var item in targets)
            {
                if(!colHasCharas.Contains(item.pos.x))
                {
                    colHasCharas.Add(item.pos.x);
                }
            }
            var rand = Random.Range(0, colHasCharas.Count);
            caster.Attack(GameManager.Instance.GetAttackTarget(caster.team.GetOpposite(), colHasCharas[rand]));
            return base.Cast(targetPos, team);
        }
        return false;
    }
}
