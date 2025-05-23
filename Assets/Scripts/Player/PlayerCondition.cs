using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    private Player _player;
    public UICondition UICondition { get; set; }

    private Condition Health => UICondition.Health;
    private Condition Stamina => UICondition.Stamina;

    public event Action OnTakeDamage;

    public bool IsDamaged { get; private set; }

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        OnTakeDamage += OnDamaged;
    }

    private void Update()
    {
        if (!IsDamaged)
        {
            Health.Add(Health.PassiveValue * Time.deltaTime);
        }

        if (!_player.Controller.IsDashing)
        {
            Stamina.Add(Stamina.PassiveValue * Time.deltaTime);
        }
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void TakePhysicalDamage(int amount)
    {
        Health.Subtract(amount);
        OnTakeDamage?.Invoke();
    }

    public void OnDamaged()
    {
        IsDamaged = true;

        CancelInvoke(nameof(OffDamaged));
        Invoke(nameof(OffDamaged), 0.8f);
    }

    private void OffDamaged()
    {
        IsDamaged = false;
    }
}
