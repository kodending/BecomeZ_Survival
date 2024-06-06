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
    [Tooltip("소환 지점 저장하는 공간")]
    public Transform m_trMazeSpawnPoint;

    [HideInInspector]
    [Tooltip("탈출 지점 오브젝트")]
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

        //해당 오브젝트의 leftbottom 위치
        m_fPosLBX = transform.position.x - ((float)(m_iMazeWidth) / 2f) + 0.5f;
        m_fPosLBZ = transform.position.z - ((float)(m_iMazeDepth) / 2f) + 0.5f;

        for (int idxX = 0; idxX < m_iMazeWidth; idxX++)
        {
            for (int idxZ = 0; idxZ < m_iMazeDepth; idxZ++)
            {
                m_arrMazeGrid[idxX, idxZ] = Instantiate(m_mazeCellPrefab, new Vector3(m_fPosLBX + idxX, 0, m_fPosLBZ + idxZ), Quaternion.identity);

                //부모오브젝트 설정
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
        //다 만들어지면 임의의 랜덤 셀에 탈출지점을 만든다.
        int randExitNumX = Random.Range(0, m_iMazeWidth);
        int randExitNumZ = Random.Range(0, m_iMazeWidth);

        m_goMazeExit = m_arrMazeGrid[randExitNumX, randExitNumZ].m_goExitPoint;

        m_goMazeExit.SetActive(true);

        //랜덤 셀에 소환지점을 정한다.(이때, 탈출지점에서 최소 5x5이상 떨어져 있어야 한다.)

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

        //랜덤한 셀에 아이템을 생성한다.

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
        //gameManager 타입을 Maze로 변경한다.
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.MAZE;

        //MAZE UI 상태로 변경
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_MAZE);

        //스폰되어야 하는 MazeSpawnPoint에 플레이어를 위치시킨다.
        PlayerController.pc.OnTeleportPlayer(m_trMazeSpawnPoint);

        //카메라 타입 변경
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.MAZE_TYPE;

        //카메라 위치시키기
        StartCoroutine(CameraMoving.cm.OnInitCameraCoroutine(1.0f));

        //카메라 무빙타임 활성화
        yield return new WaitForSeconds(1.0f);
        CameraMoving.cm.m_bMoveTimeCam = true;

        //카메라 무빙타임 비활성화
        yield return new WaitForSeconds(3.0f);
        CameraMoving.cm.m_bMoveTimeCam = false;
    }
}
