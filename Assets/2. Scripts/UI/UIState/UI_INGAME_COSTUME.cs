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

    //�켱 �ڽ�Ƭ�� �ִ� �ѹ� Ȯ��
    int m_iCostumeMaxCnt;
    //����Ǿ��ִ� �ڽ�Ƭ ��ȣ Ȯ��
    int m_iCurSavedCostume;
    //���� �ڽ�Ƭ ��ȣ Ȯ��
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
        //�ʱ�ȭ
        m_goCostumeUI.SetActive(false);
        m_btLeft.onClick.RemoveAllListeners();
        m_btRight.onClick.RemoveAllListeners();
        m_btExit.onClick.RemoveAllListeners();
        UIManager.instance.OnDisableAlarmUI();
        UIManager.instance.ClearAlarmButtonListener();

        //�ٽ� �븻���·� �ǵ�����
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;
        UIManager.instance.StartCoroutine(CameraMoving.cm.OnInitCameraCoroutine(0f));
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;
        PetController.pc.gameObject.SetActive(true);
        PlayerController.pc.OnSetActiveJoy(true);
        UIManager.instance.ActiveFieldHUD(true);
    }

    //���⼭ ĳ���� �ڽ�Ƭ ����
    void LeftButtonClick()
    {
        //���� �������� �ڽ�Ƭ�� ��������~
        PlayerController.pc.transform.GetChild(m_iCurCostume).gameObject.SetActive(false);

        m_iCurCostume--;

        if (m_iCurCostume < 0) m_iCurCostume = m_iCostumeMaxCnt;

        CostumeChangeOn(m_iCurCostume);
    }

    void RightButtonClick()
    {
        //���� �������� �ڽ�Ƭ�� ��������~
        PlayerController.pc.transform.GetChild(m_iCurCostume).gameObject.SetActive(false);

        m_iCurCostume++;

        if (m_iCurCostume > m_iCostumeMaxCnt) m_iCurCostume = 0;

        CostumeChangeOn(m_iCurCostume);
    }

    void CostumeChangeOn(int CostumeIdx)
    {
        //�ٲ� �ڽ�Ƭ���� �����Ѵ�.
        PlayerController.pc.transform.GetChild(CostumeIdx).gameObject.SetActive(true);

        //�÷��̾� �ִ԰� Ʈ�������� ���� �ٲ��ش�.
        PlayerController.pc.m_curAnim = PlayerController.pc.m_listPlayerCostume[CostumeIdx].GetComponent<Animator>();
        PlayerController.pc.m_curTransform = PlayerController.pc.m_listPlayerCostume[CostumeIdx].GetComponent<Transform>();

        //���������ϴ� �κе� �ش� �ڽ�Ƭ�� ����ٲ��ش�.
        PlayerController.pc.RefreshFindHand(CostumeIdx);

        //�ٽ� ���̵� anim ���ش�.
        PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_IDLE);
    }

    void ExitButtonClick()
    {
        //��������� ���� ��
        if(m_iCurCostume == m_iCurSavedCostume)
        {
            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);
        }

        //��������� ������ ����ֱ� ������ �ٷ� ����ǵ���
        else
        {
            UIManager.instance.ActiveAlarmUI("��������� �����Ͻðڽ��ϱ�?", true);
            UIManager.instance.ConnectAlarmButtonListener(YesButtonClick, NoButtonClick);
        }
    }

    void YesButtonClick()
    {
        //��������ߵ�
        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        playerInfo[0]["COSTUME"] = m_iCurCostume.ToString();

        CSVManager.instance.SaveFile(LOADTYPE.CHARINFO, playerInfo);
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);

        //�÷��̾� ������ �ѹ� ���Ž�Ų��.
        PlayerController.pc.RefreshInfo();
    }

    void NoButtonClick()
    {
        //���� �������� �ڽ�Ƭ�� ��������~
        PlayerController.pc.transform.GetChild(m_iCurCostume).gameObject.SetActive(false);

        //�������·� �ǵ��� ������ ����
        CostumeChangeOn(m_iCurSavedCostume);

        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);
    }
}
