using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteInfo : MonoBehaviour
{
    private KeyCode lastestKey = KeyCode.None;

    public KeyCode noteKey;
    //public float inputTime;
    public bool isNotMove;

    public TMP_Text keyText;

    private void Update()
    {
        //inputTime = 

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
            text = "��";
        else if (noteKey == InputManager.instance.rightKey)
            text = "��";
        else if (noteKey == InputManager.instance.upKey)
            text = "��";
        else if (noteKey == InputManager.instance.downKey)
            text = "��";
        else if (noteKey == InputManager.instance.dashKey)
            text = "Dash";
        else if (noteKey == InputManager.instance.jumpKey)
            text = "Jump";

        keyText.text = text;
    }
}
