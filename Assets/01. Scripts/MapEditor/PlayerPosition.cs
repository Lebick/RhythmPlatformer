using Alchemy.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPosition : MonoBehaviour
{
    public Vector2 startPosition; //���� ��ġ

    public LineRenderer playerPathView; //�÷��̾� �̵� ��� ����� ��
    public List<Vector2> positions = new(); //��ġ�� �迭
    public int oneFramePositionTask = 100; //�� ������ �� �����ŭ ��ġ���� ���� �����Ұ�����
    public int oneCalculateInterval = 25; //�� �� ��� �� �����ŭ�� ������ �ΰ� ���� ��ġ�� ����� ������

    public UnityEvent calculateEvents = new(); //�� ��� �� ���� �Ͼ �̺�Ʈ

    public MapEditor mapEditor; //������ �� �ٷ� �� ������ ����ǵ��� �ϱ� ����

    private MapInfo map; //�� ����

    public BoxCollider2D playerCollider; //�ݶ��̴� , �ڵ� ��������¥���� offset����� ���� �ټ� �߻�
    public GameObject groundCheck; //�� Ȯ�ο�

    public float moveSpeed; //�����̴� �ӵ�
    public float gravity; //�߷°�
    public Vector2 velocity; //�ӷ� ����
    public float jumpForce; //������

    public bool isDash; //�뽬�� ��������� Ȯ���ϱ� ����
    private float dashResistanceTimer; //�뽬 ���׷�
    public float dashForce = 0.5f; //�뽬 ����
    public float dashTimeMul = 1; //�뽬 Ÿ�� ��Ӱ�

    private List<MapNote> usedNotes = new(); //�뽬, ���� �� 1ȸ�� ���� �۵��ؾ��ϴ� ��Ʈ��
    private Vector2 moveDir; //�����ִ� ����

    //---------------------------------------------------------------------------------------


    [BoxGroup("���� ����")] public List<PlayerAnimInfo> playerAnimInfos = new();
    [BoxGroup("���� ����")] public Transform model;
    [BoxGroup("���� ����")] public Animator anim;
    [BoxGroup("���� ����")] public float dirRotateSpeed;
    [BoxGroup("���� ����")] private Vector2 lastestPos; //���� ������ ��, �ִϸ��̼� ���θ� �����ϱ� ���� ���� ��ġ��
    [BoxGroup("���� ����")] public float groundAnimCoyoteTime = 0.2f;
    [BoxGroup("���� ����")] private float groundAnimCoyoteTimer;


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

        if (floorValue < positions.Count - 1) //�� �ε����� �ƴ϶��
        {
            Vector2 pos1 = positions[floorValue];
            Vector2 pos2 = positions[floorValue + 1];
            pos = Vector2.Lerp(pos1, pos2, value - floorValue);
        }
        else //�� �ε������
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


        //�¿� ������
        anim.SetBool("isMove", lastestPos.x != transform.position.x);

        float dir = transform.position.x - lastestPos.x;

        if (dir != 0)
        {
            if (dir > 0) //���������� �̵�
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
        usedNotes.Clear(); //����� ��Ʈ �ʱ�ȭ
        positions.Clear(); //����Ʈ �ʱ�ȭ
        playerPathView.positionCount = 0; //��ġ ǥ�� �ʱ�ȭ
        velocity = Vector2.zero; //�ӷ°� �ʱ�ȭ
        positions.Add(startPosition); //�ʱⰪ �߰�

        int timeLineLength = Mathf.FloorToInt(map.backgroundMusic.length * map.bpm / 60 / 2);
        int endValue = timeLineLength * 200; //�ݺ� Ƚ��
        int count = 0; //�� ������ ���� ���������� �Ѿ�� ���� ī��Ʈ

        if (mapEditor != null)
            mapEditor.SaveInfo();

        for (int i = 0; i < endValue; i += oneCalculateInterval) //����ȭ�� ���� 25������ ����, ���� ��Ȳ�� ���� ����
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
        print("������ ��");
        yield break;

    }

    private void GetPosition(int index)
    {
        Vector2 dashVelocity = Vector2.zero;
        Vector2 lookDir = Vector2.zero;

        //�߷�, ���� ó��
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

        //�뽬 ����, �ӷ� ���
        if (isDash)
        {
            dashResistanceTimer += 1f / oneCalculateInterval * dashTimeMul;
            float value = 1 - Mathf.Sqrt(1 - Mathf.Pow(dashResistanceTimer - 1, 2));
            dashVelocity = lookDir * dashForce * value;

            //�߷°� ����ȭ
            velocity.y = dashVelocity.y;

            if (dashResistanceTimer <= 0.8f) //0.8���� ������ ������ �� ����
                movePos = pos; //����Ű�� ���� �̵� ����

            if (dashResistanceTimer >= 1)
            {
                isDash = false;
                dashResistanceTimer = 0;
            }
        }

        newPos = movePos; //����Ű�� ������ ���� �ݿ�

        //�뽬 ���� ����
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
