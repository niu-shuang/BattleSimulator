using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairMechanicalAll : SkillBase
{
    public int healPercentage { get; private set; }
    public RepairMechanicalAll(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        healPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        List<CharacterLogic> targets = new List<CharacterLogic>();
        var teamMate = GameManager.Instance.GetCharacters(caster.team);
        foreach (var item in teamMate)
        {
            if (item.characterType == GameDefine.CharacterType.Mechanical)
                targets.Add(item);
        }
        foreach (var target in targets)
        {
            HealInfo healInfo = new HealInfo(caster, target, target.maxHpModifier.finalValue.Value * healPercentage / GameDefine.PERCENTAGE_MAX);
            healInfo.DoHeal();
        }
        return base.Cast(targetPos, team);
    }
}
