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

    public override void Cast(Vector2Int targetPos, Team team)
    {
        base.Cast(targetPos, team);
        var drones = new List<SummonedCharacter>();
        for(int i = 0; i < 3; i++)
        {
            var chara = GameManager.Instance.GetCharacter(new Vector2Int(i, 0), caster.team);
            if(chara is SummonedCharacter)
            {
                drones.Add(chara as SummonedCharacter);
            }
        }
        if (drones.Count == 0) return;
        var rand = Random.Range(0, drones.Count);
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        target.Damage((int)(drones[rand].Hp.Value * .5f));
        drones[rand].Hp.Value = 0;
    }
}
