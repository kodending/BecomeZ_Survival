using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_INGAME_INVEN : UIBaseState
{
    GameObject m_goInvenUI;
    Button m_btExit;

    public override void OnEnterState()
    {
        UIManager.instance.StartCoroutine(CameraMoving.cm.OnInitCameraCoroutine(0f));

        m_goInvenUI = UIManager.instance.m_goCurCanvas.transform.Find("InventoryInField").gameObject;
        m_goInvenUI.SetActive(true);

        m_btExit = m_goInvenUI.transform.Find("ExitButton").GetComponent<Button>();
        m_btExit.onClick.AddListener(ExitButtonClick);

        UIManager.instance.ActiveFieldHUD(false);

        CameraMoving.cm.m_eCurCamType = CAMERATYPE.INVEN_TYPE;

        InventoryManager.im.InitInven();
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        m_btExit.onClick.RemoveAllListeners();

        UIManager.instance.ActiveFieldHUD(true);

        if(GameManagerInGame.gm.m_gtCurType == GAMETYPE.MAZE)
        {
            CameraMoving.cm.m_eCurCamType = CAMERATYPE.MAZE_TYPE;
        }

        else if(GameManagerInGame.gm.m_gtCurType == GAMETYPE.FIELD)
        {
            CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;
        }

        m_goInvenUI.SetActive(false);

        InventoryManager.im.SaveInvenInfo();

        PlayerController.pc.SavePlayerInfo(0, "LEFTWEAPON", InventoryManager.im.m_iCurLeftIdx);
        PlayerController.pc.SavePlayerInfo(0, "RIGHTWEAPON", InventoryManager.im.m_iCurRightIdx);

        PlayerController.pc.RefreshInfo();
    }

    void ExitButtonClick()
    {
        if (GameManagerInGame.gm.m_gtCurType == GAMETYPE.MAZE)
        {
            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_MAZE);
        }

        else if (GameManagerInGame.gm.m_gtCurType == GAMETYPE.FIELD)
        {
            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);
        }
    }
}
