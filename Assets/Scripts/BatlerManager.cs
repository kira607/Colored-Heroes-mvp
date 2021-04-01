using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BatlerManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject heroPrefab;
    public GameObject heroIconPrefab;

    [Header("GameFields")] 
    public GameObject batler;
    public RectTransform heroes;

    [Header("Castles")] 
    public GameObject playerCastle;
    public GameObject enemyCastle;

    private RectTransform _batlerBackground;

    private HeroLoader _heroLoader;
    private HeroIconLoader _heroIconLoader;

    private Team _playerTeam;
    private Team _enemyTeam;

    private Bot _bot;

    private float _time;

    private Dictionary<ChipColor, float> _lines;

    private float _centerCoordinate;

    public void Init(GameObject pbatler, RectTransform pheroes, GameObject pplayerCastle, GameObject penemyCastle)
    {
        batler = pbatler;
        heroes = pheroes;
        playerCastle = pplayerCastle;
        enemyCastle = penemyCastle;
        MyAwake();
    }
    private void MyAwake()
    {
        _time = 0.0f;
        
        _heroLoader = gameObject.AddComponent<HeroLoader>();
        _heroIconLoader = gameObject.AddComponent<HeroIconLoader>();
        
        _heroLoader.Init(heroPrefab, batler.GetComponent<RectTransform>());
        _heroIconLoader.Init(heroIconPrefab, heroes);

        _playerTeam = gameObject.AddComponent<Team>();
        _enemyTeam = gameObject.AddComponent<Team>();
        
        _playerTeam.Init(playerCastle);
        _enemyTeam.Init(enemyCastle);
        
        _batlerBackground = batler.transform.Find("Batler Background").gameObject.GetComponent<RectTransform>();
        _centerCoordinate = 0.0f;
    }

    public void ApplyScore(Dictionary<ChipColor, int> scores)
    {
        _playerTeam.ApplyScore(scores);
    }

    public void FixedUpdate()
    {
        _time += Time.deltaTime;
        var enemySpawnList = _bot.GetSpawnList();
        
        _playerTeam.UpdateHeroes();
        _enemyTeam.UpdateHeroes(enemySpawnList);

        var batlerState = DetectBatlerState();
        Step(batlerState);
    }
    
    private BatlerState DetectBatlerState()
    {
        bool playerTeamAlive = _playerTeam.Alive();
        bool enemyTeamAlive = _enemyTeam.Alive();
        if (!(playerTeamAlive || enemyTeamAlive))
        {
            return BatlerState.None;
        }
        
        // one of teams alive
        if (!playerTeamAlive) return _enemyTeam.HasHeroesInTheCenter(_centerCoordinate) ? BatlerState.EnemyInTheCenter1 : BatlerState.EnemyNotInTheCenter1;
        if (!enemyTeamAlive) return _playerTeam.HasHeroesInTheCenter(_centerCoordinate) ? BatlerState.PlayerInTheCenter1 : BatlerState.PlayerNotInTheCenter1;
        
        // both teams alive
        if (!_playerTeam.HasHeroesInTheCenter(_centerCoordinate)) return CompareStrength();
        return _enemyTeam.HasHeroesInTheCenter(_centerCoordinate) ? BatlerState.Battle : CompareStrength();
    }

    private BatlerState CompareStrength()
    {
        var sPlayer = _playerTeam.GetStrength();
        var sEnemy = _enemyTeam.GetStrength();
        if (Math.Abs(sEnemy - sPlayer) <= 5)
        {
            return BatlerState.EqualStrength;
        }
        return sEnemy > sPlayer ? BatlerState.EnemyWins : BatlerState.PlayerWins;
    }

    private void Step(BatlerState batlerState)
    {
        switch (batlerState)
        {
            case BatlerState.Battle:
            {
                _playerTeam.MoveToCenter(_centerCoordinate);
                _enemyTeam.MoveToCenter(_centerCoordinate);
                if (_time >= 1.0f)
                {
                    _playerTeam.Hit(_enemyTeam.GetDamage());
                    _enemyTeam.Hit(_playerTeam.GetDamage());
                    _time = 0.0f;
                }
            }
                break;
            case BatlerState.None:
            {
                // do nothing
            }
                break;
            case BatlerState.PlayerInTheCenter1:
            {
                // 155 254
                // -305 -- 305
                //check if border
                var currentBatlerPosition = _batlerBackground.anchoredPosition.x;
                if (currentBatlerPosition < 305 && currentBatlerPosition > -305)
                {
                    // move batler
                    var speed = _playerTeam.GetMaxSpeed(); // 10
                    var newPosition = Mathf.Clamp(currentBatlerPosition - speed, -305, 305);
                    _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    // TODO: move player team with diff (future)
                    _enemyTeam.MoveWithSpeed(speed);
                }
                else
                {
                    // move center
                    var speed = _playerTeam.GetMaxSpeed(); // 10
                    var newCenter = Mathf.Clamp(_centerCoordinate + speed, -204, 204);
                    _centerCoordinate = newCenter;
                    if (newCenter > 0)
                    {
                        var newPosition = Mathf.Clamp(currentBatlerPosition - speed, -305, 305);
                        _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    }

                    if (Math.Abs(newCenter - 204) < 0.1f)
                    {
                        if (_time >= 1.0f)
                        {
                            var damage = _playerTeam.GetDamage();
                            bool castleFall = _enemyTeam.HitCastle(damage);
                            if (castleFall)
                            {
                                // player win
                                WinnerHolder.PlayerWined = true;
                                SceneManager.LoadScene("End Of Game");
                            }

                            _time = 0.0f;
                        }
                    }
                    // move player team
                    _playerTeam.MoveToCenter(_centerCoordinate);    
                }
            }
                break;
            case BatlerState.EnemyInTheCenter1:
            {
                // 155 254
                // -305 -- 305
                //check if border
                var currentBatlerPosition = _batlerBackground.anchoredPosition.x;
                if (currentBatlerPosition < 305 && currentBatlerPosition > -305)
                {
                    // move batler
                    var speed = _enemyTeam.GetMaxSpeed(); // 10
                    var newPosition = Mathf.Clamp(currentBatlerPosition + speed, -305, 305);
                    _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    // TODO: move enemy team with diff (future)
                    _playerTeam.MoveWithSpeed(speed);
                }
                else
                {
                    // move center
                    var speed = _enemyTeam.GetMaxSpeed(); // 10
                    var newCenter = Mathf.Clamp(_centerCoordinate - speed, -204, 204);
                    _centerCoordinate = newCenter;
                    if (newCenter < 0)
                    {
                        var newPosition = Mathf.Clamp(currentBatlerPosition + speed, -305, 305);
                        _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    }
                    
                    if (Math.Abs(newCenter - (-204)) < 0.1f)
                    {
                        if (_time >= 1.0f)
                        {
                            var damage = _enemyTeam.GetDamage();
                            bool castleFall = _playerTeam.HitCastle(damage);
                            if (castleFall)
                            {
                                //player loose
                                WinnerHolder.PlayerWined = false;
                                SceneManager.LoadScene("End Of Game");
                            }

                            _time = 0.0f;
                        }
                    }
                    // move player team
                    _enemyTeam.MoveToCenter(_centerCoordinate);    
                }
            }
                break;
            case BatlerState.PlayerNotInTheCenter1:
            {
                _playerTeam.MoveToCenter(_centerCoordinate);
            }
                break;
            case BatlerState.EnemyNotInTheCenter1:
            {
                _enemyTeam.MoveToCenter(_centerCoordinate);
            }
                break;
            case BatlerState.PlayerWins:
            {
                if (!_playerTeam.HasHeroesInTheCenter(_centerCoordinate))
                {
                    _playerTeam.MoveToCenter(_centerCoordinate);
                    break;
                }
                // 155 254
                // -305 -- 305
                //check if border
                var currentBatlerPosition = _batlerBackground.anchoredPosition.x;
                if (currentBatlerPosition < 305 && currentBatlerPosition > -305)
                {
                    // move batler
                    var speed = _playerTeam.GetMaxSpeed(); // 10
                    var newPosition = Mathf.Clamp(currentBatlerPosition - speed, -305, 305);
                    _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    // TODO: move player team with diff (future)
                    _enemyTeam.MoveWithSpeed(speed);
                }
                else
                {
                    // move center
                    var speed = _playerTeam.GetMaxSpeed(); // 10
                    var newCenter = Mathf.Clamp(_centerCoordinate + speed, -204, 204);
                    _centerCoordinate = newCenter;
                    if (newCenter > 0)
                    {
                        var newPosition = Mathf.Clamp(currentBatlerPosition - speed, -305, 305);
                        _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    }
                    // move player team
                    _playerTeam.MoveToCenter(_centerCoordinate);    
                }
            }
                break;
            case BatlerState.EnemyWins:
            {
                if (!_enemyTeam.HasHeroesInTheCenter(_centerCoordinate))
                {
                    _enemyTeam.MoveToCenter(_centerCoordinate);
                    break;
                }
                // 155 254
                // -305 -- 305
                //check if border
                var currentBatlerPosition = _batlerBackground.anchoredPosition.x;
                if (currentBatlerPosition < 305 && currentBatlerPosition > -305)
                {
                    // move batler
                    var speed = _enemyTeam.GetMaxSpeed(); // 10
                    var newPosition = Mathf.Clamp(currentBatlerPosition + speed, -305, 305);
                    _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    // TODO: move enemy team with diff (future)
                    _playerTeam.MoveWithSpeed(speed);
                }
                else
                {
                    // move center
                    var speed = _enemyTeam.GetMaxSpeed(); // 10
                    var newCenter = Mathf.Clamp(_centerCoordinate - speed, -204, 204);
                    _centerCoordinate = newCenter;
                    if (newCenter < 0)
                    {
                        var newPosition = Mathf.Clamp(currentBatlerPosition + speed, -305, 305);
                        _batlerBackground.anchoredPosition = new Vector2(newPosition, 0);
                    }
                    // move player team
                    _enemyTeam.MoveToCenter(_centerCoordinate);    
                }
            }
                break;
            case BatlerState.EqualStrength:
            {
                _playerTeam.MoveToCenter(_centerCoordinate);
                _enemyTeam.MoveToCenter(_centerCoordinate);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(batlerState), batlerState, null);
        }
    }

    public void SetBot(Bot bot)
    {
        _bot = bot;
        _bot.SetTeam(_enemyTeam);
    }

    public void LoadHeroes(IEnumerable<string> playerHeroesMetaNames, IEnumerable<string> enemyHeroesMetaNames)
    {
        LoadPlayerHeroes(playerHeroesMetaNames);
        LoadEnemyHeroes(enemyHeroesMetaNames);
    }

    private void LoadPlayerHeroes(IEnumerable<string> metaNames)
    {
        foreach (var metaName in metaNames)
        {
            var hero = _heroLoader.Load(metaName); 
            var icon = _heroIconLoader.GenerateIcon(hero.color);
            hero.SetIcon(icon);
            _playerTeam.AddHero(hero);
        }
    }
    
    private void LoadEnemyHeroes(IEnumerable<string> metaNames)
    {
        foreach (var metaName in metaNames)
        {
            var hero = _heroLoader.Load(metaName);
            hero.SetIcon(null);
            _enemyTeam.AddHero(hero);
        }
    }
    
    
}