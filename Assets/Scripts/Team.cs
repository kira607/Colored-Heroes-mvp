
using System;
using System.Collections.Generic;
using System.Linq;
using match_board;
using UnityEngine;

public class Team : MonoBehaviour
{
    private Castle _castle;
    private Dictionary<ChipColor, Hero> _heroes;
    private List<Hero> _aliveHeroes;
    private Dictionary<ChipColor, float> _lines;

    public void Init(GameObject castle)
    {
        _heroes = new Dictionary<ChipColor, Hero>();
        _aliveHeroes = new List<Hero>();
        _lines = new Dictionary<ChipColor, float>();
        _castle = castle.GetComponent<Castle>();
        _castle.SetMaxHealth(7000);
        const float start = -68.0f;
        const float step = 27.2f;
        var colors = new List<ChipColor>
        {
            ChipColor.Orange, ChipColor.Red, ChipColor.Green, ChipColor.Blue,
            ChipColor.Purple
        };
        var i = 0;
        foreach (var color in colors)
        {
            _lines.Add(color, start + step * i);
            ++i;
        }
    }

    public void Hit(int healthToReduce)
    {
        if (_aliveHeroes.Count > 0)
        {
            _aliveHeroes[0].Hit(healthToReduce);
        }
    }

    public bool HitCastle(int damage)
    {
        return _castle.Hit(damage);
    }

    public void AddHero(Hero hero)
    {
        _heroes[hero.color] = hero;
    }
    
    /*
    public bool Update(List<ChipColor> spawnList)
    {
        // spawn heroes
        SpawnHeroes(spawnList);

        if (_aliveHeroes.Count <= 0)
        {
            return false;
        }

        // if first hero in the center of the screen
        var currentPosition = _aliveHeroes[0].rectTransform.anchoredPosition;
        var xSize = _aliveHeroes[0].rectTransform.sizeDelta.x;
        var centerPosition = - xSize / 2;
        if (_aliveHeroes[0].icon is null)
        {
            centerPosition = -centerPosition;
        }

        bool move = !(Math.Abs(currentPosition.x - centerPosition) < 0.05f);

        // kill heroes if needed and move others if needed
        var killList = new List<Hero>();
        foreach (var hero in _aliveHeroes)
        {
            if (hero.currentHealth <= 0)
            {
                hero.Kill();
                killList.Add(hero);
            }

            currentPosition = hero.rectTransform.anchoredPosition;
            xSize = hero.rectTransform.sizeDelta.x;
            centerPosition = - xSize / 2;
            if (hero.icon is null)
            {
                centerPosition = -centerPosition;
            }

            bool moveThis = !(Math.Abs(currentPosition.x - centerPosition) < 0.05f);     
            
            if (moveThis)
            {
                if (hero.icon is null)
                {
                    hero.MoveLeft();
                }
                else
                {
                    hero.MoveRight();
                }
            }
        }

        foreach (var heroToKill in killList)
        {
            _aliveHeroes.Remove(heroToKill);
        }

        return move;
    }
    */

    public void UpdateHeroes(List<ChipColor> spawnList = null)
    {
        if (spawnList is null) spawnList = GetSpawnList();
        SpawnHeroes(spawnList);
        KillHeroes();
    }
    
    public List<Hero> GetHeroes()
    {
        return _heroes.Select(heroItem => heroItem.Value).ToList();
    }

    public List<Hero> GetAliveHeroes()
    {
        return _aliveHeroes;
    }
    
    public bool HasHeroesInTheCenter(float centerCoordinate)
    {
        var heroesInTheCenter = GetHeroesInTheCenter(centerCoordinate);
        return heroesInTheCenter.Count > 0;
    }

    public void ApplyScore(Dictionary<ChipColor, int> scores)
    {
        foreach (var heroItem in _heroes)
        {
            var color = heroItem.Key;
            var hero = heroItem.Value;
            if (scores.ContainsKey(color))
            {
                hero.AddScore(scores[color]);
            }
        }
    }

    public int GetStrength()
    {
        return _aliveHeroes.Sum(hero => hero.strength);
    }

    public int GetDamage()
    {
        return _aliveHeroes.Sum(hero => hero.damage);
    }
    
    public void MoveToCenter(float centerCoordinate)
    {
        var moveList = new List<Hero>();
        var heroesInTheCenter = GetHeroesInTheCenter(centerCoordinate);
        foreach (var hero in _aliveHeroes)
        {
            if(heroesInTheCenter.Contains(hero)) continue;
            moveList.Add(hero);
        }
        foreach (var hero in moveList)
        {
            if (hero.icon is null)
            {
                hero.MoveLeft();
            }
            else
            {
                hero.MoveRight();
            }
        }
    }

    public bool Alive()
    {
        return _aliveHeroes.Count > 0;
    }

    public float GetMaxSpeed()
    {
        float maxSpeed = _aliveHeroes[0].speed;
        return _aliveHeroes.Select(hero => hero.speed).Prepend(maxSpeed).Max();
    }

    public void MoveWithSpeed(float speed)
    {
        foreach (var hero in _aliveHeroes)
        {
            if (hero.icon is null)
            {
                hero.MoveLeft(speed);
            }
            else
            {
                hero.MoveRight(speed);
            }
        }
    }

    private void KillHeroes()
    {
        var killList = new List<Hero>();
        foreach (var hero in _aliveHeroes)
        {
            if (hero.currentHealth <= 0)
            {
                hero.Kill();
                killList.Add(hero);
            }
        }

        foreach (var heroToKill in killList)
        {
            _aliveHeroes.Remove(heroToKill);
        }
    }

    private void SpawnHeroes(ICollection<ChipColor> spawnList)
    {
        foreach (var heroItem in _heroes)
        {
            var color = heroItem.Key;
            var hero = heroItem.Value;
            if (!spawnList.Contains(color)) continue;
            if (hero.alive) continue;
            hero.Spawn(_lines[hero.color]);
            _aliveHeroes.Add(hero);
        }
    }
    
    private List<Hero> GetHeroesInTheCenter(float centerCoordinate)
    {
        var heroesInTheCenter = new List<Hero>();
        foreach (var hero in _aliveHeroes)
        {
            var currentPosition = hero.rectTransform.anchoredPosition;
            var xSize = hero.rectTransform.sizeDelta.x;
            var centerPosition = centerCoordinate - xSize / 2;
            if (hero.icon is null)
            {
                centerPosition += xSize;
            }
            
            bool inCenter = Math.Abs(currentPosition.x - centerPosition) < 1.5f;
            
            if (inCenter)
            {
                heroesInTheCenter.Add(hero);
            }
        }

        return heroesInTheCenter;
    }

    private List<ChipColor> GetSpawnList()
    {
        var spawnList = new List<ChipColor>();
        foreach (var heroItem in _heroes)
        {
            var color = heroItem.Key;
            var hero = heroItem.Value;
            if(hero.icon is null) continue;
            if(!hero.icon.sendSpawnSignal) continue;
            spawnList.Add(color);
        }

        return spawnList;
    }
}