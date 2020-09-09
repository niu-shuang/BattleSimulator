using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Attack : SkillBase
{
    public int atkPercentage { get; private set; }
    public int critRate { get; private set; }
    public int critDamageRate { get; private set; }

    public int hitRate { get; private set; }
    public Attack(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
        this.description = description;
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        atkPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        critRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
        critDamageRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetInt();
        hitRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 3).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return false;
        var target = GameManager.Instance.GetAttackTarget(team, targetPos.x);
        if (target == null) return false;
        var atk = caster.atkModifier.finalValue.Value * atkPercentage / GameDefine.PERCENTAGE_MAX;
        var critAtk = caster.atkModifier.finalValue.Value * critDamageRate / GameDefine.PERCENTAGE_MAX;
        AttackInfo attackInfo = new AttackInfo(caster, target, atk, GameDefine.DamageType.Physical, critRate, critAtk, hitRate);
        attackInfo.SetOnAttack(info => info.finalAtk = (int)(info.finalAtk * GameDefine.ATKMap[Mathf.Abs(target.pos.x - caster.pos.x)]));
        attackInfo.DoDamage();
        return base.Cast(targetPos, team);
    }
}
