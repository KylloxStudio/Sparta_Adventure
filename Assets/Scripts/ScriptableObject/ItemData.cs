using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Usable
}

public enum UseType
{
    SpeedBoost,
    JumpBoost
}

[Serializable]
public class ItemDataUsable
{
    public UseType Type;
    public float Value;
    public float EffectTime;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string DisplayName;
    public string Description;
    public ItemType Type;
    public Sprite Icon;
    public GameObject DropPrefab;

    [Header("Stacking")]
    public bool CanStack;
    public int MaxStackAmount;

    [Header("Usable")]
    public ItemDataUsable[] Usables;
}
