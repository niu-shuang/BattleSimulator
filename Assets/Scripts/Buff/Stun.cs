public class Stune : BuffBase
{
    protected override void OnCast()
    {
        GameLogger.AddLog($"{target.name} is Stuned");
        target.isStun = true;
    }

    protected override void EndBuff()
    {
        target.isStun = false;
        GameLogger.AddLog($"{target.name} stun end");
        base.EndBuff();
    }
}
