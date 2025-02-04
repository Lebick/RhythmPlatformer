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

        if (Input.GetMouseButtonDown(0)) //��Ŭ���� ������ ��
        {
            SelectCheck(); //������ Ŭ���ߴ��� üũ��
        }

        if (Input.GetKeyDown(KeyCode.Delete) && currentSelectedNote != null) //���õ� ��Ʈ�� �����ϰ�, DeleteŰ�� ������ �� �ش� ��� ����
        {
            DeleteNote();
        }

        scrollRect.enabled = (!isRangeDrag && !isNoteDrag); //���� �巡�� �߿� ȭ���� ��ũ�ѵ��� �ʵ��� ��

        if (isRangeDrag) //��Ʈ ���̸� �������� ��
            ChangeNoteRange(); //��Ʈ ���� ����

        if (isNoteDrag) //��Ʈ�� �巡���Ϸ��� �� ��
            ChangeNotePosition(); //��Ʈ ��ġ ����

        if (currentSelectedNote != null) //��Ʈ�� Ȱ��ȭ �Ǿ����� �� �ش� ��Ʈ�� Ű �Է�
            ChangeNoteKey();

        if (isCheckCreate && Input.GetMouseButtonUp(0)) //��Ʈ ������ �����ϰ�, ���콺�� �ôٸ�
        {
            if(lastestClickPos == Input.mousePosition) //��ġ�� ������� �ʾ����� ����
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
        if (Input.GetMouseButton(0)) //��Ŭ���� ������ �ִ� ���¶��
        {
            //���콺 ��ġ�� ���� ��Ʈ�� ������ �κ��� �ű�
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mousePos = new Vector2(mousePos.x * 1920, mousePos.y * 1080 - 540);

            int xPos = (int)(-contentRect.anchoredPosition.x + mousePos.x + offset.x / 2 - 100);

            xPos = GetMagneticPos(xPos);

            xPos = Mathf.Max(xPos, ((int)currentSelectedNote.offsetMin.x - 50));

            currentSelectedNote.offsetMax = new(xPos, currentSelectedNote.offsetMax.y);
        }
        else //��Ŭ���� ������ ���� �ʴٸ�
        {
            //�巡�� ���¸� �����
            isRangeDrag = false;
        }
    }

    private void ChangeNotePosition()
    {
        if (Input.GetMouseButton(0)) //��Ŭ���� ������ �ִ� ���¶��
        {
            //���콺 ��ġ�� ���� ��Ʈ�� �ű�
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mousePos = new Vector2(mousePos.x * 1920, mousePos.y * 1080 - 540);

            int xPos = (int)(-contentRect.anchoredPosition.x + mousePos.x + offset.x / 2);

            xPos = GetMagneticPos(xPos);

            currentSelectedNote.offsetMin = new(xPos, currentSelectedNote.offsetMin.y);
            currentSelectedNote.offsetMax = new(xPos - noteXPos, currentSelectedNote.offsetMax.y);


        }
        else //��Ŭ���� ������ ���� �ʴٸ�
        {
            //�巡�� ���¸� �����
            isNoteDrag = false;
        }
    }

    private void SelectCheck()
    {
        if (IsPointerOverTrack("TimeLineBG")) //Ÿ�Ӷ����� Ŭ������ ��
            return; //�Ʒ� �ڵ� �̽���

        if (IsPointerOverTrack("DragAbleRange")) //���� �巡�� ���� ������ Ŭ������ ��
        {
            isRangeDrag = true; //���̸� �巡�� �� �� �ֵ��� ��
            return;
        }

        if (IsPointerOverTrack("Note"))
        {
            isNoteDrag = true; //��Ʈ�� �巡�� �� �� �ֵ��� ��
            noteXPos = -(-currentSelectedNote.offsetMin.x + currentSelectedNote.offsetMax.x);

            return;
        }

        SetSelectedNote(null);

        if (IsPointerOverTrack("TrackViewport")) //���� �ƴ� ������ Ŭ������ ��
        {
            isCheckCreate = true; //��Ʈ�� ������ �� �ֵ��� ��
            lastestClickPos = Input.mousePosition; //���콺 ��ġ�� ������� �ʾ��� ������ �����ϵ��� �ϱ� ����
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
