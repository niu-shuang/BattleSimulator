using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
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
        atkPercentage = sheet.GetRow(6).GetCell(1).GetInt();   
    }

    public override void Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return;
        var target = GameManager.Instance.GetAttackTarget(team, targetPos.x);
        caster.Attack(target, atkPercentage);
        base.Cast(targetPos, team);
    }
}
