using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : SkillBase
{
    public int healPercentage { get; private set; }
    public Heal(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        healPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target == null || target.team != caster.team || target.characterType == GameDefine.CharacterType.Mechanical) return false;
        HealInfo healInfo = new HealInfo(caster, target, (int)(target.maxHp.Value * healPercentage / GameDefine.PERCENTAGE_MAX));
        healInfo.DoHeal();
        return base.Cast(targetPos, team);
    }
}
