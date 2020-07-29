public class HitRateDown : BuffBase
{
    Modifier.Item item;
    protected override void OnCast()
    {
        item = new Modifier.Item() { isMulti = true, value = 500 };
        GameLogger.AddLog($"{target.name} hit rate down -50%");
        target.hitRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        GameLogger.AddLog($"{target.name} hit rate modifier removed");
        target.hitRateModifier.RemoveItem(item);
        base.EndBuff();
    }
}
