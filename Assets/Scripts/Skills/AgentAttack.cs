using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AgentAttack : SkillBase
{
    public AgentAttack(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        if (team == caster.team) return false;
        var target = GameManager.Instance.GetAttackTarget(team, targetPos.x);
        disposable.Add(caster.beforeAttackSubject
            .Subscribe(attackInfo =>
            {
                switch (attackInfo.target.characterType)
                {
                    case GameDefine.CharacterType.Biological:
                        attackInfo.finalAtk = (int)(attackInfo.finalAtk * 1200 / 1000f);
                        break;
                    case GameDefine.CharacterType.Mechanical:
                        attackInfo.finalAtk = (int)(attackInfo.finalAtk * 800 / 1000f);
                        break;
                    default:
                        break;
                }

            }));
        caster.Attack(target);
        return base.Cast(targetPos, team);
    }
}
