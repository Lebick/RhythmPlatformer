using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorNoteInfo : MonoBehaviour
{
    private KeyCode lastestKey = KeyCode.None;

    public KeyCode noteKey;
    public bool isNotMove;

    public TMP_Text keyText;

    private void Update()
    {
        if(lastestKey != noteKey)
        {
            lastestKey = noteKey;
            UpdateText();
        }

        if (isNotMove)
            GetComponent<Image>().color = Color.red;
        else
            GetComponent<Image>().color = Color.white;

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
        else if (noteKey == InputManager.instance.attackKey)
            text = InputManager.instance.attackKey.ToString();

        keyText.text = text;
    }
}
