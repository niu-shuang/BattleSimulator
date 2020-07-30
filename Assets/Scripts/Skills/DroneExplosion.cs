using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosion : SkillBase
{
    public DroneExplosion(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        Drone drone = null;
        var chara = GameManager.Instance.GetCharacter(targetPos, caster.team);
        if(chara is Drone)
        {
            drone = chara as Drone;
        }
        if (drone == null) return false;
        var target = GameManager.Instance.GetAttackTarget(team.GetOpposite(), targetPos.x);
        if(target != null)
        {
            DoAttack(drone, target);
            return base.Cast(targetPos, team);
        }else
        {
            var targets = GameManager.Instance.GetCharacters(caster.team.GetOpposite());
            List<int> colHasCharas = new List<int>();
            foreach (var item in targets)
            {
                if (!colHasCharas.Contains(item.pos.x))
                {
                    colHasCharas.Add(item.pos.x);
                }
            }
            if(colHasCharas.Count > 0)
            {
                target = GameManager.Instance.GetAttackTarget(team.GetOpposite(), colHasCharas[0]);
                DoAttack(drone, target);
                return base.Cast(targetPos, team);
            }
            else
            {
                return false;
            }
        }
        
    }

    private void DoAttack(SummonedCharacter drone, CharacterLogic target)
    {
        AttackInfo info = new AttackInfo(drone, target, (int)(drone.Hp.Value * 750 / GameDefine.PERCENTAGE_MAX), GameDefine.DamageType.Magical);
        info.DoDamage();
        drone.Hp.Value = 0;
    }
}
