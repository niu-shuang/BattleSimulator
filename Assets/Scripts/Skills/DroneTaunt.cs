using NPOI.SS.UserModel;
using UnityEngine;

public class DroneTaunt : SkillBase
{
    public DroneTaunt(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        if (team != caster.team) return false;
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target == null) return false;
        if (!(target is SummonedCharacter)) return false;
        var drone = target as SummonedCharacter;
        Taunt buff = new Taunt();
        buff.Init(drone, caster, GameDefine.BuffTickType.Turn, false, 3);
        drone.AddBuff(buff);
        return base.Cast(targetPos, team);
    }
}
