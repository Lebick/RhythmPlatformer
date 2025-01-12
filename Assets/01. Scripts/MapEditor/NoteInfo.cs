using TMPro;
using UnityEngine;

public class NoteInfo : MonoBehaviour
{
    private KeyCode lastestKey = KeyCode.None;

    public KeyCode noteKey;
    public float inputTime;
    public NoteType type;

    public TMP_Text keyText;

    private void Update()
    {
        //inputTime = 

        if(lastestKey != noteKey)
        {
            lastestKey = noteKey;
            UpdateText();
        }
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
            text = "Dash";
        else if (noteKey == InputManager.instance.jumpKey)
            text = "Jump";

        keyText.text = text;
    }
}
