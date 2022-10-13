using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    //[SerializeField] private float _lowHealth = 50f;
    [SerializeField] private float _healthHit = 10f;
    [SerializeField] private float _health;

    [SerializeField] private Text _playerHealthText;

    // Start is called before the first frame update
    void Start()
    {
        // Full Health
        ResetHealth();

        // Display
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHealth()
    {
        // Full Health
        _health = _maxHealth;
        SetHealth(_maxHealth);
    }

    public void updateUI()
    {
        // Display Player Health
        _playerHealthText.text = $"{_health}";
    }

    private void SetHealth(float inhealth)
    {
        // Set new health, adjusting to ensure it is in health range
        _health = inhealth;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        else
        if (_health < 0)
        {
            _health = 0;
        }

        // Show health status
        updateUI();
    }

    public bool IsDead()
    {
        // if Health is 0 then dead
        if (_health > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Hit()
    {
        // Hit. Standard damage.
        Hit(_healthHit);
    }

    private void Hit(float inHealthHit)
    {
        // Hit. Update health by specified damage
        _health -= inHealthHit;
        SetHealth(_health);

        if (IsDead())
        {   // Dead. Tell player.
            Dead();
        }
    }

    void AddHealth()
    {
        // Add standard health bonus
        _health += _healthHit;
        SetHealth(_health);
    }
    
    void Dead()
    {
        // Placeholder for future code releases
        return;
    }
}
