using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : SkillBase
{
    public int healPercentage { get; private set; }
    public Heal(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        healPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override void Cast(Vector2Int targetPos, Team team)
    {
        base.Cast(targetPos, team);
        HealInfo healInfo = new HealInfo(caster, caster, (int)(caster.maxHp.Value * healPercentage / 1000f));
        healInfo.DoHeal();
    }
}
