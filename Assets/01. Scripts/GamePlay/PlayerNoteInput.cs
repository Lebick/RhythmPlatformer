using System.Collections.Generic;
using UnityEngine;

public class PlayerNoteInput : MonoBehaviour
{
    public RectTransform judgmentPos; //판정위치

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
                    if (i == j) continue; //자기자신 인덱스 제외

                    if (notes[j].inputTime <= notes[i].inputTime &&
                        notes[j].endInputTime+100 >= notes[i].inputTime) //자기자신을 제외하고, 해당 노트가 눌릴 타이밍에 다른 노트가 존재한다면
                    {
                        if (IsDashOrJump(notes[j])) //만약 방향키가 아닌 다른 키가 존재한다면
                            noteRect.anchoredPosition = new Vector2(noteRect.anchoredPosition.x, 175); //y좌표를 175로 설정
                        else
                            noteRect.anchoredPosition = new Vector2(noteRect.anchoredPosition.x, 100); //y좌표를 100으로 설정
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
        foreach (KeyCode key in InputManager.instance.keys) //입력 가능한 키들을 전부 탐색
            if (Input.GetKey(key)) //입력 가능한 키 중 하나를 입력
                foreach(NoteInfo note in notes) //현재 존재하는 노트들 탐색
                    if(note.noteKey == key) //노트들 중, 입력한 키와 일치하는 노트를 선별
                        CheckAccuracy(note); //정확도 판단
    }

    private void CheckAccuracy(NoteInfo note)
    {
        //방향키는 시작위치, 끝위치가 정확해야함
        //점프 & 대쉬키는 시작위치만 맞으면 됨.

        float value = Mathf.Abs(note.inputTime - GamePlayManager.instance.progress);

        print(value);

        if (note.noteKey == InputManager.instance.dashKey ||
        note.noteKey == InputManager.instance.jumpKey)
        {
            
        }
    }
}
