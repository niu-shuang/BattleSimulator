using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UIAnimation;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public bool hasCharacter { get; private set; }
    [SerializeField]
    public CharacterView characterView;

    public Vector2Int pos { get; private set; }
    public Team team { get; private set; }
    private IDisposable dispose;
    public CharacterLogic characterLogic { get; private set; }

    public void SetCharacter(CharacterLogic character, Sprite iconSprite, Vector2Int pos, Team team)
    {
        this.team = team;
        this.pos = pos;
        this.characterView.Init(character, iconSprite);
        characterLogic = character;
        hasCharacter = true;
        dispose = character.isDead.Subscribe(isDead => { 
            if(isDead)
            {
                hasCharacter = false;
                dispose?.Dispose();
            }
        });
    }

    public void SetEmpty(Vector2Int pos, Team team)
    {
        this.pos = pos;
        this.team = team;
        this.characterView.SetEmpty();
        hasCharacter = false;
    }

    public void OnClick()
    {
        GameManager.Instance.OnClickGrid(pos, team);
    }

    private void Reset()
    {
        characterView = GetComponent<CharacterView>();
    }
}
