using NPOI.SS.UserModel;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeCardPool : SkillBase
{
    public ExchangeCardPool(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var cardPool = team == Team.Team1 ? SkillCardManager.Instance.team1CardPool : SkillCardManager.Instance.team2CardPool;
        var usedCardPool = team == Team.Team1 ? SkillCardManager.Instance.team1UsedCardPool : SkillCardManager.Instance.team2UsedCardPool;
        List<SkillBase> cardPoolSkillList = new List<SkillBase>();
        cardPool.ForEach(i => cardPoolSkillList.Add(i));
        List<SkillBase> usedCardPoolSkillList = new List<SkillBase>();
        usedCardPool.ForEach(i => usedCardPoolSkillList.Add(i));
        cardPool.Clear();
        usedCardPool.Clear();
        cardPoolSkillList.ForEach(i => usedCardPool.Add(i));
        usedCardPoolSkillList.ForEach(i => cardPool.Add(i));
        return base.Cast(targetPos, team);
    }
}
