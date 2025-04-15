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
    attackKey,
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
    public KeyCode attackKey    = KeyCode.C;

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
        if (key == attackKey)   return KeyList.attackKey;

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
        if (key == KeyList.attackKey)   return attackKey;

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
            attackKey,
            dashKey
        };
    }
}
