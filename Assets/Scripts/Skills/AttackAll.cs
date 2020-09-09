using NPOI.SS.UserModel;
using UnityEngine;

public class AttackAll : SkillBase
{
    public int atkPercentage { get; private set; }
    public int critRate { get; private set; }
    public int critDamgeRate { get; private set; }
    public int hitRate { get; private set; }

    public AttackAll(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        atkPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        critRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
        critDamgeRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
        hitRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 3).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var targets = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
        foreach (var target in targets)
        {
            var atk = caster.atkModifier.finalValue.Value * atkPercentage / GameDefine.PERCENTAGE_MAX;
            var critAtk = caster.atkModifier.finalValue.Value * critDamgeRate / GameDefine.PERCENTAGE_MAX;
            AttackInfo attackInfo = new AttackInfo(caster, target, atk, GameDefine.DamageType.Physical, critRate, critAtk, hitRate);
            attackInfo.DoDamage();
        }
        return base.Cast(targetPos, team);
    }
}
