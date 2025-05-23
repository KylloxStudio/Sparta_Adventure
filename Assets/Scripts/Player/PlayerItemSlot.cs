using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemSlot : MonoBehaviour
{
    private Player _player;
    private ItemData[] _items;
    [SerializeField] private ItemSlot[] _itemSlots;

    private int _index = 0;

    private void Awake()
    {
        _player = Singleton<CharacterManager>.Instance().Player;
        _items = new ItemData[_itemSlots.Length];
    }

    public void TryAddItem(ItemObject item)
    {
        if (_index >= _itemSlots.Length)
        {
            return;
        }

        _index++;
        _items[_index] = item.Data;
        _itemSlots[_index].Icon.sprite = item.Data.Icon;
        _itemSlots[_index].Icon.gameObject.SetActive(true);
        item.gameObject.SetActive(false);
    }

    public void OnUsedItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryUseItem();
        }
    }

    private void TryUseItem()
    {
        if (_index <= 0)
        {
            return;
        }

        StartCoroutine(UseItem());

        _items[_index] = null;
        _itemSlots[_index].Icon.sprite = null;
        _itemSlots[_index].Icon.gameObject.SetActive(false);

        _index--;
    }

    private IEnumerator UseItem()
    {
        ItemData item = _items[_index];
        if (item.Type != ItemType.Usable)
        {
            yield break;
        }

        for (int i = 0; i < item.Usables.Length; i++)
        {
            switch (item.Usables[i].Type)
            {
                case UseType.SpeedBoost:
                    _player.Controller.SpeedBonus += _player.Controller.MoveSpeed * item.Usables[i].Value;
                    yield return new WaitForSeconds(item.Usables[i].EffectTime);
                    _player.Controller.SpeedBonus -= _player.Controller.MoveSpeed * item.Usables[i].Value;
                    break;
                case UseType.JumpBoost:
                    _player.Controller.JumpPowerBonus += _player.Controller.DefaultJumpPower * item.Usables[i].Value;
                    yield return new WaitForSeconds(item.Usables[i].EffectTime);
                    _player.Controller.JumpPowerBonus -= _player.Controller.DefaultJumpPower * item.Usables[i].Value;
                    break;
            }
        }
    }
}
