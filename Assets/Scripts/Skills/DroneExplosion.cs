using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosion : SkillBase
{
    public DroneExplosion(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        SummonedCharacter drone = null;
        var chara = GameManager.Instance.GetCharacter(targetPos, caster.team);
        if(chara is SummonedCharacter)
        {
            drone = chara as SummonedCharacter;
        }
        if (drone == null) return false;
        var target = GameManager.Instance.GetAttackTarget(team.GetOpposite(), targetPos.x);
        AttackInfo info = new AttackInfo(drone, target, (int)(drone.Hp.Value * .5f), GameDefine.DamageType.Magical);
        info.DoDamage();
        drone.Hp.Value = 0;
        OnCastSuc();
        return true;
    }
}
