using NPOI.SS.UserModel;
using UnityEngine;

public class FireBall : SkillBase
{
    public int damage { get; private set; }
    public FireBall(int id, string skillName, int coolDown, bool selectable, CharacterLogic caster, string description) : base(id, skillName, coolDown, selectable, caster, description)
    {
    }

    public override void Cast(Vector2Int targetPos, Team team)
    {
        Team targetTeam = caster.team == Team.Team1 ? Team.Team2 : Team.Team1;
        var target = GameManager.Instance.GetCharacter(targetPos, targetTeam);
        if(target != null)
        {
            AttackInfo info = new AttackInfo(caster, target, damage, GameDefine.DamageType.Magical);
            info.DoDamage();
            base.Cast(targetPos, team);
        }
    }

    public override void LoadCustomProperty(ISheet sheet)
    {
        damage = sheet.GetRow(6).GetCell(1).GetInt();
    }
}
