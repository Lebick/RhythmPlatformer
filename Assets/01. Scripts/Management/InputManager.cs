using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public KeyCode upKey    = KeyCode.UpArrow;
    public KeyCode downKey  = KeyCode.DownArrow;
    public KeyCode leftKey  = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey  = KeyCode.Z;
    public KeyCode dashKey  = KeyCode.X;

    public List<KeyCode> keys;

    private void Start()
    {
        UpdateKeyList();
    }

    private void UpdateKeyList()
    {
        keys = new()
        {
            upKey,
            downKey,
            leftKey,
            rightKey,
            jumpKey,
            dashKey
        };
    }
}
