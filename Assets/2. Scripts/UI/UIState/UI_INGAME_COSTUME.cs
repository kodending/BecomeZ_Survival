using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class UI_INGAME_COSTUME : UIBaseState
{
    GameObject m_goCostumeUI;
    Button m_btLeft;
    Button m_btRight;
    Button m_btExit;

    //우선 코스튬의 최대 넘버 확인
    int m_iCostumeMaxCnt;
    //저장되어있는 코스튬 번호 확인
    int m_iCurSavedCostume;
    //입은 코스튬 번호 확인
    int m_iCurCostume;

    public override void OnEnterState()
    {
        m_goCostumeUI = UIManager.instance.m_goCurCanvas.transform.Find("CostumeUI").gameObject;
        m_goCostumeUI.SetActive(true);

        m_btLeft = m_goCostumeUI.transform.Find("LeftButton").GetComponent<Button>();
        m_btLeft.onClick.AddListener(LeftButtonClick);

        m_btRight = m_goCostumeUI.transform.Find("RightButton").GetComponent<Button>();
        m_btRight.onClick.AddListener(RightButtonClick);

        m_btExit = m_goCostumeUI.transform.Find("ExitButton").GetComponent<Button>();
        m_btExit.onClick.AddListener(ExitButtonClick);

        m_iCostumeMaxCnt = PlayerController.pc.m_listPlayerCostume.Count - 1;
        m_iCurSavedCostume = PlayerController.pc.m_sPlayerInfo.costume;
        m_iCurCostume = m_iCurSavedCostume;

        UIManager.instance.ActiveFieldHUD(false);
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        //초기화
        m_goCostumeUI.SetActive(false);
        m_btLeft.onClick.RemoveAllListeners();
        m_btRight.onClick.RemoveAllListeners();
        m_btExit.onClick.RemoveAllListeners();
        UIManager.instance.OnDisableAlarmUI();
        UIManager.instance.ClearAlarmButtonListener();

        //다시 노말상태로 되돌리기
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;
        UIManager.instance.StartCoroutine(CameraMoving.cm.OnInitCameraCoroutine(0f));
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;
        PetController.pc.gameObject.SetActive(true);
        PlayerController.pc.OnSetActiveJoy(true);
        UIManager.instance.ActiveFieldHUD(true);
    }

    //여기서 캐릭터 코스튬 변경
    void LeftButtonClick()
    {
        //먼저 착용중인 코스튬을 꺼버리고~
        PlayerController.pc.transform.GetChild(m_iCurCostume).gameObject.SetActive(false);

        m_iCurCostume--;

        if (m_iCurCostume < 0) m_iCurCostume = m_iCostumeMaxCnt;

        CostumeChangeOn(m_iCurCostume);
    }

    void RightButtonClick()
    {
        //먼저 착용중인 코스튬을 꺼버리고~
        PlayerController.pc.transform.GetChild(m_iCurCostume).gameObject.SetActive(false);

        m_iCurCostume++;

        if (m_iCurCostume > m_iCostumeMaxCnt) m_iCurCostume = 0;

        CostumeChangeOn(m_iCurCostume);
    }

    void CostumeChangeOn(int CostumeIdx)
    {
        //바뀐 코스튬으로 착용한다.
        PlayerController.pc.transform.GetChild(CostumeIdx).gameObject.SetActive(true);

        //플레이어 애님과 트랜스폼을 맞춰 바꿔준다.
        PlayerController.pc.m_curAnim = PlayerController.pc.m_listPlayerCostume[CostumeIdx].GetComponent<Animator>();
        PlayerController.pc.m_curTransform = PlayerController.pc.m_listPlayerCostume[CostumeIdx].GetComponent<Transform>();

        //무기착용하는 부분도 해당 코스튬에 맞춰바꿔준다.
        PlayerController.pc.RefreshFindHand(CostumeIdx);

        //다시 아이들 anim 해준다.
        PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_IDLE);
    }

    void ExitButtonClick()
    {
        //변경사항이 없을 때
        if(m_iCurCostume == m_iCurSavedCostume)
        {
            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);
        }

        //변경사항이 있을때 띄워주기 없으면 바로 종료되도록
        else
        {
            UIManager.instance.ActiveAlarmUI("변경사항을 저장하시겠습니까?", true);
            UIManager.instance.ConnectAlarmButtonListener(YesButtonClick, NoButtonClick);
        }
    }

    void YesButtonClick()
    {
        //저장해줘야됨
        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        playerInfo[0]["COSTUME"] = m_iCurCostume.ToString();

        CSVManager.instance.SaveFile(LOADTYPE.CHARINFO, playerInfo);
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);

        //플레이어 정보를 한번 갱신시킨다.
        PlayerController.pc.RefreshInfo();
    }

    void NoButtonClick()
    {
        //먼저 착용중인 코스튬을 꺼버리고~
        PlayerController.pc.transform.GetChild(m_iCurCostume).gameObject.SetActive(false);

        //원래상태로 되돌려 보내고 종료
        CostumeChangeOn(m_iCurSavedCostume);

        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);
    }
}
