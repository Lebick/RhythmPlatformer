using Coffee.UISoftMask;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteInfo : MonoBehaviour
{
    public float inputTime;
    public float endInputTime;

    public KeyCode noteKey;

    public TMP_Text keyText;

    public void Setting(MapNote note)
    {
        GetComponent<Image>().color = Color.white;
        inputTime = note.startTime;
        endInputTime = note.endTime;
        noteKey = InputManager.instance.EnumToKeyCode(note.requireKey);
        UpdateText();
    }

    private void UpdateText()
    {
        string text = string.Empty;

        if (noteKey == InputManager.instance.leftKey)
            text = "¢¸";
        else if (noteKey == InputManager.instance.rightKey)
            text = "¢º";
        else if (noteKey == InputManager.instance.upKey)
            text = "¡ã";
        else if (noteKey == InputManager.instance.downKey)
            text = "¡å";
        else if (noteKey == InputManager.instance.dashKey)
            text = InputManager.instance.dashKey.ToString();
        else if (noteKey == InputManager.instance.jumpKey)
            text = InputManager.instance.jumpKey.ToString();

        keyText.text = text;
    }
}
