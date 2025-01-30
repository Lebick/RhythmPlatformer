using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackPlay : MonoBehaviour
{
    public RectTransform trackCursor;

    public MapEditor mapEditor;

    private float currentPlayTime;
    private bool isPlay;

    private bool isWaitOffset;
    private float offsetTimer;

    private bool isCorrection;
    private float offsetCorrection;

    public Canvas canvas;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    private void Update()
    {
        GetCurrentPlayTime();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPlay = !isPlay;

            if (isPlay)
            {
                isWaitOffset = true;
                SoundManager.instance.bgmSource.time = currentPlayTime;
                SoundManager.instance.bgmSource.Play();
            }
            else
            {
                isWaitOffset = false;
                SoundManager.instance.bgmSource.Stop();
            }
        }

        if (Input.GetMouseButtonDown(0) && IsPointerOverTrack("TimeLineBG"))
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.y = trackCursor.position.y;
            trackCursor.position = mousePos;

            if (isPlay)
            {
                GetCurrentPlayTime();

                SoundManager.instance.bgmSource.Stop();
                isWaitOffset = true;
                SoundManager.instance.bgmSource.time = currentPlayTime;
                SoundManager.instance.bgmSource.Play();
            }
        }

        if (isWaitOffset)
        {
            if(mapEditor.mapInfo.offset <= offsetTimer)
            {
                offsetCorrection = offsetTimer - mapEditor.mapInfo.offset;
                isWaitOffset = false;
                isCorrection = true;
                offsetTimer = 0;
                return;
            }
            offsetTimer += Time.deltaTime;
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

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.name.Equals(name))
            {

                return true;
            }
        }

        return false;
    }

    private void GetCurrentPlayTime()
    {
        currentPlayTime = (trackCursor.anchoredPosition.x + 50) / 100 / (mapEditor.mapInfo.bpm / 60f);
    }

    private void FixedUpdate()
    {
        if (isPlay && !isWaitOffset)
            Play();

        //юс╫ц
        GamePlayManager.instance.progress = trackCursor.anchoredPosition.x + 50;
    }

    private void Play()
    {
        if (isCorrection)
        {
            trackCursor.anchoredPosition += new Vector2(offsetCorrection * 100 * mapEditor.mapInfo.bpm / 60f, 0);
            isCorrection = false;
        }

        trackCursor.anchoredPosition += new Vector2(Time.fixedDeltaTime * 100 * mapEditor.mapInfo.bpm / 60f, 0);
    }
}
