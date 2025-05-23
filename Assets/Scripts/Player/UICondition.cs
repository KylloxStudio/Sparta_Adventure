using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    [SerializeField]
    private Condition _health;
    public Condition Health => _health;

    [SerializeField]
    private Condition _stamina;
    public Condition Stamina => _stamina;

    private void Start()
    {
        Singleton<CharacterManager>.Instance().Player.Condition.UICondition = this;
    }
}
