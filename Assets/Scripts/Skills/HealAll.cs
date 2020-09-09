using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAll : SkillBase
{
    public int healPercentage { get; private set; }
    public HealAll(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        healPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        List<CharacterLogic> targets = new List<CharacterLogic>();
        for (int i = 0; i < 3; i++)
        {
            targets.AddRange(GameManager.Instance.GetCharactersInOneCol(i, caster.team));
        }
        foreach (var item in targets)
        {
            if (item.characterType == GameDefine.CharacterType.Mechanical) continue;
            HealInfo healInfo = new HealInfo(caster, item, (int)(item.maxHp.Value * healPercentage / GameDefine.PERCENTAGE_MAX));
            healInfo.DoHeal();
        }
        return base.Cast(targetPos, team);
    }
}
