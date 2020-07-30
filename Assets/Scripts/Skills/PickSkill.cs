using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickSkill : SkillBase
{
    public PickSkill(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var deck = caster.team == Team.Team1 ? SkillCardManager.Instance.currentDeckTeam1 : SkillCardManager.Instance.currentDeckTeam2;
        var tempDeck = new List<SkillBase>();
        foreach (var item in deck)
        {
            if(item != this)
            {
                tempDeck.Add(item);
            }
        }
        if(tempDeck.Count > 0)
        {
            var rand = Random.Range(0, tempDeck.Count);
            var targetCard = tempDeck[rand];
            targetCard.ClearView();
            SkillCardManager.Instance.OnUseSkill(targetCard);
        }
        for(int i = 0; i < 2; i++)
        {
            SkillCardManager.Instance.PickSkill(caster.team);
        }
        SkillCardManager.Instance.ShowDeck(caster.team);
        return base.Cast(targetPos, team);
    }
}
