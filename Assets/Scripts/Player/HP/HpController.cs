using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float healAmount = 0.05f;
    [SerializeField] private TextMeshProUGUI hpText;

    private DeadScreen deadscreen;

    private void Awake()
    {
        hpText.text = health.ToString();
        deadscreen = FindObjectOfType<DeadScreen>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        HandleHpChange();
    }

    public void Heal()
    {
        if (health < 100)
        {
            health = health + Time.deltaTime * ((100 - health) * healAmount);
            health = Mathf.Min(health, 100);
            HandleHpChange();
        }
    }

    private void HandleHpChange()
    {
        hpText.text = Math.Round(health).ToString();
        if (health <= 0f)
        {
            deadscreen.Die();
        }
    }
}
