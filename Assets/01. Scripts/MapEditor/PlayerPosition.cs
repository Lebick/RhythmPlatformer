using Alchemy.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPosition : MonoBehaviour
{
    public Vector2 startPosition; //시작 위치

    public LineRenderer playerPathView; //플레이어 이동 경로 디버깅 용
    public List<Vector2> positions = new(); //위치값 배열
    public int oneFramePositionTask = 100; //한 프레임 당 어느만큼 위치값을 새로 갱신할것인지
    public int oneCalculateInterval = 25; //한 번 계산 후 어느만큼의 간격을 두고 다음 위치를 계산할 것인지

    public UnityEvent calculateEvents = new(); //매 계산 시 마다 일어날 이벤트

    public MapEditor mapEditor; //랜더링 시 바로 맵 정보에 저장되도록 하기 위함

    private MapInfo map; //맵 정보

    public BoxCollider2D playerCollider; //콜라이더 , 코드 개떡같이짜놔서 offset변경시 문제 다수 발생
    public GameObject groundCheck; //땅 확인용

    public float moveSpeed; //움직이는 속도
    public float gravity; //중력값
    public Vector2 velocity; //속력 계산용
    public float jumpForce; //점프력

    public bool isDash; //대쉬를 사용중인지 확인하기 위함
    private float dashResistanceTimer; //대쉬 저항력
    public float dashForce = 0.5f; //대쉬 강도
    public float dashTimeMul = 1; //대쉬 타임 배속값

    private List<MapNote> usedNotes = new(); //대쉬, 점프 등 1회에 한해 작동해야하는 노트들
    private Vector2 moveDir; //가고있는 방향

    //---------------------------------------------------------------------------------------


    [BoxGroup("실행 설정")] public List<PlayerAnimInfo> playerAnimInfos = new();
    [BoxGroup("실행 설정")] public Transform model;
    [BoxGroup("실행 설정")] public Animator anim;
    [BoxGroup("실행 설정")] public float dirRotateSpeed;
    [BoxGroup("실행 설정")] private Vector2 lastestPos; //실제 움직일 때, 애니메이션 여부를 결정하기 위한 이전 위치값
    [BoxGroup("실행 설정")] public float groundAnimCoyoteTime = 0.2f;
    [BoxGroup("실행 설정")] private float groundAnimCoyoteTimer;


    private void Start()
    {
        map = GamePlayManager.instance.mapInfo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Renderering());
        }
    }

    private void FixedUpdate()
    {
        if (positions.Count > 0)
            SetTransform();
    }

    private bool IsGround(Vector2 pos)
    {
        Vector2 centerPos = pos + (Vector2)groundCheck.transform.localPosition * groundCheck.transform.lossyScale;
        Vector2 size = groundCheck.GetComponent<BoxCollider2D>().size * groundCheck.transform.lossyScale;

        if (Physics2D.OverlapBox(centerPos, size, 0, LayerMask.GetMask("Ground"))) return true;

        return false;
    }

    private void SetTransform()
    {
        lastestPos = transform.position;

        float value = (GamePlayManager.instance.progress) / oneCalculateInterval;
        int floorValue = Mathf.FloorToInt(value);

        Vector2 pos;

        if (floorValue < positions.Count - 1) //끝 인덱스가 아니라면
        {
            Vector2 pos1 = positions[floorValue];
            Vector2 pos2 = positions[floorValue + 1];
            pos = Vector2.Lerp(pos1, pos2, value - floorValue);
        }
        else //끝 인덱스라면
        {
            pos = positions[floorValue];
        }

        transform.position = pos;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        foreach(PlayerAnimInfo animInfo in playerAnimInfos)
        {
            if (animInfo.time <= GamePlayManager.instance.progress)
            {
                if (!animInfo.isUsed)
                {
                    animInfo.isUsed = true;
                    anim.SetTrigger(animInfo.triggerName);
                }
            }
            else
            {
                animInfo.isUsed = false;
            }
        }


        //좌우 움직임
        anim.SetBool("isMove", lastestPos.x != transform.position.x);

        float dir = transform.position.x - lastestPos.x;

        if (dir != 0)
        {
            if (dir > 0) //오른쪽으로 이동
                model.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(model.transform.localEulerAngles.y, 90, dirRotateSpeed * Time.fixedDeltaTime), 0);
            else
                model.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(model.transform.localEulerAngles.y, 270, dirRotateSpeed * Time.fixedDeltaTime), 0);
        }

        if (IsGround(transform.position))
        {
            anim.SetBool("isGround", true);
            groundAnimCoyoteTimer = 0;
        }
        else
        {
            groundAnimCoyoteTimer += Time.fixedDeltaTime;
            if(groundAnimCoyoteTimer >= groundAnimCoyoteTime)
            {
                anim.SetBool("isGround", false);
            }
        }
    }

    private IEnumerator Renderering()
    {
        usedNotes.Clear(); //사용한 노트 초기화
        positions.Clear(); //리스트 초기화
        playerPathView.positionCount = 0; //위치 표시 초기화
        velocity = Vector2.zero; //속력값 초기화
        positions.Add(startPosition); //초기값 추가

        int timeLineLength = Mathf.FloorToInt(map.backgroundMusic.length * map.bpm / 60 / 2);
        int endValue = timeLineLength * 200; //반복 횟수
        int count = 0; //렉 방지용 다음 프레임으로 넘어가기 위한 카운트

        if (mapEditor != null)
            mapEditor.SaveInfo();

        for (int i = 0; i < endValue; i += oneCalculateInterval) //최적화를 위해 25단위로 측정, 추후 상황에 따라 조정
        {
            count++;

            GetPosition(i);
            calculateEvents?.Invoke();

            if (count >= oneFramePositionTask)
            {
                count = 0;
                yield return null;
            }
        }
        print("렌더링 끝");
        yield break;

    }

    private void GetPosition(int index)
    {
        Vector2 dashVelocity = Vector2.zero;
        Vector2 lookDir = Vector2.zero;

        //중력, 마찰 처리
        if (IsGround(positions[^1]))
        {
            velocity.x = 0;
            velocity.y = 0;
        }
        else
        {
            velocity.y -= gravity * oneCalculateInterval;
        }

        Vector2 pos = positions[^1];
        Vector2 newPos = pos;
        Vector2 movePos = newPos;

        for (int i = 0; i < map.trackNote.Count; i++)
        {
            for (int j = 0; j < map.trackNote[i].notes.Count; j++)
            {
                if (map.trackNote[i].notes[j].startTime <= index + 50 &&
                    map.trackNote[i].notes[j].endTime > index - 50)
                {
                    FindKey(i, j, index);

                    lookDir = new Vector2(moveDir.x != 0 ? moveDir.x : lookDir.x,
                                        moveDir.y != 0 ? moveDir.y : lookDir.y);

                    movePos += new Vector2(moveDir.x * moveSpeed, 0);
                }
            }
        }

        movePos += new Vector2(velocity.x, velocity.y);
        movePos.x = CollisionCheck(Vector2.right, pos, movePos).x;
        movePos.y = CollisionCheck(Vector2.up, pos, movePos).y;

        //대쉬 방향, 속력 계산
        if (isDash)
        {
            dashResistanceTimer += 1f / oneCalculateInterval * dashTimeMul;
            float value = 1 - Mathf.Sqrt(1 - Mathf.Pow(dashResistanceTimer - 1, 2));
            dashVelocity = lookDir * dashForce * value;

            //중력값 동기화
            velocity.y = dashVelocity.y;

            if (dashResistanceTimer <= 0.8f) //0.8보다 작으면 움직일 수 없음
                movePos = pos; //방향키에 의한 이동 제거

            if (dashResistanceTimer >= 1)
            {
                isDash = false;
                dashResistanceTimer = 0;
            }
        }

        newPos = movePos; //방향키로 움직인 방향 반영

        //대쉬 상태 적용
        if (isDash)
        {
            newPos += dashVelocity;
            newPos.x = CollisionCheck(Vector2.right, pos, newPos).x;
            newPos.y = CollisionCheck(Vector2.up, pos, newPos).y;
        }

        positions.Add(newPos);
        playerPathView.positionCount++;
        playerPathView.SetPosition(playerPathView.positionCount - 1, newPos);
    }

    private Vector2 CollisionCheck(Vector2 checkDir, Vector2 currentPos, Vector2 nextPos)
    {
        bool isAdded;

        if (checkDir.x == 1)
            isAdded = nextPos.x - currentPos.x > 0;
        else
            isAdded = nextPos.y - currentPos.y > 0;

        Vector2 boxValue = Vector2.zero;

        if (checkDir.x != 0)
            boxValue = new Vector2((playerCollider.size.x / 2) * checkDir.x, 0);
        else if (checkDir.y != 0)
            boxValue = new Vector2(0, (playerCollider.size.y / 2) * checkDir.y);

        Vector2 endPos = currentPos + (nextPos - currentPos) * checkDir + boxValue * (isAdded ? 1 : -1);

        RaycastHit2D ray = Physics2D.Linecast(currentPos, endPos, LayerMask.GetMask("Ground"));

        if (!ray)
            return nextPos;

        Vector2 newPos = ray.point;
        if (checkDir.x != 0)
            newPos.x -= boxValue.x * (isAdded ? 1 : -1);
        else if (checkDir.y != 0)
            newPos.y -= boxValue.y * (isAdded ? 1 : -1);

        return newPos;
    }

    private void FindKey(int i, int j, int index)
    {
        int h = 0;
        int v = 0;

        MapNote currentNote = map.trackNote[i].notes[j];

        if (currentNote.requireKey == KeyList.rightKey)
            h++;

        if (currentNote.requireKey == KeyList.leftKey)
            h--;

        if (currentNote.requireKey == KeyList.upKey)
            v++;

        if (currentNote.requireKey == KeyList.downKey)
            v--;

        if (currentNote.requireKey == KeyList.dashKey && !usedNotes.Contains(currentNote))
        {
            isDash = true;
            usedNotes.Add(currentNote);
            playerAnimInfos.Add(new PlayerAnimInfo("Dash", index));
        }

        if (currentNote.requireKey == KeyList.jumpKey && !usedNotes.Contains(currentNote))
        {
            velocity.y = jumpForce;
            usedNotes.Add(currentNote);
            playerAnimInfos.Add(new PlayerAnimInfo("Jump", index));
        }

        if (!currentNote.isNotMove)
            moveDir = new Vector2(h, v);
    }
}

[System.Serializable]
public class PlayerAnimInfo
{
    public string triggerName;
    public float time;
    public bool isUsed;

    public PlayerAnimInfo(string triggerName, float time)
    {
        this.triggerName = triggerName;
        this.time = time;
    }
}
