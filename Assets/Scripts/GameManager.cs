using System.Collections.Generic;
using UnityEngine;
using J;
using System;
using UniRx;
using UnityEngine.UI;
using System.IO;
using Cysharp.Threading.Tasks;
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
        AutoAttack,      //敌方自动攻击
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

    public ReactiveProperty<int> team1AliveCount;
    public ReactiveProperty<int> team2AliveCount;

    private Dictionary<int, List<CharacterLogic>> enemyList;
    private Dictionary<int, List<SkillBase>> enemySkills;

    private Dictionary<Team, List<CharacterLogic>> tauntUnit;

    public Dictionary<CharacterLogic, Sprite> characterIcons;
    public int currentWave { get; private set; }

    private CompositeDisposable disposable;

    private void Start()
    {
        turn = 0;
        turnEndSubject = new Subject<int>();
        endTurnTasks = new List<Func<UniTask>>();
        beginTurnTasks = new List<Func<UniTask>>();
        turnBeginSubject = new Subject<int>();
        phase = new ReactiveProperty<GamePhase>(GamePhase.Prepare);
        team1 = new List<CharacterLogic>();
        team2 = new List<CharacterLogic>();
        mana = new List<ReactiveProperty<int>>();
        mana.Add(new ReactiveProperty<int>(0));
        mana.Add(new ReactiveProperty<int>(0));
        maxMana = new List<ReactiveProperty<int>>();
        maxMana.Add(new ReactiveProperty<int>(GameDefine.MAX_MANA_PER_TURN(1)));
        maxMana.Add(new ReactiveProperty<int>(GameDefine.MAX_MANA_PER_TURN(1)));
        characterIcons = new Dictionary<CharacterLogic, Sprite>();
        tauntUnit = new Dictionary<Team, List<CharacterLogic>>();
        tauntUnit[Team.Team1] = new List<CharacterLogic>();
        tauntUnit[Team.Team2] = new List<CharacterLogic>();
        skillCardManager.Init();
        characterPanel.Init();
        team1AliveCount = new ReactiveProperty<int>(0);
        team2AliveCount = new ReactiveProperty<int>(0);
        enemyList = new Dictionary<int, List<CharacterLogic>>();
        enemySkills = new Dictionary<int, List<SkillBase>>();
        disposable = new CompositeDisposable();
        currentWave = 1;
        CharacterImporter.LoadExcel();
        
    }

    /// <summary>
    /// 导入角色成功
    /// </summary>
    /// <param name="team1Info"></param>
    /// <param name="team2Info"></param>
    public void OnImportCharacterSuc(Dictionary<Vector2Int, CharacterInfo> team1Info, Dictionary<int,Dictionary<Vector2Int, CharacterInfo>> team2Info)
    {
        PopupManager.Instance.Init();
        SkillsImporter.OpenExcel(Path.Combine(CharacterImporter.path, $"Skills.xls"));

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
            team1AliveCount.Value++;
            disposable.Add(script.isDead.Subscribe(dead =>
            {
                if (dead)
                    team1AliveCount.Value--;
            }));
        }

        foreach (var enemys in team2Info)
        {
            int wave = enemys.Key;
            enemySkills[wave] = new List<SkillBase>();
            enemyList[wave] = new List<CharacterLogic>();
            var enemyInfos = enemys.Value;
            foreach (var item in enemyInfos)
            {
                GameDefine.CharacterType type = (GameDefine.CharacterType)Enum.Parse(typeof(GameDefine.CharacterType), item.Value.characterType);
                Type scriptType = Type.GetType(item.Value.script);
                object[] args = new object[] { item.Value.characterId, item.Key, item.Value.characterName, item.Value.hp, Team.Team2, item.Value.atk, item.Value.def, type, item.Value.dodgeRate };
                CharacterLogic script = Activator.CreateInstance(scriptType, args) as CharacterLogic;
                enemyList[wave].Add(script);
                foreach (var skillId in item.Value.skills)
                {
                    
                    if (skillId > 0)
                    {
                        var skill = SkillsImporter.LoadSkill(skillId, script);
                        script.AddSkill(skill);
                        //enemySkills[wave].Add(skill);
                    }
                }
                Sprite sprite = Resources.Load<Sprite>($"Icons/{ item.Value.icon }");
                characterIcons[script] = sprite;
            }
        }
        disposable.Add(team1AliveCount.Subscribe( count =>
        {
            Debug.Log("team 1 alive " + count);
            if(count == 0)
            {
                BattleResult.battleResult = "Team 2 Win";
                EndGame();
            }
        }));
        disposable.Add(team2AliveCount.Skip(1).Subscribe(count =>
        {
            Debug.Log("team 2 alive " + count);
                       
            if(count == 0)
            {
                if (enemyList.Count > currentWave)
                {
                    currentWave++;
                    Observable.Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(.5f))
                        .Subscribe(_=>
                        {
                            PrepareForWave(currentWave);
                            phase.Value = GamePhase.EndTurn;
                        });                   
                }
                else
                {
                    BattleResult.battleResult = "Team 1 Win";
                    EndGame();
                }   
            }
        }));
        SkillsImporter.Close();
        grids.Init(team1);
        PrepareForWave(currentWave);
        disposable.Add(phase.Subscribe(OnPhaseChanged));
        NextTurnProcess();


        Observable.Timer(TimeSpan.FromSeconds(2f))
            .Subscribe(_ =>
            {
                OnClickGrid(new Vector2Int(0, 1), Team.Team1);
            });
    }

    /// <summary>
    /// 添加一波次的敌人
    /// </summary>
    /// <param name="wave"></param>
    public void PrepareForWave(int wave)
    {
        Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>> enemyTeam = new Dictionary<Vector2Int, KeyValuePair<CharacterLogic, Sprite>>();
        foreach (var item in enemyList[wave])
        {
            team2.Add(item);
            enemyTeam[item.pos] = new KeyValuePair<CharacterLogic, Sprite>(item, characterIcons[item]);
            team2AliveCount.Value++;
            disposable.Add(item.isDead.Subscribe(dead =>
            {
                if (dead)
                    team2AliveCount.Value--;
            }));
        }
        grids.AddEnemyWave(enemyTeam);
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
            case GamePhase.AutoAttack:
                info.text = "Auto Attack";
                AutoAttack();
                break;
            case GamePhase.EndTurn:
                info.text = "next turn processing";
                NextTurnProcess();
                break;
            default:
                break;
        }
    }

    private async void AutoAttack()
    {
        foreach (var item in team1)
        {
            if (item.isDead.Value != true)
            {
                item.AutoAttack();
                await UniTask.Delay(500);
            }
        }
        foreach (var item in team2)
        {
            if(item.isDead.Value != true)
            {
                var enemy = item as EnemyBase;
                enemy.AutoAttack();
                await UniTask.Delay(500);
            }
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

    public CharacterLogic GetRandomAttackTarget(Team team)
    {
        var targets = GetCharacters(team);
        if(targets.Count > 0)
        {
            List<int> colHasCharas = new List<int>();
            foreach (var item in targets)
            {
                if (!colHasCharas.Contains(item.pos.x))

                {
                    colHasCharas.Add(item.pos.x);
                }
            }
            var rand = UnityEngine.Random.Range(0, colHasCharas.Count);
            return GetAttackTarget(team, colHasCharas[rand]);
        }
        return null;
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
        phase.Value = GamePhase.AutoAttack;
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
        for (int i = 0; i < 2; i++)
        {
            maxMana[i].Value = GameDefine.MAX_MANA_PER_TURN(turn);
            int nextMana = mana[i].Value + GameDefine.RECOVER_MANA_PER_TURN(turn);
            if (nextMana > maxMana[i].Value) nextMana = maxMana[i].Value;
            mana[i].Value = nextMana;
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
        SceneManager.LoadScene("BattleResult");
    }

    protected override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        Debug.Log("End Game");

        endTurnTasks.Clear();
        beginTurnTasks.Clear();
        turnEndSubject.OnCompleted();
        turnBeginSubject.OnCompleted();
        characterPanel.Dispose();
        disposable.Dispose();
        Debug.Log("GameManager Disposed");
    }
}
