using System.Collections.Generic;
using UnityEngine;

public class PlayerNoteInput : MonoBehaviour
{
    public RectTransform judgmentPos; //������ġ

    public List<NoteInfo> notes = new();

    private void Update()
    {
        if (notes.Count > 0)
        {
            UpdateNotePosition();
            UpdateNoteInput();
        }
    }

    private void UpdateNotePosition()
    {
        for(int i=0; i<notes.Count; i++)
        {
            RectTransform noteRect = notes[i].transform as RectTransform;

            if (IsDashOrJump(notes[i]))
            {
                for(int j=0; j<notes.Count; j++)
                {
                    if (i == j) continue; //�ڱ��ڽ� �ε��� ����

                    if (notes[j].inputTime <= notes[i].inputTime &&
                        notes[j].endInputTime+100 >= notes[i].inputTime) //�ڱ��ڽ��� �����ϰ�, �ش� ��Ʈ�� ���� Ÿ�ֿ̹� �ٸ� ��Ʈ�� �����Ѵٸ�
                    {
                        if (IsDashOrJump(notes[j])) //���� ����Ű�� �ƴ� �ٸ� Ű�� �����Ѵٸ�
                            noteRect.anchoredPosition = new Vector2(noteRect.anchoredPosition.x, 175); //y��ǥ�� 175�� ����
                        else
                            noteRect.anchoredPosition = new Vector2(noteRect.anchoredPosition.x, 100); //y��ǥ�� 100���� ����
                    }
                }
            }

            float widthValue = (notes[i].endInputTime - notes[i].inputTime + 50) * GamePlayManager.instance.noteSpeedMultiplier;


            noteRect.sizeDelta = new Vector2(100 + widthValue, 100);

            float progress = notes[i].inputTime - GamePlayManager.instance.progress;

            float xPos = progress * GamePlayManager.instance.noteSpeedMultiplier + (widthValue / 2);

            noteRect.anchoredPosition = new Vector2(xPos, noteRect.anchoredPosition.y);
        }
    }

    private bool IsDashOrJump(NoteInfo note)
    {
        return note.noteKey == InputManager.instance.dashKey || note.noteKey == InputManager.instance.jumpKey;
    }

    private void UpdateNoteInput()
    {
        foreach (KeyCode key in InputManager.instance.keys) //�Է� ������ Ű���� ���� Ž��
            if (Input.GetKey(key)) //�Է� ������ Ű �� �ϳ��� �Է�
                foreach(NoteInfo note in notes) //���� �����ϴ� ��Ʈ�� Ž��
                    if(note.noteKey == key) //��Ʈ�� ��, �Է��� Ű�� ��ġ�ϴ� ��Ʈ�� ����
                        CheckAccuracy(note); //��Ȯ�� �Ǵ�
    }

    private void CheckAccuracy(NoteInfo note)
    {
        //����Ű�� ������ġ, ����ġ�� ��Ȯ�ؾ���
        //���� & �뽬Ű�� ������ġ�� ������ ��.

        float value = Mathf.Abs(note.inputTime - GamePlayManager.instance.progress);

        print(value);

        if (note.noteKey == InputManager.instance.dashKey ||
        note.noteKey == InputManager.instance.jumpKey)
        {
            
        }
    }
}
