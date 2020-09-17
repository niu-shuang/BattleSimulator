using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMana : SkillBase
{
    public int recoverMana { get; private set; }
    public ChargeMana(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        recoverMana = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        int recoverValue = recoverMana;
        int diffValue = GameManager.Instance.maxMana[(int)caster.team].Value - GameManager.Instance.mana[(int)caster.team].Value;
        if (diffValue < recoverMana)
            recoverValue = diffValue;
        GameManager.Instance.mana[(int)caster.team].Value = GameManager.Instance.mana[(int)caster.team].Value + recoverValue;
        return base.Cast(targetPos, team);
    }
}
