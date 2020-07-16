using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Attack : SkillBase
{
    public int atkPercentage { get; private set; }
    public Attack(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
        this.description = description;
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        atkPercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();   
    }

    public override void Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return;
        var target = GameManager.Instance.GetAttackTarget(team, targetPos.x);
        disposable.Add(caster.beforeAttackSubject
            .Subscribe(attackInfo =>
            {
                attackInfo.finalAtk = (int)(attackInfo.finalAtk * atkPercentage / 1000f);
            }));
        caster.Attack(target);
        base.Cast(targetPos, team);
    }
}
