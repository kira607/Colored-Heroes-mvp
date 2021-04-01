using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
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
        if (_timer >= 1.0f)
        {
            _timer = 0.0f;
            _team.ApplyScore(new Dictionary<ChipColor, int>{{Helpers.instance.GetRandomColor(), 150}});
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
