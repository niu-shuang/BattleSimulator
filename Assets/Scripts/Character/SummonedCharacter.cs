using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SummonedCharacter : CharacterLogic
{
    /// <summary>
    /// 存活时间，N turn
    /// </summary>
    public int aliveTime { get; private set; }
    private int initTurn;
    private CompositeDisposable disposable;
    public SummonedCharacter(int characterId, Vector2Int pos, string name, int maxHp, Team team, int atk, int def, GameDefine.CharacterType characterType, int aliveTime) : base(characterId, pos, name, maxHp, team, atk, def, characterType)
    {
        disposable = new CompositeDisposable();
        this.aliveTime = aliveTime;
        initTurn = GameManager.Instance.turn;
        if(aliveTime > 0)
        {
            disposable.Add(GameManager.Instance.turnEndSubject.Subscribe(turn =>
            {
                if (turn - initTurn == aliveTime)
                {
                    Hp.Value = 0;
                }
            }));
        }
        
        disposable.Add(isDead.Subscribe(isDead=>
        {
            if (isDead) Die();
        }));
    }

    private void Die()
    {
        disposable.Dispose();
    }
}
