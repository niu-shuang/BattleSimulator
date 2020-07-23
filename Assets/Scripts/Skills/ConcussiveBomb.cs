using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcussiveBomb : SkillBase
{
    public ConcussiveBomb(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return false;
        var targets = GameManager.Instance.GetCharactersInOneCol(targetPos.x, team);
        if (targets.Count == 0) return false;
        foreach (var target in targets)
        {
            var rand = Random.Range(0, GameDefine.PERCENTAGE_MAX);
            if(rand >= GameDefine.PERCENTAGE_MAX / 2f)
            {
                Stune buff = new Stune();
                buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, 1);
                target.AddBuff(buff);
            }
        }
        return base.Cast(targetPos, team);
    }
}
