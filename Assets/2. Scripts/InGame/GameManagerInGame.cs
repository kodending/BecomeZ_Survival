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

        //�ӽ�
        Object obj = CSVManager.Instance;
        Object obj2 = UIManager.Instance;
    }

    private void Update()
    {
        if(m_fMazeTimer <= 0)
        {
            UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                                               ("GAME OVER", 2f));

            //�ٽ� ���� �ʵ���·� ���ư���
            GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;

            //�÷��̾� ��ġ �ű��
            PlayerController.pc.OnTeleportPlayer(CameraMoving.cm.m_trCamList[3]);

            //ī�޶� �ʵ�� �ٲٱ�
            CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;

            //�����ϰ� �ִ� ���� ���ش�.
            InventoryManager.im.ClearWeapon();

            //UI�� �������
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
