using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonGangster : SkillBase
{
    public int aliveTime { get; private set; }
    public string name { get; private set; }

    public string icon { get; private set; }

    public SummonGangster(int id, string skillName, int cost, bool selectable, CharacterLogic caster, string description) : base(id, skillName, cost, selectable, caster, description)
    {
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        aliveTime = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW).GetCell(1).GetInt();
        name = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 1).GetCell(1).GetString();
        icon = sheet.GetRow(GameDefine.SKILL_CUSTOM_PROPERTY_START_ROW + 2).GetCell(1).GetString();
    }

    public override bool Cast(Vector2Int targetPos, Team team)
    {
        Gangster character = new Gangster(-1, new Vector2Int(caster.pos.x, 0), name, caster.baseHP, caster.team, caster.baseATK, caster.baseDef, GameDefine.CharacterType.Biological, caster.baseDodgeRate, aliveTime);
        GameManager.Instance.AddSummonCharacter(character, icon);
        return base.Cast(targetPos, team);
    }
}
