using NPOI.SS.UserModel;
using UnityEngine;

public class FireBall : SkillBase
{
    public int damage { get; private set; }
    public FireBall(int id, string skillName, int coolDown, bool selectable, CharacterLogic caster) : base(id, skillName, coolDown, selectable, caster)
    {
    }

    public override void Cast(Vector2Int targetPos)
    {
        Team targetTeam = caster.team == Team.Team1 ? Team.Team2 : Team.Team1;
        var target = GameManager.Instance.GetCharacter(targetPos, targetTeam);
        if(target != null)
        {
            target.Damage(damage);
            base.Cast(targetPos);
        }
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        damage = sheet.GetRow(5).GetCell(1).GetInt();
    }
}
