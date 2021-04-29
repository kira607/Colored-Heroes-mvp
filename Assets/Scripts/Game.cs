using System;
using System.Collections.Generic;
using MatchBoard;
using UnityEngine;

public class Game : MonoBehaviour
{
    private readonly string[] _playerHeroesMetaNames =
    {
        "shield_bearer", 
        "spearman", 
        "royal_guard", 
        "heavy_knight", 
        "inquisitor"
    };

    private readonly string[] _enemyHeroesMetaNames = 
    {
        "armored_gnome",
        "clan_leader",
        "deep_digger",
        "shield_master",
        "hammer_thrower"
    };

    private void Start()
    {
        /*
        _resolutions = Screen.resolutions;
        foreach (var res in _resolutions)
        {
            var mes = res.width + ":" + res.height;
            //Debug.Log(mes);
        }
        */
    }

    private void Update()
    {
        
    }
}