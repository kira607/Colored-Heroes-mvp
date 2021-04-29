using System;
using match_board;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    // config fields
    public string metaName;
    public string heroName;
    public ChipColor color;
    public double defence;
    public int price;
    public int health;
    public int damage;
    public float speed;
    public int attackSpeed;
    public int attackRange;
    
    // calculated fields
    public int strength;
    public int damagePerSecond;

    // Init fields
    public Image heroModel;
    public HeroIcon icon;
    public RectTransform rectTransform;
    
    // other
    public bool alive;
    public int score;
    public int currentHealth;

    public void ApplyInfo(HeroInfo info)
    {
        metaName = info.metaName;
        heroName = info.name;
        color = Helpers.instance.ConvertStringIntoColor(info.color);
        defence = info.defence;
        price = info.price;
        health = info.health;
        damage = info.damage;
        speed = info.speed;
        speed /= 40;
        attackSpeed = info.attackSpeed;
        attackRange = info.attackRange;

        strength = health * damage / price;
        damagePerSecond = damage * attackSpeed;

        LoadModel();

        transform.name = heroName;
        rectTransform = GetComponent<RectTransform>();
        alive = false;
    }

    private void LoadModel()
    {
        var path = "Heroes/" + metaName + "/" + metaName + ".model";
        heroModel = gameObject.AddComponent<Image>();
        heroModel.sprite = Resources.Load<Sprite>(path);
        // TODO: make this not hardcoded in the future
        // heroModel.rectTransform.sizeDelta = new Vector2(50,100);
    }

    public void SetIcon(HeroIcon heroIcon)
    {
        if (heroIcon is null)
        {
            icon = null;
            return;
        }
        icon = heroIcon;
        var path = "Heroes/" + metaName + "/" + metaName + ".icon";
        var iconImage = Resources.Load<Sprite>(path);
        icon.SetMaxScore(price);
        icon.SetIconImage(iconImage);
        icon.TurnOnBlackout();
    }

    public void AddScore(int scoreToAdd)
    {
        if(alive) return;
        score = score + scoreToAdd >= price ? price : score + scoreToAdd;
        if (icon is null) return;
        icon.SetScore(score);
    }

    public void Hit(int healthToReduce)
    {
        currentHealth -= healthToReduce;
        if(icon is null) return;
        icon.UpdateHealthBar(currentHealth);
    }

    public void Spawn(float yCoord)
    {
        if (icon is null)
        {
            rectTransform.anchoredPosition = new Vector2(254, yCoord);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(-254, yCoord);
        }

        alive = true;
        gameObject.SetActive(true);
        currentHealth = health;
        score = 0;
        if(icon is null) return;
        icon.Block();
        icon.ShowHealthBar(currentHealth);
    }

    public void Kill()
    {
        alive = false;
        gameObject.SetActive(false);
        currentHealth = 0;
        score = 0;
        if(icon is null) return;
        icon.Reset();
    }

    public void MoveRight(float fspeed = 0)
    {
        if (fspeed == 0)
        {
            fspeed = speed;
        }
        var currentPosition = rectTransform.anchoredPosition;
        var newPosition = new Vector2(currentPosition.x + fspeed, currentPosition.y);
        rectTransform.anchoredPosition = newPosition;
    }
    
    public void MoveLeft(float fspeed = 0)
    {
        if (fspeed == 0)
        {
            fspeed = speed;
        }
        var currentPosition = rectTransform.anchoredPosition;
        var newPosition = new Vector2(currentPosition.x - fspeed, currentPosition.y);
        rectTransform.anchoredPosition = newPosition;
    }
}

public class HeroInfo
{
    public string metaName;
    public string name;
    public string color;
    public double defence;
    public int price;
    public int health;
    public int damage;
    public int speed;
    public int attackSpeed;
    public int attackRange;
}