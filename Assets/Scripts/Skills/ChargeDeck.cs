using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDeck : SkillBase
{
    public ChargeDeck(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var currentDeck = caster.team == Team.Team1 ? SkillCardManager.Instance.currentDeckTeam1 : SkillCardManager.Instance.currentDeckTeam2;
        var chargeCount = 6 - currentDeck.Count;
        if(chargeCount > 0)
        { 
            for(int i = 0; i < chargeCount; i++)
            {
                SkillCardManager.Instance.PickSkill(caster.team);
            }
        }
        SkillCardManager.Instance.ShowDeck(caster.team);
        return base.Cast(targetPos, team);
    }
}
