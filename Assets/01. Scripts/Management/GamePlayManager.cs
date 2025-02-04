using UnityEditor.UI;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public PlayerPosition player;
    public MapInfo mapInfo;

    public float progress;

    [Range(0.5f, 10)] public float noteSpeedMultiplier;
    

    public bool isTest;
    private bool isPlay;

    protected override void Awake()
    {
        base.Awake();
        FindPlayer();
    }

    private void Update()
    {
        FindPlayer();

        if (Input.GetKeyDown(KeyCode.Keypad0))
            progress = 0;

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            isPlay = !isPlay;
    }

    private void FindPlayer()
    {
        if (player == null && GameObject.Find("Player"))
            player = GameObject.Find("Player").GetComponent<PlayerPosition>();
    }

    private void FixedUpdate()
    {
        if (isTest && isPlay)
        {
            progress += Time.fixedDeltaTime * 100 * mapInfo.bpm / 60f;
        }
    }
}
