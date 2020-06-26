using UniRx;

public class BuffBase
{
    public int aliveTime;
    private int initTurn;
    protected CharacterLogic target;
    protected CharacterLogic caster;
    protected CompositeDisposable disposable;

    public virtual void Init(CharacterLogic target, CharacterLogic caster)
    {
        disposable = new CompositeDisposable();
        this.target = target;
        this.caster = caster;
        this.initTurn = GameManager.Instance.turn;
        disposable.Add(GameManager.Instance.turnEndSubject.Subscribe( turn =>
        {
            if(turn - initTurn == aliveTime)
            {
                EndBuff();
            }
            else
            {
                TickOnTurnEnds();
            }
        }));
        disposable.Add(GameManager.Instance.turnBeginSubject.Subscribe(turn =>
        {
            TickOnTurnBegins();
        }));
        
    }

    protected virtual void TickOnTurnBegins()
    {

    }

    protected virtual void TickOnTurnEnds()
    {

    }

    protected virtual void EndBuff()
    {
        target.RemoveBuff(this);
        disposable.Dispose();
    }
}
