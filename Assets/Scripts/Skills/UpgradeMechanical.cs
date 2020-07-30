using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMechanical : SkillBase
{
    public int upgradePercentage { get; private set; }
    public UpgradeMechanical(int id, string skillName, SkillType skillType, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, skillType, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        upgradePercentage = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        var target = GameManager.Instance.GetCharacter(targetPos, team);
        if (target.team == caster.team && target.characterType == GameDefine.CharacterType.Mechanical)
        {
            AllPropertyUp buff = new AllPropertyUp(upgradePercentage);
            buff.Init(target, caster, GameDefine.BuffTickType.Turn, true, -1);
            target.AddBuff(buff);
            return base.Cast(targetPos, team);
        }
        else
        {
            return false;
        }
    }
}
