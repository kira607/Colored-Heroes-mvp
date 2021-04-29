using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatchBoard
{
    public class Helpers : MonoBehaviour
    {
        public static Helpers instance;
        private System.Random _randomGenerator;
        private List<ChipColor> _colors;
        public const int CellSize = 84;
    
        [Header("Scores settings")]
        public int scorePerOneChip = 50;
        public int scorePerRayChip = 100;
        public int scorePerBombChip = 100;
        public int scorePerDiamondChip = 200;

        private void Awake()
        {
            instance = this;
            var seed = GetRandomSeed();
            _randomGenerator = new System.Random(seed.GetHashCode());
            _colors = new List<ChipColor>
            {
                ChipColor.Orange, ChipColor.Red, ChipColor.Green, ChipColor.Blue,
                ChipColor.Purple
            };
        }

        private string GetRandomSeed()
        {
            var seed = "";
            const string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
            for(var i = 0; i < 20; ++i)
            {
                seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
            }
            return seed;
        }

        public ChipColor GetRandomColor()
        {
            var val = _randomGenerator.Next(0, 5);
            return _colors[val];
        }

        public ChipColor GetAvailableColor(ref List<ChipColor> badColors)
        {
            var availableColors = new List<ChipColor>
            {
                ChipColor.Blue, ChipColor.Green, ChipColor.Purple, ChipColor.Red,
                ChipColor.Orange
            };

            foreach (var badColor in badColors)
            {
                if (availableColors.Contains(badColor))
                {
                    availableColors.Remove(badColor);
                }
            }

            if (availableColors.Count > 0)
            {
                return availableColors[_randomGenerator.Next(0, availableColors.Count)];
            }

            return ChipColor.Blank;
        }

        public Vector2 GetPositionFromPoint(Point point)
        {
            var x = point.x;
            var y = point.y;
            return new Vector2((CellSize / 2) + (CellSize * x), -(CellSize / 2) - (CellSize * y));
        }

        public List<ChipColor> CommonColors()
        {
            return _colors;
        }

        public Point[] BasicDirections()
        {
            return new[]
            {
                Point.Down, Point.Left, Point.Up, Point.Right
            };
        }
    
        public ChipColor ConvertStringIntoColor(string colorString)
        {
            switch (colorString)
            {
                case "orange": return ChipColor.Orange;
                case "red":    return ChipColor.Red   ;
                case "green":  return ChipColor.Green ;
                case "blue":   return ChipColor.Blue  ;
                case "purple": return ChipColor.Purple;
            }

            throw new ArgumentOutOfRangeException("bad color definition in config", new Exception());
        }


    }
}