using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CostReduction : SkillBase
{
    public int reductionValue { get; private set; }
    public CostReduction(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        reductionValue = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var skills = SkillCardManager.Instance.currentDeckTeam1.Where(i => i != this);
        foreach (var item in skills)
        {
            item.ReductCost(reductionValue);
        }
        return base.Cast(targetPos, team);
    }

}
