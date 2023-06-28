using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerState playerState;
    public enum PlayerState
    {
        Idle,
        Move,
        Dash,
        Attack,
        Die
    }
}
