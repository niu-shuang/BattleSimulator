using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossType1 : EnemyType1
{
    public BossType1(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int dodgeRate) : base(characterId, pos, name, maxHp, team, atk, def, characterType, dodgeRate)
    {
    }

    public override void AutoAttack()
    {
        if (isStun) return;
        var rand = Random.Range(0, skills.Count);
        if (skills[rand] is Attack)
        {
            PickRandomTargetAttack(skills[rand]);
        }
        else if (skills[rand] is DualAttack)
        {
            UnSelectableSkill(skills[rand]);
        }
        else if (skills[rand] is SelfTaunt)
        {
            UnSelectableSkill(skills[rand]);
        }
        else if(skills[rand] is AttackOneCol)
        {
            PickRandomTargetAttack(skills[rand]);
        }
        else if(skills[rand] is AttackAll)
        {
            UnSelectableSkill(skills[rand]);
        }
        else if (skills[rand] is Accumulation)
        {
            UnSelectableSkill(skills[rand]);
        }
        else if (skills[rand] is Courage)
        {
            PickRandomTeammateSkill(skills[rand]);
        }
        else if (skills[rand] is SelfDefence)
        {
            UnSelectableSkill(skills[rand]);
        }
    }

    protected void PickRandomTeammateSkill(SkillBase skill)
    {
        var targets = GameManager.Instance.GetCharacters(team);
        if (targets.Count <= 0) return;
        var rand = Random.Range(0, targets.Count);
        var target = targets[rand];
        skill.Cast(target.pos, team);
        skill.Reset();
    }

    
}
