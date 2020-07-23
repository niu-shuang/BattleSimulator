using UniRx;

public class BuffBase
{
    public int aliveTime { get; private set; }
    public int initTurn { get; private set; }
    public bool isPermanent { get; private set; }
    public GameDefine.BuffTickType tickType { get; private set; }
    protected CharacterLogic target;
    protected CharacterLogic caster;
    protected CompositeDisposable disposable;

    public virtual void Init(CharacterLogic target, CharacterLogic caster, GameDefine.BuffTickType tickType, bool isPermanent, int aliveTime)
    {
        disposable = new CompositeDisposable();
        this.target = target;
        this.caster = caster;
        this.initTurn = GameManager.Instance.turn;
        this.aliveTime = aliveTime;
        this.isPermanent = isPermanent;
        this.tickType = tickType;
        disposable.Add(target.isDead.Subscribe(dead =>
        {
            if (dead)
            {
                EndBuff();
            }
        }));
        disposable.Add(GameManager.Instance.turnEndSubject.Subscribe(turn =>
        {
            OnTurnEnds();
        }));
        disposable.Add(GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            if (tickType == GameDefine.BuffTickType.Turn)
                Tick();
            OnTurnBegins();
        }));
        disposable.Add(target.beforeAttackSubject.Subscribe(OnBeforeAttack));
        disposable.Add(target.afterAttackSubject.Subscribe(attackInfo =>
        {
            if (tickType == GameDefine.BuffTickType.Attack)
                Tick();
            OnAfterAttack(attackInfo);
        }));
        disposable.Add(target.beforeDamageSubject.Subscribe(OnBeforeDamage));
        disposable.Add(target.afterDamageSubject.Subscribe(damageInfo =>
        {
            if (tickType == GameDefine.BuffTickType.Damage)
                Tick();
            OnAfterDamage(damageInfo);
        }));
        OnCast();
    }

    protected virtual void OnCast()
    {

    }
    protected virtual void OnTurnBegins()
    {

    }

    protected virtual void OnTurnEnds()
    {

    }

    protected virtual void OnBeforeAttack(AttackInfo info)
    {

    }

    protected virtual void OnAfterAttack(AttackInfo info)
    {

    }

    protected virtual void OnBeforeDamage(DamageInfo info)
    {

    }

    protected virtual void OnAfterDamage(DamageInfo info)
    {

    }

    protected virtual void EndBuff()
    {
        target.RemoveBuff(this);
        disposable.Dispose();
    }

    protected void Tick()
    {
        if (!isPermanent)
        {
            aliveTime -= 1;
            if (aliveTime <= 0)
            {
                EndBuff();
            }
        }
    }
}
