using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegenerateDeck : SkillBase
{
    public RegenerateDeck(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var skills = SkillCardManager.Instance.currentDeckTeam1.Where(i => i != this).ToList();
        foreach (var item in skills)
        {
            item.ClearView();
            SkillCardManager.Instance.OnUseSkill(item);
        }
        for(int i = 0; i < 5; i++)
        {
            SkillCardManager.Instance.PickSkill(caster.team);
        }
        SkillCardManager.Instance.ShowDeck(caster.team);
        return base.Cast(targetPos, team);
    }
}
