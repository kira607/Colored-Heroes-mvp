using System;
using UnityEngine;

namespace HeroesTile
{
    public class HeroesTile : MonoBehaviour
    {
        [Header("Prefabs")] public GameObject heroIconPrefab;
        public RectTransform heroesTile;
        
        private HeroIconLoader _heroIconLoader;
        

        public void Start()
        {
            heroesTile = gameObject.GetComponent<RectTransform>();
            _heroIconLoader = gameObject.AddComponent<HeroIconLoader>();
            _heroIconLoader.Init(heroIconPrefab, heroesTile);
        }
    }
}