using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMETYPE
{
    NONE,
    START,
    FIELD,
    BATTLE,
    BOSS,
    FARMING,
    MAZE,
    COSTUME,
    PETCHANGE,
    END,
    _MAX_
}

public enum MAPTYPE
{
    FOREST,
    _MAX_
}

public class GameManagerInGame : MonoBehaviour
{
    public static GameManagerInGame gm;

    public GAMETYPE m_gtCurType;

    public float m_fMazeTimer;

    private void Awake()
    {
        gm = this;
        m_gtCurType = GAMETYPE.FIELD;
        m_fMazeTimer = 180.0f;

        //임시
        Object obj = CSVManager.Instance;
        Object obj2 = UIManager.Instance;
    }

    private void Update()
    {
        if(m_fMazeTimer <= 0)
        {
            UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                                               ("GAME OVER", 2f));

            //다시 원래 필드상태로 돌아가기
            GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;

            //플레이어 위치 옮기기
            PlayerController.pc.OnTeleportPlayer(CameraMoving.cm.m_trCamList[3]);

            //카메라 필드로 바꾸기
            CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;

            //착용하고 있던 무기 없앤다.
            InventoryManager.im.ClearWeapon();

            //UI를 원래대로
            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);

            CameraMoving.cm.FieldCameraRot();

            ResetMazeTimer();
        }
    }

    public void ResetMazeTimer()
    {
        m_fMazeTimer = 180.0f;
    }

    public void StartBattle()
    {
        BattleSystem.bs.InitStateMachine();
    }
}
