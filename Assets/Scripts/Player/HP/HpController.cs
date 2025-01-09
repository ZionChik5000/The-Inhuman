using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private TextMeshProUGUI hpText;

    private DeadScreen deadscreen;

    private void Awake()
    {
        hpText.text = health.ToString();
        deadscreen = FindObjectOfType<DeadScreen>();
    }


    private void Update()
    {
        TakeDamage(0.05f);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        hpText.text = Math.Round(health).ToString();
        if (health <= 0f)
        {
            deadscreen.Die();
        }
    }
}
