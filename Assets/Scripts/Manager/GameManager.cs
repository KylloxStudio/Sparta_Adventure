using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public Player Player { get; set; }
    [SerializeField] private ItemObject[] _itemObjects;

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnResetEvent();
        }
    }

    public void OnResetEvent()
    {
        Player.ItemSlot.Clear();
        Player.transform.SetPositionAndRotation(Player.InitialPosition, Quaternion.Euler(0f, 0f, 0f));

        for (int i = 0; i < _itemObjects.Length; i++)
        {
            _itemObjects[i].gameObject.SetActive(true);
        }
    }
}
