using System.Collections;
using System.Collections.Generic;
using Batler;
using Common;
using MatchBoard;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public float timerOffset = 1.0f;
    private float _timer;
    [SerializeField]
    private Team _team;

    void Start()
    {
        _timer = 0.0f;
    }
    
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= timerOffset)
        {
            _timer = 0.0f;
            _team.ApplyScore(new Dictionary<ChipColor, int>{{Helpers.GetRandomColor(), 150}});
        }
    }

    public List<ChipColor> GetSpawnList()
    {
        var heroes = _team.GetHeroes();
        var spawnList = new List<ChipColor>();
        foreach (var hero in heroes)
        {
            if (hero.score >= hero.price)
            {
                spawnList.Add(hero.color);
            }
        }

        return spawnList;
    }

    public void SetTeam(Team team)
    {
        _team = team;
    }
}
