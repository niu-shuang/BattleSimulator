using Cysharp.Threading.Tasks;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlashAttack : SkillBase
{
    public FlashAttack(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var deck = SkillCardManager.Instance.currentDeckTeam1.Where(i=> i.skillType == SkillType.SelectableAttack);
        while(deck.Count() > 0)
        {
            var skill = deck.FirstOrDefault();
            var targets = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
            if (skill != null)
            {
                List<int> colHasCharas = new List<int>();
                foreach (var item in targets)
                {
                    if (!colHasCharas.Contains(item.pos.x))
                    {
                        colHasCharas.Add(item.pos.x);
                    }
                }
                var rand = Random.Range(0, colHasCharas.Count);
                skill.SetCostFree();
                var target = GameManager.Instance.GetAttackTarget(caster.team.GetOpposite(), colHasCharas[rand]);
                skill.Cast(target.pos, target.team);
                skill.ClearView();
                SkillCardManager.Instance.OnUseSkill(skill);
            }
        }
        return base.Cast(targetPos, team);
    }
}
