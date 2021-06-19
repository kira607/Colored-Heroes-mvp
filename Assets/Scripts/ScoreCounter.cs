using System;
using System.Collections.Generic;
using Common;
using MatchBoard;

public class ScoreCounter
{
    private Dictionary<ChipColor, int> _scoreDictionary;

    public ScoreCounter()
    {
        _scoreDictionary = new Dictionary<ChipColor, int>
        {
            {ChipColor.Orange, 0},
            {ChipColor.Red,    0},
            {ChipColor.Green,  0},
            {ChipColor.Blue,   0},
            {ChipColor.Purple, 0}
        };
    }
    
    /*
    @brief Count scores according to the list of removed chips.
    @returns Dictionary of scores per each color.
    */
    public Dictionary<ChipColor, int> CountScore(ref List<Chip> deadChips)
    {
        ResetScores();
        foreach(var chip in deadChips)
        {
            CountScores(chip);
        }
        return _scoreDictionary;
    }

    private void CountScores(Chip chip)
    {
        if (Helpers.CommonColors().Contains(chip.color))
        {
            _scoreDictionary[chip.color] += 1;//Helpers.scorePerOneChip;
        }
        else if (chip.color == ChipColor.Multicolor)
        {
            CountSuperScores(chip);
        }
    }

    private void CountSuperScores(Chip chip)
    {
        if (!Helpers.CommonColors().Contains(chip.alternateColor))
            return;
        
        int scoreToAdd;
        switch (chip.superColor)
        {
            case SuperColor.LineUpDown:
            case SuperColor.LineLeftRight:
                scoreToAdd = 1;//Helpers.scorePerRayChip;
                break;
            case SuperColor.Bomb:
                scoreToAdd = 1;//Helpers.scorePerBombChip;
                break;
            case SuperColor.Diamond:
                scoreToAdd = 1;//Helpers.scorePerDiamondChip;
                break;
            case SuperColor.None:
                scoreToAdd = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _scoreDictionary[chip.alternateColor] += scoreToAdd;
    }

    private void ResetScores()
    {
        _scoreDictionary[ChipColor.Orange] = 0;
        _scoreDictionary[ChipColor.Red   ] = 0;
        _scoreDictionary[ChipColor.Green ] = 0;
        _scoreDictionary[ChipColor.Blue  ] = 0;
        _scoreDictionary[ChipColor.Purple] = 0;
    }
}