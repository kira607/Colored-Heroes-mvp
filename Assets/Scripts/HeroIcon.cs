using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroIcon : MonoBehaviour, IPointerDownHandler
{
    public Slider curtainSlider;
    public Slider healthBarSlider;
    public GameObject healthBar;
    public Image blackout;
    public GameObject iconImage;
    
    public bool collectedMax;
    public bool sendSpawnSignal;
    public bool blocked;
    
    private readonly Color _blackoutColor = new Color(0,0,0, (200f/255f));


    private void Start()
    {
        blackout = blackout.GetComponent<Image>();
        healthBar = gameObject.transform.Find("Health Bar").gameObject;
        healthBarSlider = healthBar.GetComponent<Slider>();
    }

    public void SetMaxScore(int score)
    {
        curtainSlider.maxValue = score;
        curtainSlider.value = 0;
        collectedMax = false;
        sendSpawnSignal = false;
        blocked = false;
    }

    public void SetScore(int score)
    {
        if(blocked) return;
        if (score >= curtainSlider.maxValue)
        {
            blackout.color = Color.clear;
            curtainSlider.value = 0;
            collectedMax = true;
        }
        else
        {
            curtainSlider.value = score;
            collectedMax = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!blocked)
        {
            if (collectedMax)
            {
                sendSpawnSignal = true;
            }
        }
    }

    public void SetIconImage(Sprite icon)
    {
        iconImage = gameObject.transform.Find("IconImage").gameObject;
        iconImage.GetComponent<Image>().color = new Color(1,1,1,1);
        iconImage.GetComponent<Image>().sprite = icon;
    }

    public void ShowHealthBar(int health)
    {
        healthBar.SetActive(true);
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
    }

    public void UpdateHealthBar(int health)
    {
        healthBarSlider.value = health;
    }

    public void Block()
    {
        blocked = true;
        sendSpawnSignal = false;
    }

    public void TurnOnBlackout()
    {
        blackout.color = _blackoutColor;
    }

    public void Reset()
    {
        curtainSlider.value = 0;
        healthBarSlider.value = 0;
        healthBar.SetActive(false);
        blackout.color = _blackoutColor;
        collectedMax = false;
        blocked = false;
    }
}
