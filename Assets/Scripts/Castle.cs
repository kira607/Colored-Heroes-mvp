
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [SerializeField]
    private Slider healthBarSlider;
    private int _health;
    private int _currentHealth;

    public void SetMaxHealth(int health)
    {
        _health = health;
        _currentHealth = health;
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
    }

    public bool Hit(int damage)
    {
        _currentHealth -= damage;
        healthBarSlider.value = _currentHealth;
        return _currentHealth <= 0;
    }
}