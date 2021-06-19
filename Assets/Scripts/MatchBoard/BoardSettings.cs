using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatchBoard
{
    [Serializable]
    public class BoardSettings
    {
        [Header("Board Size")] 
        public int width;
        public int height;

        [Header("Scores settings")] 
        public int scorePerOneChip = 50;
        public int scorePerRayChip = 100;
        public int scorePerBombChip = 100;
        public int scorePerDiamondChip = 200;
        
        [Header("Other")]
        public float chipSize;

        [Header("Prefabs")] 
        public GameObject cell;
        public GameObject[] chips;
        public GameObject[] superChips;
    }
}