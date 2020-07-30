using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddShieldAuto : SkillBase
{
    public AddShieldAuto(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }


    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharactersInOneCol(caster.pos.x, caster.team);
        Shield buff = new Shield();
        buff.Init(target.FirstOrDefault(), caster, GameDefine.BuffTickType.Damage, true, -1);
        target.FirstOrDefault().AddBuff(buff);
        return base.Cast(targetPos, team);
    }
}
