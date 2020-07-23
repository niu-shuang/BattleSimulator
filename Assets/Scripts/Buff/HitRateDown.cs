public class HitRateDown : BuffBase
{
    Modifier.Item item;
    protected override void OnCast()
    {
        item = new Modifier.Item() { isMulti = true, value = 500 };
        target.hitRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        target.hitRateModifier.RemoveItem(item);
        base.EndBuff();
    }
}
