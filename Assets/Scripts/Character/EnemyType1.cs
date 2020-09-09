using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType1 : EnemyBase
{
    public EnemyType1(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int dodgeRate) : base(characterId, pos, name, maxHp, team, atk, def, characterType, dodgeRate)
    {
        
    }

    public override void AutoAttack()
    {
        var rand = Random.Range(0, skills.Count);
        if(skills[rand] is Attack)
        {
            PickRandomTargetAttack(skills[rand]);
        }
        else if(skills[rand] is DualAttack)
        {
            UnSelectableSkill(skills[rand]);
        }
        else if(skills[rand] is SelfTaunt)
        {
            UnSelectableSkill(skills[rand]);
        }
    }

    protected void PickRandomTargetAttack(SkillBase skill)
    {
        var target = GameManager.Instance.GetRandomAttackTarget(team.GetOpposite());
        if(target != null)
        {
            skill.Cast(target.pos, team.GetOpposite());
            skill.Reset();
        }
    }

    protected void UnSelectableSkill(SkillBase skill)
    {
        skill.Cast(Vector2Int.zero, team.GetOpposite());
        skill.Reset();
    }
}
