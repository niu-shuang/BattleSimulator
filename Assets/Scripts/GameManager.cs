using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J;
using System;
using UniRx;
using UnityEngine.UI;
using System.IO;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum GamePhase
    {
        Prepare,
        SelectChara,      //选择角色
        Attack,　　　　　 //选择攻击对象
        SpecSkill,             //选择施法对象
        EnemyAttack,      //敌方自动攻击
        EndTurn           //结束turn
    }

    public ReactiveProperty<GamePhase> phase;
    public int turn { get; private set; }
    public Subject<int> turnEndSubject;
    public Subject<int> turnBeginSubject;
    public List<Func<UniTask>> endTurnTasks;
    public List<Func<UniTask>> beginTurnTasks;

    private List<CharacterLogic> team1;
    private List<CharacterLogic> team2;
    [SerializeField]
    private GridHelper grids;
    [SerializeField]
    private CharacterPanel characterPanel;
    [SerializeField]
    private Text info;
    [SerializeField]
    private SkillCardManager skillCardManager;

    private CharacterLogic currentChara;
    private SkillBase currentSkill;
    public List<ReactiveProperty<int>> mana;
    public List<ReactiveProperty<int>> maxMana;

    private Dictionary<Team, List<CharacterLogic>> tauntUnit;

    public Dictionary<CharacterLogic, Sprite> characterIcons;

    private CompositeDisposable disposable;

    protected override void Awake()
    {
        base.Awake();
        turn = 0;
        turnEndSubject = new Subject<int>();
        endTurnTasks = new List<Func<UniTask>>();
        beginTurnTasks = new List<Func<UniTask>>();
        turnBeginSubject = new Subject<int>();
        phase = new ReactiveProperty<GamePhase>(GamePhase.Prepare);
        team1 = new List<CharacterLogic>();
        team2 = new List<CharacterLogic>();
        mana = new List<ReactiveProperty<int>>();
        mana.Add(new ReactiveProperty<int>(GameDefine.DEFAULT_MANA_PER_TURN));
        mana.Add(new ReactiveProperty<int>(GameDefine.DEFAULT_MANA_PER_TURN));
        maxMana = new List<ReactiveProperty<int>>();
        maxMana.Add(new ReactiveProperty<int>(GameDefine.DEFAULT_MANA_PER_TURN));
        maxMana.Add(new ReactiveProperty<int>(GameDefine.DEFAULT_MANA_PER_TURN));
        characterIcons = new Dictionary<CharacterLogic, Sprite>();
        tauntUnit = new Dictionary<Team, List<CharacterLogic>>();
        tauntUnit[Team.Team1] = new List<CharacterLogic>();
        tauntUnit[Team.Team2] = new List<CharacterLogic>();
        skillCardManager.Init();
        characterPanel.Init();
        disposable = new CompositeDisposable();
    }

    public void OnClickImportCharacter()
    {
        CharacterImporter.OpenExcel();
    }

    public void OnImportCharacterSuc(Dictionary<Vector2Int, CharacterInfo> team1Info, Dictionary<Vector2Int, CharacterInfo> team2Info)
    {
        PopupManager.Instance.Init();
        for(int i = 0; i < 4; i++)
        {
            SkillsImporter.OpenExcel(Path.Combine(CharacterImporter.path, $"Skills{i}.xls"));
        }
        
        Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> team1 = new Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>>();
        Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> team2 = new Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>>();
        foreach (var item in team1Info)
        {
            GameDefine.CharacterType type = (GameDefine.CharacterType)Enum.Parse(typeof(GameDefine.CharacterType), item.Value.characterType);
            Type scriptType = Type.GetType(item.Value.script);
            object[] args = new object[] { item.Value.characterId, item.Key, item.Value.characterName, item.Value.hp, Team.Team1, item.Value.atk, item.Value.def, type, item.Value.dodgeRate };
            CharacterLogic script = Activator.CreateInstance(scriptType, args) as CharacterLogic;
            foreach (var skillId in item.Value.skills)
            {
                if (skillId > 0)
                {
                    var skill = SkillsImporter.LoadSkill(skillId, script);
                    skillCardManager.AddSkill(skill, Team.Team1);
                }
            }
            Sprite sprite = Resources.Load<Sprite>($"Icons/{ item.Value.icon }");
            characterIcons[script] = sprite;
            this.team1.Add(script);
            team1[item.Key] = new KeyValuePair<CharacterLogic, Sprite>(script, sprite);
        }
        foreach (var item in team2Info)
        {
            GameDefine.CharacterType type = (GameDefine.CharacterType)Enum.Parse(typeof(GameDefine.CharacterType), item.Value.characterType);
            Type scriptType = Type.GetType(item.Value.script);
            object[] args = new object[] { item.Value.characterId, item.Key, item.Value.characterName, item.Value.hp, Team.Team2, item.Value.atk, item.Value.def, type, item.Value.dodgeRate };
            CharacterLogic script = Activator.CreateInstance(scriptType, args) as CharacterLogic;
            //CharacterLogic logic = new CharacterLogic(item.Value.characterId, item.Key, item.Value.characterName, item.Value.hp, Team.Team2, item.Value.atk, item.Value.def, type, item.Value.dodgeRate);
            foreach (var skillId in item.Value.skills)
            {
                if (skillId > 0)
                {
                    var skill = SkillsImporter.LoadSkill(skillId, script);
                    skillCardManager.AddSkill(skill, Team.Team2);
                }
            }
            Sprite sprite = Resources.Load<Sprite>($"Icons/{ item.Value.icon }");
            characterIcons[script] = sprite;
            this.team2.Add(script);
            team2[item.Key] = new KeyValuePair<CharacterLogic, Sprite>(script, sprite);
        }
        SkillsImporter.Close();
        grids.Init(team1, team2);
        disposable.Add(phase.Subscribe(OnPhaseChanged));
        NextTurnProcess();
        
        
        Observable.Timer(TimeSpan.FromSeconds(2f))
            .Subscribe(_ =>
            {
                OnClickGrid(new Vector2Int(0, 1), Team.Team1);
            });
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        Debug.Log($"phase : {phase}");
        switch (phase)
        {
            case GamePhase.Prepare:
                break;
            case GamePhase.SelectChara:
                info.text = "Select a Chara";
                break;
            case GamePhase.Attack:
                info.text = "Select a target to attack";
                break;
            case GamePhase.SpecSkill:
                info.text = "Select a target to spell";
                break;
            case GamePhase.EnemyAttack:
                info.text = "Enemy Attack";
                EnemyAttack();
                break;
            case GamePhase.EndTurn:
                info.text = "next turn processing";
                NextTurnProcess();
                break;
            default:
                break;
        }
    }

    private async void EnemyAttack()
    {
        var deck = skillCardManager.currentDeckTeam2;
        while(deck.Count > 0)
        {
            deck[0].Cast(Vector2Int.zero, Team.Team1);
            deck[0].ClearView();
            skillCardManager.OnUseSkill(deck[0]);
            await UniTask.Delay(500);
        }
        phase.Value = GamePhase.EndTurn;
    }

    public void AddSummonCharacter(CharacterLogic characterLogic, string icon)
    {
        var existChara = GetCharacter(characterLogic.pos, characterLogic.team);
        if (existChara != null)
        {
            grids.RemoveCharacter(characterLogic.pos, characterLogic.team);
        }
        if (characterLogic.team == Team.Team1)
            team1.Add(characterLogic);
        else
            team2.Add(characterLogic);
        Sprite sprite = Resources.Load<Sprite>($"Icons/{ icon }");
        grids.AddCharacter(characterLogic, sprite, characterLogic.pos);
    }

    public void RegisterTauntUnit(CharacterLogic unit)
    {
        tauntUnit[unit.team].Add(unit);
    }

    public void UnRegisterTauntUnit(CharacterLogic unit)
    {
        tauntUnit[unit.team].Remove(unit);
    }

    public CharacterLogic GetAttackTarget(Team team, int col)
    {
        if (tauntUnit[team].Count > 0)
        {
            return tauntUnit[team].FirstOrDefault();
        }
        return grids.GetAttackTarget(team, col);
    }

    public void OnClickGrid(Vector2Int pos, Team team)
    {
        switch (phase.Value)
        {
            case GamePhase.Prepare:
                break;
            case GamePhase.SelectChara:
                {
                    var chara = grids.GetCharacter(team, pos);
                    if (chara != null)
                    {
                        characterPanel.SetChara(chara);
                        currentChara = chara;
                    }
                }
                break;
            case GamePhase.Attack:
                {
                    if (team == currentChara.team) return;
                    var target = grids.GetAttackTarget(team, pos.x);
                    currentChara.Attack(target);
                    phase.Value = GamePhase.SelectChara;
                }
                break;
            case GamePhase.SpecSkill:
                {
                    var isSuc = currentSkill.Cast(pos, team);
                    if (isSuc)
                    {
                        skillCardManager.OnUseSkill(currentSkill);
                    }
                    phase.Value = GamePhase.SelectChara;
                }
                break;
            case GamePhase.EndTurn:
                break;
            default:
                break;
        }
    }

    public CharacterLogic GetCharacter(Vector2Int pos, Team team)
    {
        return grids.GetCharacter(team, pos);
    }

    public List<CharacterLogic> GetCharactersInOneCol(int col, Team team)
    {
        List<CharacterLogic> targets = new List<CharacterLogic>();
        for (int i = 0; i < 2; i++)
        {
            var chara = GetCharacter(new Vector2Int(col, i), team);
            if (chara != null)
            {
                targets.Add(chara);
            }
        }
        return targets;
    }

    public List<CharacterLogic> GetCharacters(Team team)
    {
        List<CharacterLogic> value = new List<CharacterLogic>();
        if (team == Team.Team1)
            value = team1.Where(i => !i.isDead.Value).ToList();
        else
            value = team2.Where(i => !i.isDead.Value).ToList();
        return value;
    }
    public void OnClickCancel()
    {
        switch (phase.Value)
        {
            case GamePhase.SpecSkill:
                phase.Value = GamePhase.SelectChara;
                break;
            default:
                break;
        }
    }

    public void OnClickNextTurn()
    {
        phase.Value = GamePhase.EnemyAttack;
    }

    private async void NextTurnProcess()
    {
        endTurnTasks.Clear();
        beginTurnTasks.Clear();
        turnEndSubject.OnNext(turn);
        for (int i = 0; i < endTurnTasks.Count; i++)
        {
            await endTurnTasks[i].Invoke();
        }
        turn++;
        turnBeginSubject.OnNext(turn);
        for (int i = 0; i < beginTurnTasks.Count; i++)
        {
            await beginTurnTasks[i].Invoke();
        }
        phase.Value = GamePhase.SelectChara;
        info.text = "Select a Chara";
        for(int i = 0; i < 2; i++)
        {
            mana[i].Value = maxMana[i].Value;
        }
    }

    public void OnClickAttack()
    {
        phase.Value = GamePhase.Attack;
    }

    public void OnClickSkill(SkillBase skill)
    {
        currentSkill = skill;
        if (currentSkill.selectable)
        {
            phase.Value = GamePhase.SpecSkill;
        }
        else
        {
            currentSkill.Cast(Vector2Int.zero, skill.caster.team);
            phase.Value = GamePhase.SelectChara;
            skillCardManager.OnUseSkill(skill);
        }
    }
    public void EndGame()
    {
        SceneManager.LoadScene("EmptyScene");
    }

    protected override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        Debug.Log("End Game");
        foreach (var item in team1)
        {
            item.Hp.Value = 0;
        }
        foreach (var item in team2)
        {
            item.Hp.Value = 0;
        }
        
        endTurnTasks.Clear();
        beginTurnTasks.Clear();
        turnEndSubject.OnCompleted();
        turnBeginSubject.OnCompleted();
        characterPanel.Dispose();
        disposable.Dispose();
        Debug.Log("GameManager Disposed");
    }
}
