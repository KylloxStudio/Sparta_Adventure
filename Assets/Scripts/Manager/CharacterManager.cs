using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    private Player _player;
    public Player Player
    {
        get => _player;
        set => _player = value;
    }


}
