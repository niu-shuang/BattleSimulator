using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualAttack : SkillBase
{
    public DualAttack(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
        this.description = description;
    }

    public int atkPercentage { get; private set; }
    public int critRate { get; private set; }
    public int critDamgeRate { get; private set; }

    public int hitRate { get; private set; }

    public override void LoadCustomProperty(ISheet sheet)
    {
        atkPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        critRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
        critDamgeRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
        hitRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 3).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        for(int i = 0; i < 2; i++)
        {
            var target = GameManager.Instance.GetRandomAttackTarget(caster.team.GetOpposite());
            if (target != null)
            {
                var atk = caster.atkModifier.finalValue.Value * atkPercentage / GameDefine.PERCENTAGE_MAX;
                var critAtk = caster.atkModifier.finalValue.Value * critDamgeRate / GameDefine.PERCENTAGE_MAX;
                AttackInfo attackInfo = new AttackInfo(caster, target, atk, GameDefine.DamageType.Physical, critRate, critAtk, hitRate);
                attackInfo.SetOnAttack(info => info.finalAtk = (int)(info.finalAtk * GameDefine.ATKMap[Mathf.Abs(target.pos.x - caster.pos.x)]));
                attackInfo.DoDamage();
            }
        }
        
        return base.Cast(targetPos, team);
    }
}
