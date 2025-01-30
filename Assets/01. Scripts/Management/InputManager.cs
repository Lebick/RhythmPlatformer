using System.Collections.Generic;
using UnityEngine;

public enum KeyList
{
    upKey,
    downKey,
    leftKey,
    rightKey,
    jumpKey,
    dashKey,
    none
}

public class InputManager : Singleton<InputManager>
{
    public KeyCode upKey        = KeyCode.UpArrow;
    public KeyCode downKey      = KeyCode.DownArrow;
    public KeyCode leftKey      = KeyCode.LeftArrow;
    public KeyCode rightKey     = KeyCode.RightArrow;
    public KeyCode jumpKey      = KeyCode.Z;
    public KeyCode dashKey      = KeyCode.X;

    public List<KeyCode> keys;

    private void Start()
    {
        UpdateKeyList();
    }

    public KeyList KeyCodeToEnum(KeyCode key)
    {
        if (key == upKey)       return KeyList.upKey;
        if (key == downKey)     return KeyList.downKey;
        if (key == leftKey)     return KeyList.leftKey;
        if (key == rightKey)    return KeyList.rightKey;
        if (key == jumpKey)     return KeyList.jumpKey;
        if (key == dashKey)     return KeyList.dashKey;

        return KeyList.none;
    }

    public KeyCode EnumToKeyCode(KeyList key)
    {
        if (key == KeyList.upKey)       return upKey;
        if (key == KeyList.downKey)     return downKey;
        if (key == KeyList.leftKey)     return leftKey;
        if (key == KeyList.rightKey)    return rightKey;
        if (key == KeyList.jumpKey)     return jumpKey;
        if (key == KeyList.dashKey)     return dashKey;

        return KeyCode.None;
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
