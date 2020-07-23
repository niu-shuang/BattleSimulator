public class Stune : BuffBase
{
    protected override void OnCast()
    {
        target.isStun = true;
    }

    protected override void EndBuff()
    {
        target.isStun = false;
        base.EndBuff();
    }
}
