public class PerfectDodgeOnce : BuffBase
{
    private Modifier.Item item;
    protected override void OnBeforeDamage(DamageInfo info)
    {
        item = new Modifier.Item() { isMulti = false, value = GameDefine.PERCENTAGE_MAX };
        target.dodgeRateModifier.AddItem(item);
    }

    protected override void EndBuff()
    {
        target.dodgeRateModifier.RemoveItem(item);
        base.EndBuff();
    }
}
