using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator mg;

    [SerializeField]
    private MazeCell m_mazeCellPrefab;

    [SerializeField]
    private int m_iMazeWidth;

    [SerializeField]
    private int m_iMazeDepth;

    [HideInInspector]
    public MazeCell[,] m_arrMazeGrid;

    private float m_fPosLBX;
    private float m_fPosLBZ;

    [HideInInspector]
    [Tooltip("��ȯ ���� �����ϴ� ����")]
    public Transform m_trMazeSpawnPoint;

    [HideInInspector]
    [Tooltip("Ż�� ���� ������Ʈ")]
    public GameObject m_goMazeExit;

    [SerializeField]
    GameObject m_goRandomItemPrefab;

    private void Awake()
    {
        mg = this;
    }

    void Start()
    {
        m_arrMazeGrid = new MazeCell[m_iMazeWidth, m_iMazeDepth];

        //�ش� ������Ʈ�� leftbottom ��ġ
        m_fPosLBX = transform.position.x - ((float)(m_iMazeWidth) / 2f) + 0.5f;
        m_fPosLBZ = transform.position.z - ((float)(m_iMazeDepth) / 2f) + 0.5f;

        for (int idxX = 0; idxX < m_iMazeWidth; idxX++)
        {
            for (int idxZ = 0; idxZ < m_iMazeDepth; idxZ++)
            {
                m_arrMazeGrid[idxX, idxZ] = Instantiate(m_mazeCellPrefab, new Vector3(m_fPosLBX + idxX, 0, m_fPosLBZ + idxZ), Quaternion.identity);

                //�θ������Ʈ ����
                m_arrMazeGrid[idxX, idxZ].transform.SetParent(this.transform);
            }
        }

        GenerateMaze(null, m_arrMazeGrid[0, 0]);

        AdditionalElement();
    }

    private void GenerateMaze(MazeCell i_preMazeCell, MazeCell i_curMazeCell)
    {
        i_curMazeCell.Visit();
        ClearWalls(i_preMazeCell, i_curMazeCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(i_curMazeCell);

            if (nextCell != null)
            {
               GenerateMaze(i_curMazeCell, nextCell);
            }

        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell i_curMazeCell)
    {
        var unvisitedCells = GetUnivisitedCells(i_curMazeCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnivisitedCells(MazeCell i_curMazeCell)
    {
        int x = (int)(i_curMazeCell.transform.position.x - m_fPosLBX);
        int z = (int)(i_curMazeCell.transform.position.z - m_fPosLBZ);

        if (x + 1 < m_iMazeWidth)
        {
            var cellToRight = m_arrMazeGrid[x + 1, z];

            if (!cellToRight.m_bVisited)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = m_arrMazeGrid[x - 1, z];

            if (!cellToLeft.m_bVisited)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < m_iMazeDepth)
        {
            var cellToFront = m_arrMazeGrid[x, z + 1];

            if (!cellToFront.m_bVisited)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = m_arrMazeGrid[x, z - 1];

            if (!cellToBack.m_bVisited)
            {
                yield return cellToBack;
            }
        }
    }


    private void ClearWalls(MazeCell i_preMazeCell, MazeCell i_curMazeCell)
    {
        if (i_preMazeCell == null)
        {
            return;
        }

        if (i_preMazeCell.transform.position.x < i_curMazeCell.transform.position.x)
        {
            i_preMazeCell.ClearRightWall();
            i_curMazeCell.ClearLeftWall();
            return;
        }

        if (i_preMazeCell.transform.position.x > i_curMazeCell.transform.position.x)
        {
            i_preMazeCell.ClearLeftWall();
            i_curMazeCell.ClearRightWall();
            return;
        }

        if (i_preMazeCell.transform.position.z < i_curMazeCell.transform.position.z)
        {
            i_preMazeCell.ClearFrontWall();
            i_curMazeCell.ClearBackWall();
            return;
        }

        if (i_preMazeCell.transform.position.z > i_curMazeCell.transform.position.z)
        {
            i_preMazeCell.ClearBackWall();
            i_curMazeCell.ClearFrontWall();
            return;
        }
    }

    private void AdditionalElement()
    {
        //�� ��������� ������ ���� ���� Ż�������� �����.
        int randExitNumX = Random.Range(0, m_iMazeWidth);
        int randExitNumZ = Random.Range(0, m_iMazeWidth);

        m_goMazeExit = m_arrMazeGrid[randExitNumX, randExitNumZ].m_goExitPoint;

        m_goMazeExit.SetActive(true);

        //���� ���� ��ȯ������ ���Ѵ�.(�̶�, Ż���������� �ּ� 5x5�̻� ������ �־�� �Ѵ�.)

        int randSpawnNumX = 0;
        int randSpawnNumZ = 0;

        while (true)
        {
            randSpawnNumX = Random.Range(0, m_iMazeWidth);

            if (Mathf.Abs(randExitNumX - randSpawnNumX) < 5) continue;

            randSpawnNumZ = Random.Range(0, m_iMazeWidth);

            if (Mathf.Abs(randExitNumZ - randSpawnNumZ) < 5) continue;

            break;
        }

        m_trMazeSpawnPoint = m_arrMazeGrid[randSpawnNumX, randSpawnNumZ].transform;

        //������ ���� �������� �����Ѵ�.

        int itemAmount = Random.Range(2, 5);

        for(int idx = 0; idx < itemAmount; idx++)
        {
            int x = Random.Range(0, m_iMazeWidth);
            int z = Random.Range(0, m_iMazeWidth);

            if ((x == randExitNumX && z == randExitNumZ) || (x == randSpawnNumX && z == randSpawnNumZ)) continue;

            Instantiate(m_goRandomItemPrefab, m_arrMazeGrid[x, z].transform.position, Quaternion.identity, m_arrMazeGrid[x, z].transform);
        }

    }

    public IEnumerator InitMaze()
    {
        //gameManager Ÿ���� Maze�� �����Ѵ�.
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.MAZE;

        //MAZE UI ���·� ����
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_MAZE);

        //�����Ǿ�� �ϴ� MazeSpawnPoint�� �÷��̾ ��ġ��Ų��.
        PlayerController.pc.OnTeleportPlayer(m_trMazeSpawnPoint);

        //ī�޶� Ÿ�� ����
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.MAZE_TYPE;

        //ī�޶� ��ġ��Ű��
        StartCoroutine(CameraMoving.cm.OnInitCameraCoroutine(1.0f));

        //ī�޶� ����Ÿ�� Ȱ��ȭ
        yield return new WaitForSeconds(1.0f);
        CameraMoving.cm.m_bMoveTimeCam = true;

        //ī�޶� ����Ÿ�� ��Ȱ��ȭ
        yield return new WaitForSeconds(3.0f);
        CameraMoving.cm.m_bMoveTimeCam = false;
    }
}
