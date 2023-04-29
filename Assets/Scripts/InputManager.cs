using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInputActions PlayerActions;
    public static PlayerInputActions.PlayerActions Player;

    private void Awake()
    {
        PlayerActions = new PlayerInputActions();
        Player = PlayerActions.Player;
    }

    private void OnEnable()
    {
        PlayerActions.Enable();
        Player.Enable();
    }

    private void OnDisable()
    {
        PlayerActions.Disable();
        Player.Disable();
    }
}
