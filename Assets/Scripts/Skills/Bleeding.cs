using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Bleeding : SkillBase
{
    public int LifeDownRate { get; private set; }
    public int aliveTime { get; private set; }
    public Bleeding(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        LifeDownRate = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target.team != caster.team)
        {
            BleedingBuff buff = new BleedingBuff();
            buff.LifeDownRate = LifeDownRate;
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, false, 3);
            caster.AddBuff(buff);

            caster.beforeAttackSubject.Subscribe(attackInfo =>
            {
                attackInfo.finalAtk = (int)(attackInfo.finalAtk * LifeDownRate / 1000f);
            });

            // bleeding for the first time.
            buff.Excute();

            return base.Cast(targetPos, team);
        }
        else
        {
            return base.Cast(targetPos, team);
        }
    }
}
