using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDrone : SkillBase
{
    public string droneName { get; private set; }
    public string droneIcon { get; private set; }
    public int droneHp { get; private set; }
    public int droneAtk { get; private set; }
    public int droneDef { get; private set; }

    public int aliveTime { get; private set; }

    public SummonDrone(int id, string skillName,int coolDown, bool selectable, CharacterLogic caster) : base(id, skillName, coolDown, selectable, caster)
    {

    }

    public override void Cast(Vector2Int targetPos)
    {
        base.Cast(targetPos);
        CharacterInfo info = new CharacterInfo()
        {
            characterId = -1,
            characterName = droneName,
            icon = droneIcon,
            hp = droneHp,
            atk = droneAtk,
            def = droneDef
        };
        GameManager.Instance.AddSummonCharacter(caster.team, caster.pos + new Vector2Int(0, -1), info, aliveTime);
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        this.droneName = sheet.GetRow(5).GetCell(1).GetString();
        this.droneIcon = sheet.GetRow(6).GetCell(1).GetString();
        this.droneHp = sheet.GetRow(7).GetCell(1).GetInt();
        this.droneAtk = sheet.GetRow(8).GetCell(1).GetInt();
        this.droneDef = sheet.GetRow(9).GetCell(1).GetInt();
        this.aliveTime = sheet.GetRow(10).GetCell(1).GetInt();
    }
}
