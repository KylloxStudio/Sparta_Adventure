using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemSlot : MonoBehaviour
{
    private Player _player;
    private Queue<ItemData> _items = new Queue<ItemData>();
    [SerializeField] private ItemSlot[] _itemSlots;

    private int _index = 0;

    private void Awake()
    {
        _player = Singleton<GameManager>.Instance().Player;
    }

    public void Clear()
    {
        _items.Clear();
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Icon.sprite = null;
            _itemSlots[i].Icon.gameObject.SetActive(false);
        }

        _index = 0;
    }

    public void AddItem(ItemObject item)
    {
        if (_items.Count >= _itemSlots.Length)
        {
            return;
        }


        _items.Enqueue(item.Data);
        _itemSlots[_index].Icon.sprite = item.Data.Icon;
        _itemSlots[_index].Icon.gameObject.SetActive(true);

        _index++;

        item.gameObject.SetActive(false);
    }

    public ItemData RemoveItem()
    {
        if (_items.Count <= 0)
        {
            return null;
        }

        for (int i = 1; i < _index; i++)
        {
            _itemSlots[i - 1].Icon.sprite = _itemSlots[i].Icon.sprite;
            _itemSlots[i - 1].Icon.gameObject.SetActive(true);
        }

        _itemSlots[_index - 1].Icon.sprite = null;
        _itemSlots[_index - 1].Icon.gameObject.SetActive(false);

        _index--;

        return _items.Dequeue();
    }

    public void OnUsedItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (_items.Count <= 0)
        {
            return;
        }

        StartCoroutine(UseItemCoroutine());
    }

    private IEnumerator UseItemCoroutine()
    {
        ItemData item = RemoveItem();
        for (int i = 0; i < item.Usables.Length; i++)
        {
            switch (item.Usables[i].Type)
            {
                case UseType.SpeedBoost:
                    _player.Controller.SpeedBonus += _player.Controller.MoveSpeed * item.Usables[i].Value;
                    yield return new WaitForSeconds(item.Usables[i].Duration);
                    _player.Controller.SpeedBonus -= _player.Controller.MoveSpeed * item.Usables[i].Value;
                    break;
                case UseType.JumpBoost:
                    _player.Controller.JumpPowerBonus += _player.Controller.DefaultJumpPower * item.Usables[i].Value;
                    yield return new WaitForSeconds(item.Usables[i].Duration);
                    _player.Controller.JumpPowerBonus -= _player.Controller.DefaultJumpPower * item.Usables[i].Value;
                    break;
            }
        }

        yield break;
    }
}
