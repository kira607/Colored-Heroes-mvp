using System;
using System.Collections.Generic;
using match_board;
using UnityEngine;
using UnityEngine.UI;

public class HeroIconLoader : MonoBehaviour
{
    private GameObject _heroIconPrefab;
    private RectTransform _parentRectTransform;
    
    public void Init(GameObject heroIconPrefab, RectTransform parentRectTransform)
    {
        _heroIconPrefab = heroIconPrefab;
        _parentRectTransform = parentRectTransform;
    }

    public Dictionary<ChipColor, HeroIcon> GenerateIcons()
    {
        var heroesIcons = new Dictionary<ChipColor, HeroIcon>();
        foreach (var color in Helpers.instance.CommonColors())
        {
            heroesIcons[color] = GenerateIcon(color);
        }

        return heroesIcons;

    }

    public HeroIcon GenerateIcon(ChipColor color)
    {
        int i = (int)color - 1;
        var iconWidth = _heroIconPrefab.GetComponent<RectTransform>().sizeDelta.x;
        var newIcon = Instantiate(_heroIconPrefab, _parentRectTransform);
        var transformStartPoint = -243; //TODO: replace this later with value taken from scene 
        newIcon.transform.name = "Hero Icon " + color;
        var newPos = new Vector2(transformStartPoint + i * iconWidth,0);
        newIcon.GetComponent<RectTransform>().anchoredPosition = newPos;
        newIcon.gameObject.transform.Find("CurtainFill").GetComponent<Image>().color = ChipColorToColor(color);
        var icon = newIcon.GetComponent<HeroIcon>();
        return icon;
    }

    private Color ChipColorToColor(ChipColor color)
    {
        var newColor = Color.clear;
        switch (color)
        {
            case ChipColor.Blank:
                break;
            case ChipColor.Orange:
                newColor = new Color(255/255.0f, 144/255.0f, 8/255.0f, 136/255.0f);
                break;
            case ChipColor.Blue:
                newColor = new Color(8/255.0f, 64/255.0f, 255/255.0f, 136/255.0f);
                break;
            case ChipColor.Red:
                newColor = new Color(255/255.0f, 8/255.0f, 18/255.0f, 136/255.0f);
                break;
            case ChipColor.Green:
                newColor = new Color(8/255.0f, 255/255.0f, 8/255.0f, 136/255.0f);
                break;
            case ChipColor.Purple:
                newColor = new Color(255/255.0f, 8/255.0f, 243/255.0f, 136/255.0f);
                break;
            case ChipColor.Multicolor:
                break;
            case ChipColor.Hole:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
        return newColor;
    }
}
