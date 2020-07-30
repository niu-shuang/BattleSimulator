using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Attack : SkillBase
{
    public int atkPercentage { get; private set; }
    public Attack(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
        this.description = description;
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        atkPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();   
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return false;
        var target = GameManager.Instance.GetAttackTarget(team, targetPos.x);
        disposable.Add(caster.beforeAttackSubject
            .Subscribe(attackInfo =>
            {
                attackInfo.finalAtk = (int)(attackInfo.finalAtk * atkPercentage / 1000f);
            }));
        caster.Attack(target);
        return base.Cast(targetPos, team);
    }
}
