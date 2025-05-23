using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    private float _curValue;
    public float CurValue => _curValue;

    [SerializeField]
    private float _maxValue;
    public float MaxValue => _maxValue;

    [SerializeField]
    private float _startValue;
    public float StartValue => _startValue;

    [SerializeField]
    private float _passiveValue;
    public float PassiveValue => _passiveValue;

    [SerializeField]
    private Slider _uiBar;

    private void Start()
    {
        _curValue = _startValue;
        _uiBar.value = GetPercentage();
    }

    private void Update()
    {
        _uiBar.value = Mathf.Lerp(_uiBar.value, GetPercentage(), Time.deltaTime * 10f);
    }

    public void Set(float value)
    {
        _curValue = Mathf.Min(value, MaxValue);
    }

    public void Add(float amount)
    {
        _curValue = Mathf.Min(_curValue + amount, MaxValue);
    }

    public void Subtract(float amount)
    {
        _curValue = Mathf.Max(_curValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return _curValue / _maxValue;
    }
}
