using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemData _data;
    public ItemData Data => _data;

    public string GetInteractPrompt()
    {
        string str = $"{_data.DisplayName}\n{_data.Description}";
        return str;
    }

    public void OnInteract()
    {
        Singleton<CharacterManager>.Instance().Player.ItemSlot.TryAddItem(this);
    }
}
