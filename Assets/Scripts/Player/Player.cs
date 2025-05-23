using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public PlayerItemSlot ItemSlot { get; private set; }

    public GameObject CharacterModel { get; private set; }
    public Vector3 InitialPosition { get; private set; }

    private void Awake()
    {
        Singleton<GameManager>.Instance().Player = this;
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        ItemSlot = GetComponent<PlayerItemSlot>();
        CharacterModel = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        InitialPosition = transform.position;
    }
}
