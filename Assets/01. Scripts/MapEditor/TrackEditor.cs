using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackEditor : MonoBehaviour
{
    public Sprite noteNormalSprite;
    public Sprite noteSelectSprite;

    public RectTransform contentRect;

    private readonly Vector2 offset = new(-610, -100);

    public List<RectTransform> tracks;

    public GameObject notePrefab;

    private int magneticValue = 25;

    private RectTransform currentSelectedNote;

    public ScrollRect scrollRect;

    public Canvas canvas;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private bool isCheckCreate;
    private Vector3 lastestClickPos;

    private bool isRangeDrag;

    private float noteXPos;
    private bool isNoteDrag;

    private void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0)) //좌클릭을 눌렀을 때
        {
            SelectCheck(); //무엇을 클릭했는지 체크함
        }

        if (Input.GetKeyDown(KeyCode.Delete) && currentSelectedNote != null) //선택된 노트가 존재하고, Delete키를 눌렀을 시 해당 노드 제거
        {
            DeleteNote();
        }

        scrollRect.enabled = (!isRangeDrag && !isNoteDrag); //영역 드래그 중엔 화면이 스크롤되지 않도록 함

        if (isRangeDrag) //노트 길이를 수정중일 때
            ChangeNoteRange(); //노트 길이 수정

        if (isNoteDrag) //노트를 드래그하려고 할 때
            ChangeNotePosition(); //노트 위치 수정

        if (currentSelectedNote != null) //노트가 활성화 되어있을 때 해당 노트에 키 입력
            ChangeNoteKey();

        if (isCheckCreate && Input.GetMouseButtonUp(0)) //노트 생성이 가능하고, 마우스를 뗐다면
        {
            if(lastestClickPos == Input.mousePosition) //위치가 변경되지 않았으면 생성
                CreateNote();

            isCheckCreate = false;
        }
    }

    private void DeleteNote()
    {
        Destroy(currentSelectedNote.gameObject);
        isRangeDrag = false;
        isNoteDrag = false;
        SetSelectedNote(null);
    }

    private void ChangeNoteKey()
    {
        foreach(KeyCode key in InputManager.instance.keys)
        {
            if (Input.GetKeyDown(key))
            {
                currentSelectedNote.GetComponent<EditorNoteInfo>().noteKey = key;
                currentSelectedNote.GetComponent<EditorNoteInfo>().isNotMove = Input.GetKey(KeyCode.LeftAlt);
            }
        }
    }

    private void ChangeNoteRange()
    {
        if (Input.GetMouseButton(0)) //좌클릭을 누르고 있는 상태라면
        {
            //마우스 위치에 따라 노트의 오른쪽 부분을 옮김
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mousePos = new Vector2(mousePos.x * 1920, mousePos.y * 1080 - 540);

            int xPos = (int)(-contentRect.anchoredPosition.x + mousePos.x + offset.x / 2 - 100);

            xPos = GetMagneticPos(xPos);

            xPos = Mathf.Max(xPos, ((int)currentSelectedNote.offsetMin.x - 50));

            currentSelectedNote.offsetMax = new(xPos, currentSelectedNote.offsetMax.y);
        }
        else //좌클릭을 누르고 있지 않다면
        {
            //드래그 상태를 취소함
            isRangeDrag = false;
        }
    }

    private void ChangeNotePosition()
    {
        if (Input.GetMouseButton(0)) //좌클릭을 누르고 있는 상태라면
        {
            //마우스 위치에 따라 노트를 옮김
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mousePos = new Vector2(mousePos.x * 1920, mousePos.y * 1080 - 540);

            int xPos = (int)(-contentRect.anchoredPosition.x + mousePos.x + offset.x / 2);

            xPos = GetMagneticPos(xPos);

            currentSelectedNote.offsetMin = new(xPos, currentSelectedNote.offsetMin.y);
            currentSelectedNote.offsetMax = new(xPos - noteXPos, currentSelectedNote.offsetMax.y);


        }
        else //좌클릭을 누르고 있지 않다면
        {
            //드래그 상태를 취소함
            isNoteDrag = false;
        }
    }

    private void SelectCheck()
    {
        if (IsPointerOverTrack("TimeLineBG")) //타임라인을 클릭했을 때
            return; //아래 코드 미실행

        if (IsPointerOverTrack("DragAbleRange")) //길이 드래그 가능 영역을 클릭했을 때
        {
            isRangeDrag = true; //길이를 드래그 할 수 있도록 함
            return;
        }

        if (IsPointerOverTrack("Note"))
        {
            isNoteDrag = true; //노트를 드래그 할 수 있도록 함
            noteXPos = -(-currentSelectedNote.offsetMin.x + currentSelectedNote.offsetMax.x);

            return;
        }

        SetSelectedNote(null);

        if (IsPointerOverTrack("TrackViewport")) //뭣도 아닌 영역을 클릭했을 때
        {
            isCheckCreate = true; //노트를 생성할 수 있도록 함
            lastestClickPos = Input.mousePosition; //마우스 위치가 변경되지 않았을 때에만 생성하도록 하기 위함
            return;
        }

    }

    private bool IsPointerOverTrack(string name)
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        for(int i=0; i<results.Count; i++)
        {
            if (results[i].gameObject.name.Equals(name))
            {
                if (name.Equals("Note"))
                    SetSelectedNote(results[i].gameObject.GetComponent<RectTransform>());
                else if(name.Equals("DragAbleRange"))
                    SetSelectedNote(results[i].gameObject.transform.parent.GetComponent<RectTransform>());

                return true;
            }
        }

        return false;
    }

    private void CreateNote()
    {
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        mousePos = new Vector2(mousePos.x * 1920, mousePos.y * 1080 - 540);

        int myTrack = tracks.IndexOf(tracks.OrderBy(a => Mathf.Abs((a.anchoredPosition.y + offset.y) - mousePos.y)).First());

        int xPos = (int)(-contentRect.anchoredPosition.x + mousePos.x + offset.x/2);

        xPos = GetMagneticPos(xPos);

        GameObject newNote = Instantiate(notePrefab, tracks[myTrack]);

        newNote.name = "Note";

        RectTransform noteRect = newNote.GetComponent<RectTransform>();

        SetSelectedNote(noteRect);

        noteRect.offsetMin = new(xPos, noteRect.offsetMin.y);
        noteRect.offsetMax = new(xPos - 50, noteRect.offsetMax.y);
    }

    private int GetMagneticPos(int value)
    {
        string newPos = value.ToString("D3");
        int lastTwoIndex = int.Parse(newPos[^2..]);

        List<int> values = new();
        for (int i = 0; i <= 100; i += magneticValue)
        {
            if (magneticValue <= 0) break;

            values.Add(i);
        }

        int nearestValue = values.OrderBy(a => Mathf.Abs(a - lastTwoIndex)).First();

        if (nearestValue == 100)
            newPos = (int.Parse(newPos) + 100).ToString();

        string finalPos = $"{newPos[..^2]}{nearestValue.ToString("D2")[^2..]}";

        return int.Parse(finalPos);
    }

    private void SetSelectedNote(RectTransform note)
    {
        if (currentSelectedNote != null)
            currentSelectedNote.GetComponent<Image>().sprite = noteNormalSprite;


        currentSelectedNote = note;

        if (note != null)
            currentSelectedNote.GetComponent<Image>().sprite = noteSelectSprite;
    }

    public void OnClickMagneticBtn(int value)
    {
        magneticValue = 200 / value;
    }

    public void Initialization()
    {
        for(int i=0; i<tracks.Count; i++)
        {
            for(int j=tracks[i].childCount-1; j>=0; j--)
            {
                Destroy(tracks[i].GetChild(j).gameObject);
            }
        }
    }
}
