using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_INGAME_PET : UIBaseState
{
    GameObject m_goPetUI;
    Button m_btLeft;
    Button m_btRight;
    Button m_btExit;
    Button m_btActivePet;
    Button m_btInactivePet;
    Button m_btLock;
    GameObject m_goLock;
    GameObject m_goInfo;

    int m_iSavedFollowPetIndex;

    int m_iCurPetInfoIdx;

    int m_iMonsterMaxIdx;

    List<Dictionary<string, object>> m_monsterInfoList;

    List<int> m_myPetIdxList;

    public PETINFO m_sPetInfo;


    public override void OnEnterState()
    {
        m_goPetUI = UIManager.instance.m_goCurCanvas.transform.Find("PetUI").gameObject;
        m_goPetUI.SetActive(true);

        m_goLock = m_goPetUI.transform.Find("LockBack").gameObject;

        m_goInfo = m_goPetUI.transform.Find("Info").gameObject;

        m_btLock = m_goLock.transform.Find("LockButton").GetComponent<Button>();
        m_btLock.onClick.AddListener(LockButtonClick);

        m_btLeft = m_goPetUI.transform.Find("LeftButton").GetComponent<Button>();
        m_btLeft.onClick.AddListener(LeftButtonClick);

        m_btRight = m_goPetUI.transform.Find("RightButton").GetComponent<Button>();
        m_btRight.onClick.AddListener(RightButtonClick);

        m_btExit = m_goPetUI.transform.Find("ExitButton").GetComponent<Button>();
        m_btExit.onClick.AddListener(ExitButtonClick);

        m_btActivePet = m_goPetUI.transform.Find("ActivePetButton").GetComponent<Button>();
        m_btActivePet.onClick.AddListener(ActiveFollowButtonClick);

        m_btInactivePet = m_goPetUI.transform.Find("InactivePetButton").GetComponent<Button>();
        m_btInactivePet.onClick.AddListener(InactiveFollowButtonClick);

        m_iSavedFollowPetIndex = CurSavedFollowPetIdx();
        m_iCurPetInfoIdx = m_iSavedFollowPetIndex;

        m_monsterInfoList = CSVManager.instance.m_dicData[LOADTYPE.MONSTER].recordDataList;

        m_iMonsterMaxIdx = m_monsterInfoList.Count;

        m_myPetIdxList = RefreshMyPetIndex();

        RefreshPet(m_iCurPetInfoIdx);

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
        m_goPetUI.SetActive(false);
        m_btLeft.onClick.RemoveAllListeners();
        m_btRight.onClick.RemoveAllListeners();
        m_btExit.onClick.RemoveAllListeners();
        UIManager.instance.OnDisableAlarmUI();
        UIManager.instance.ClearAlarmButtonListener();

        GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;
        UIManager.instance.StartCoroutine(CameraMoving.cm.OnInitCameraCoroutine(0f));
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;
        PetController.pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_FOLLOW);
        PlayerController.pc.gameObject.SetActive(true);
        PlayerController.pc.OnSetActiveJoy(true);
        PlayerController.pc.OnTeleportPlayer(CameraMoving.cm.m_trCamList[(int)CAMERATYPE.PET_TYPE]);
        UIManager.instance.ActiveFieldHUD(true);
    }


    int CurSavedFollowPetIdx()
    {
        int petIdx = 0;

        List<Dictionary<string, object>> petInfoList = CSVManager.instance.m_dicData[LOADTYPE.MYPETINFO].recordDataList;

        for (int idx = 0; idx < petInfoList.Count; idx++)
        {
            if (int.Parse(petInfoList[idx]["FOLLOW"].ToString()) == 1)
            {
                petIdx = int.Parse(petInfoList[idx]["INDEX"].ToString());
            }
        }

        return petIdx;
    }

    void LeftButtonClick()
    {
        m_iCurPetInfoIdx--;

        if (m_iCurPetInfoIdx <= 0) m_iCurPetInfoIdx = m_iMonsterMaxIdx;

        RefreshPet(m_iCurPetInfoIdx);
    }

    void RightButtonClick()
    {
        m_iCurPetInfoIdx++;

        if (m_iCurPetInfoIdx > m_iMonsterMaxIdx) m_iCurPetInfoIdx = 1;

        RefreshPet(m_iCurPetInfoIdx);
    }

    void RefreshPet(int m_iCurPetIdx)
    {
        //�ѹ� �ʱ�ȭ
        ActivePetInfo(false, 0);
        ActiveLock(false);

        m_myPetIdxList = RefreshMyPetIndex();

        GameObject goMonster;
        string strPetIndex = "MONSTER/" + m_monsterInfoList[m_iCurPetIdx - 1]["INDEX"].ToString();
        goMonster = Resources.Load(strPetIndex) as GameObject;

        //���� �� ����
        Destroy(PetController.pc.m_goCurPet);

        //��ü�� ������ ǥ��
        PetController.pc.m_goCurPet = Instantiate(goMonster, PetController.pc.transform);
        PetController.pc.m_petAnim = PetController.pc.m_goCurPet.GetComponent<Animator>();

        PetController.pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);

        bool isLockPet = true;

        for(int idx = 0; idx < m_myPetIdxList.Count; idx++)
        {
            if(m_iCurPetIdx == m_myPetIdxList[idx])
            {
                isLockPet = false;
                ActivePetInfo(true, m_iCurPetIdx);
                break;
            }
        }

        if(isLockPet)
        {
            ActiveLock(true);
        }
    }

    List<int> RefreshMyPetIndex()
    {
        if(m_myPetIdxList != null)
            m_myPetIdxList.Clear();

        List<int> list = new List<int>();

        List<Dictionary<string, object>> myPetList = CSVManager.instance.m_dicData[LOADTYPE.MYPETINFO].recordDataList;

        for (int idx = 0; idx < myPetList.Count; idx++)
        {
            int petIdx = int.Parse(myPetList[idx]["INDEX"].ToString());

            list.Add(petIdx);
        }

        return list;
    }

    void ActivePetInfo(bool b_bActive, int i_iCurPetIdx)
    {
        if(!b_bActive)
        {
            m_goInfo.SetActive(false);
            return;
        }

        m_goInfo.SetActive(b_bActive);

        //�� ���� ����
        List<Dictionary<string, object>> myPetList = CSVManager.instance.m_dicData[LOADTYPE.MYPETINFO].recordDataList;

        Text levelTxt   = m_goInfo.transform.Find("LevelText").GetComponent<Text>();
        Text hpTxt      = m_goInfo.transform.Find("HpText").GetComponent<Text>();
        Text atkTxt     = m_goInfo.transform.Find("AtkText").GetComponent<Text>();
        Text spdTxt     = m_goInfo.transform.Find("SpdText").GetComponent<Text>();
        Text defTxt     = m_goInfo.transform.Find("DefText").GetComponent<Text>();

        for(int idx = 0; idx < myPetList.Count; idx++)
        {
            if (i_iCurPetIdx == int.Parse(myPetList[idx]["INDEX"].ToString()))
            {
                levelTxt.text   = myPetList[idx]["LEVEL"].ToString();
                hpTxt.text      = myPetList[idx]["HP"].ToString();
                atkTxt.text     = myPetList[idx]["ATK"].ToString();
                spdTxt.text     = myPetList[idx]["SPD"].ToString();
                defTxt.text     = myPetList[idx]["DEF"].ToString();

                if(int.Parse(myPetList[idx]["FOLLOW"].ToString()) == 1)
                {
                    m_btInactivePet.gameObject.SetActive(false);
                    m_btActivePet.gameObject.SetActive(true);
                }

                else
                {
                    m_btInactivePet.gameObject.SetActive(true);
                    m_btActivePet.gameObject.SetActive(false);
                }

                break;
            }
        }
    }

    void ActiveLock(bool i_bActive)
    {
        m_goLock.SetActive(i_bActive);

        Text MoneyTxt = m_goLock.transform.Find("MoneyText").GetComponent<Text>();

        MoneyTxt.text = m_monsterInfoList[m_iCurPetInfoIdx - 1]["MONEY"].ToString();

        m_btInactivePet.gameObject.SetActive(false);
        m_btActivePet.gameObject.SetActive(false);
    }

    void LockButtonClick()
    {
        UIManager.instance.ActiveAlarmUI("�����Ͻðڽ��ϱ�?", true);
        UIManager.instance.ConnectAlarmButtonListener(YesButtonClick, NoButtonClick);
    }

    void ExitButtonClick()
    {
        //�ٽ� ���� �ʵ���·� �ǵ��ư���
        //�ʱ�ȭ �ؾ��Ұ� �ʱ�ȭ�ϰ�
        //���� �� ����
        Destroy(PetController.pc.m_goCurPet);

        //���� �ٽ� �ֽ�ȭ�ؾߵ�
        PetController.pc.RefreshPetInfo();

        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_NORMAL);
    }

    void YesButtonClick()
    {
        //���� ������ �ݾװ� ���ϱ�
        int iPurchaseMoney = int.Parse(m_monsterInfoList[m_iCurPetInfoIdx - 1]["MONEY"].ToString());

        //���ź�뺸�� �� �������� �۴ٸ�
        if(iPurchaseMoney > PlayerController.pc.m_sPlayerInfo.myMoney)
        {
            //�ý��۸޼���â�� ����
            UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg("�������� �����մϴ�", 2f));

            //�������� â�� ��������.
            UIManager.instance.OnDisableAlarmUI();
            UIManager.instance.ClearAlarmButtonListener();
        }

        //�������� ����ϴٸ�
        else
        {
            //�÷��̾� �ݾ� �ֽ�ȭ�ؼ� �����ϰ�
            int myMoney = PlayerController.pc.m_sPlayerInfo.myMoney - iPurchaseMoney;

            List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

            playerInfo[0]["MONEY"] = myMoney.ToString();

            CSVManager.instance.SaveFile(LOADTYPE.CHARINFO, playerInfo);

            PlayerController.pc.RefreshInfo();

            UIManager.instance.RefreshMyMoneyHud();

            //�� ������������� �����.
            //���� ���ο� �� ������ ���� ��������
            Dictionary<string, object> dicPetInfo = new Dictionary<string, object>();

            //index, name, hp, atk, spd, def, level, curhp, nextexp, curexp, follow �־�ߵ�
            dicPetInfo.Add("INDEX"      , m_monsterInfoList[m_iCurPetInfoIdx - 1]["INDEX"]);
            dicPetInfo.Add("NAME"       , m_monsterInfoList[m_iCurPetInfoIdx - 1]["NAME"].ToString());
            dicPetInfo.Add("HP"         , m_monsterInfoList[m_iCurPetInfoIdx - 1]["HP"]);
            dicPetInfo.Add("ATK"        , m_monsterInfoList[m_iCurPetInfoIdx - 1]["ATK"]);
            dicPetInfo.Add("SPD"        , m_monsterInfoList[m_iCurPetInfoIdx - 1]["SPD"]);
            dicPetInfo.Add("DEF"        , m_monsterInfoList[m_iCurPetInfoIdx - 1]["DEF"]);
            dicPetInfo.Add("LEVEL"      , 1);
            dicPetInfo.Add("CURHP"      , m_monsterInfoList[m_iCurPetInfoIdx - 1]["HP"]);
            dicPetInfo.Add("NEXTEXP"    , 4);
            dicPetInfo.Add("CUREXP"     , 0);
            dicPetInfo.Add("FOLLOW"     , 0);

            List<Dictionary<string, object>> petInfo = CSVManager.instance.m_dicData[LOADTYPE.MYPETINFO].recordDataList;

            petInfo.Add(dicPetInfo);

            CSVManager.instance.SaveFile(LOADTYPE.MYPETINFO, petInfo);

            //�ֽ�ȭ
            RefreshPet(m_iCurPetInfoIdx);

            //�������� â�� ��������.
            UIManager.instance.OnDisableAlarmUI();
            UIManager.instance.ClearAlarmButtonListener();
        }
    }

    void NoButtonClick()
    {
        UIManager.instance.OnDisableAlarmUI();
        UIManager.instance.ClearAlarmButtonListener();
    }

    void ActiveFollowButtonClick()
    {
        //��Ƽ�갡 Ȱ��ȭ�� ��ư�� Ŭ���ص� �ƹ��� ������ ������Ѵ�.
    }

    void InactiveFollowButtonClick()
    {
        //��Ȱ��ȭ�� ��ư�� Ȱ��ȭ�ϸ� �������� ���� �����Ѵ�.

        m_btInactivePet.gameObject.SetActive(false);
        m_btActivePet.gameObject.SetActive(true);

        List<Dictionary<string, object>> myPetList = CSVManager.instance.m_dicData[LOADTYPE.MYPETINFO].recordDataList;

        for (int idx = 0; idx < myPetList.Count; idx++)
        {
            if (m_iCurPetInfoIdx == int.Parse(myPetList[idx]["INDEX"].ToString()))
            {
                myPetList[idx]["FOLLOW"] = 1;
            }

            else
            {
                myPetList[idx]["FOLLOW"] = 0;
            }

        }

        CSVManager.instance.SaveFile(LOADTYPE.MYPETINFO, myPetList);
    }
}
