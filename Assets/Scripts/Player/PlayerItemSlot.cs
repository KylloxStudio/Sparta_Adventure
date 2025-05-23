using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemSlot : MonoBehaviour
{
    private Player _player;
    private List<ItemData> _items = new List<ItemData>();
    [SerializeField] private ItemSlot[] _itemSlots;

    private int _index = 0;

    private void Awake()
    {
        _player = Singleton<CharacterManager>.Instance().Player;
    }

    public void TryAddItem(ItemObject item)
    {
        if (_items.Count >= _itemSlots.Length)
        {
            return;
        }

        _items.Add(item.Data);
        _index++;

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
        if (_items.Count <= 0)
        {
            return;
        }

        StartCoroutine(UseItem());

        _items.Remove(_items[_index]);
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
