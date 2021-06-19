using System;
using System.Collections.Generic;
using MatchBoard;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public static class Helpers
    {
        private static System.Random _randomGenerator;
        private static List<ChipColor> _colors;
        public const int CellSize = 84;

        public static void Init()
        {
            var seed = GetRandomSeed();
            _randomGenerator = new System.Random(seed.GetHashCode());
            _colors = new List<ChipColor>
            {
                ChipColor.Orange, ChipColor.Red, ChipColor.Green, ChipColor.Blue,
                ChipColor.Purple
            };
        }

        public static ChipColor GetRandomColor()
        {
            var val = _randomGenerator.Next(0, 5);
            return _colors[val];
        }

        public static ChipColor GetAvailableColor(ref List<ChipColor> badColors)
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

        public static Vector2 GetPositionFromPoint(Point point)
        {
            var x = point.x;
            var y = point.y;
            return new Vector2((CellSize / 2) + (CellSize * x), -(CellSize / 2) - (CellSize * y));
        }

        public static List<ChipColor> CommonColors()
        {
            return _colors;
        }

        public static Point[] BasicDirections()
        {
            return new[]
            {
                Point.Down, Point.Left, Point.Up, Point.Right
            };
        }
    
        public static ChipColor ConvertStringIntoColor(string colorString)
        {
            return colorString switch
            {
                "orange" => ChipColor.Orange,
                "red" => ChipColor.Red,
                "green" => ChipColor.Green,
                "blue" => ChipColor.Blue,
                "purple" => ChipColor.Purple,
                _ => throw new ArgumentOutOfRangeException("bad color definition in config", new Exception())
            };
        }
        
        private static string GetRandomSeed()
        {
            var seed = "";
            const string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
            for(var i = 0; i < 20; ++i)
            {
                seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
            }
            return seed;
        }


    }
}