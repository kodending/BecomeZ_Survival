using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UISTATE
{
    MAIN,
    INGAME_NORMAL,
    INGAME_BATTLE,
    INGAME_BATTLE_ING,
    INGAME_BATTLE_RESULT,
    INGAME_BATTLE_END,
    INGAME_MAZE,
    INGAME_COSTUME,
    INGAME_PET,
    INGAME_INVEN,
    _MAX_
}


public abstract class UIBaseState : MonoBehaviour
{
    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

public class UIStateMachine
{
    public UIBaseState CurrentUIState { get; private set; }
    private Dictionary<UISTATE, UIBaseState> dicUIStates =
                                                     new Dictionary<UISTATE, UIBaseState>();

    public UIStateMachine(UISTATE i_eState, UIBaseState i_sUI)
    {
        AddState(i_eState, i_sUI);
        CurrentUIState = GetUIState(i_eState);
    }

    public void AddState(UISTATE i_eState, UIBaseState i_sUI)
    {
        if (!dicUIStates.ContainsKey(i_eState))
        {
            dicUIStates.Add(i_eState, i_sUI);
        }
    }

    public UIBaseState GetUIState(UISTATE i_eState)
    {
        if (dicUIStates.TryGetValue(i_eState, out UIBaseState state))
        {
            return state;
        }

        return null;
    }

    public void DeleteUIState(UISTATE i_eUIState)
    {
        if (dicUIStates.ContainsKey(i_eUIState))
        {      
            dicUIStates.Remove(i_eUIState);
        }
    }

    public void ChangeUIState(UISTATE i_eState)
    {
        CurrentUIState?.OnExitState(); //���� ���¸� �����ϴ� �޼ҵ带 �����ϰ�
        if (dicUIStates.TryGetValue(i_eState, out UIBaseState state))
        {
            CurrentUIState = state;
        }

        CurrentUIState?.OnEnterState(); //���� ���� ���� �޼��� ����
    }

    public void UpdateState()
    {
        CurrentUIState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentUIState?.OnFixedUpdateState();
    }
}

public class UIManager : MonoBehaviour
{
    static GameObject go;

    public static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                go = new GameObject();
                go.name = "UIManager";
                instance = go.AddComponent<UIManager>() as UIManager;

                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }

    //���¸ӽ� ��������
    [HideInInspector]
    public UIStateMachine m_stateMachine { get; private set; }

    [HideInInspector]
    public GameObject m_goCurCanvas;

    [HideInInspector]
    [Tooltip("��Ʋ�� ���ȴ��� Ȯ���ϴ� ��")]
    public bool m_bBattleStart;

    [HideInInspector]
    [Tooltip("�÷��̾� HP HUD")]
    public Slider m_sPlayerHPBar;
    
    [HideInInspector]
    [Tooltip("�� HP HUD")]
    public Slider m_sPetHPBar;

    [HideInInspector]
    [Tooltip("���� �˶� â UI")]
    public GameObject m_goAlarmUI;

    [HideInInspector]
    [Tooltip("�ý��� â UI")]
    public GameObject m_goSystemUI;

    [HideInInspector]
    [Tooltip("�� �� HUD")]
    public GameObject m_goMoneyHUD;

    [HideInInspector]
    public Button m_btSetting;

    [HideInInspector]
    public Button m_btInven;

    [HideInInspector]
    [Tooltip("�κ� HUD")]
    public GameObject m_goInvenHUD;

    public struct HPTEXT
    {
        public Text txtMaxHP;
        public Text txtCurHP;
    }

    HPTEXT m_PlayerHPText;
    HPTEXT m_PetHPText;

    [HideInInspector]
    public GameObject m_goRandomCardHUD;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitStateMachine();

        m_goCurCanvas = GameObject.Find("Canvas");
        m_sPlayerHPBar = m_goCurCanvas.transform.Find("PlayerHP").GetComponent<Slider>();
        m_sPetHPBar = m_goCurCanvas.transform.Find("PetHP").GetComponent<Slider>();

        m_PlayerHPText.txtCurHP = m_sPlayerHPBar.transform.Find("CurHPText").GetComponent<Text>();
        m_PlayerHPText.txtMaxHP = m_sPlayerHPBar.transform.Find("MaxHPText").GetComponent<Text>();
        m_PetHPText.txtCurHP = m_sPetHPBar.transform.Find("CurHPText").GetComponent<Text>();
        m_PetHPText.txtMaxHP = m_sPetHPBar.transform.Find("MaxHPText").GetComponent<Text>();

        m_goAlarmUI = m_goCurCanvas.transform.Find("AlarmWindow").gameObject;
        m_goSystemUI = m_goCurCanvas.transform.Find("SystemMessage").gameObject;
        m_goMoneyHUD = m_goCurCanvas.transform.Find("MyMoneyHud").gameObject;

        m_btSetting = m_goCurCanvas.transform.Find("SettingButton").GetComponent<Button>();
        m_btSetting.onClick.AddListener(SettingButtonClick);
        m_btInven = m_goCurCanvas.transform.Find("InvenButton").GetComponent<Button>();
        m_btInven.onClick.AddListener(InvenButtonClick);

        m_goInvenHUD = m_goCurCanvas.transform.Find("InventoryInField").gameObject;

        m_goRandomCardHUD = m_goCurCanvas.transform.Find("RandomCardSelect").gameObject;

        RefreshMyMoneyHud();
    }

    private void Update()
    {
        m_stateMachine.UpdateState();

        RefreshHpHud();
    }

    private void FixedUpdate()
    {
        m_stateMachine.FixedUpdateState();
    }

    public void InitStateMachine()
    {
        m_stateMachine = new UIStateMachine(UISTATE.INGAME_NORMAL, gameObject.AddComponent<UI_INGAME_NORMAL>());
        m_stateMachine.AddState(UISTATE.INGAME_BATTLE, gameObject.AddComponent<UI_INGAME_BATTLE>());
        m_stateMachine.AddState(UISTATE.INGAME_BATTLE_ING, gameObject.AddComponent<UI_INGAME_BATTLE_ING>());
        m_stateMachine.AddState(UISTATE.INGAME_BATTLE_RESULT, gameObject.AddComponent<UI_INGAME_BATTLE_RESULT>());
        m_stateMachine.AddState(UISTATE.INGAME_MAZE, gameObject.AddComponent<UI_INGAME_MAZE>());
        m_stateMachine.AddState(UISTATE.INGAME_COSTUME, gameObject.AddComponent<UI_INGAME_COSTUME>());
        m_stateMachine.AddState(UISTATE.INGAME_PET, gameObject.AddComponent<UI_INGAME_PET>());
        m_stateMachine.AddState(UISTATE.INGAME_INVEN, gameObject.AddComponent<UI_INGAME_INVEN>());

        m_stateMachine.CurrentUIState?.OnEnterState();
    }

    public IEnumerator BattleTextOn()
    {
        GameObject go;
        go = m_goCurCanvas.transform.Find("BattleText").gameObject;
        go.SetActive(true);

        yield return new WaitForSeconds(3f);

        go.SetActive(false);
    }

    public void RefreshHpHud()
    {
        //�÷��̾� HP HUD
        m_sPlayerHPBar.value = HPCal(m_sPlayerHPBar.value, PlayerController.pc.m_sPlayerInfo.curHp,
                                    PlayerController.pc.m_sPlayerInfo.hp);
        m_PlayerHPText.txtCurHP.text = PlayerController.pc.m_sPlayerInfo.curHp.ToString();
        m_PlayerHPText.txtMaxHP.text = PlayerController.pc.m_sPlayerInfo.hp.ToString();


        //�� HP HUD
        m_sPetHPBar.value = HPCal(m_sPetHPBar.value, PetController.pc.m_sPetInfo.curHp,
                                  PetController.pc.m_sPetInfo.hp);
        m_PetHPText.txtCurHP.text = PetController.pc.m_sPetInfo.curHp.ToString();
        m_PetHPText.txtMaxHP.text = PetController.pc.m_sPetInfo.hp.ToString();
    }

    private float HPCal(float fValue, float fCurHP, float fMaxHP)
    {
        float value = 0;

        value = Mathf.Lerp(fValue, fCurHP / fMaxHP, Time.deltaTime * 10);

        return value;
    }

    public void ActvieHPHud(bool isActive)
    {
        //�÷��̾� HPBar On
        m_sPetHPBar.gameObject.SetActive(isActive);
        m_sPlayerHPBar.gameObject.SetActive(isActive);
    }

    public void ActiveAlarmUI(string i_strTitle, bool i_bTimeSleep)
    {
        if (i_bTimeSleep) Time.timeScale = 0;

        m_goAlarmUI.SetActive(true);

        Text titleTxt = m_goAlarmUI.transform.Find("TitleText").GetComponent<Text>();

        titleTxt.text = i_strTitle;
    }

    public void OnDisableAlarmUI()
    {
        if (Time.timeScale == 0) Time.timeScale = 1f;

        m_goAlarmUI.SetActive(false);
    }

    public void ConnectAlarmButtonListener(UnityEngine.Events.UnityAction i_YBtnAction, UnityEngine.Events.UnityAction i_XBtnAction)
    {
        Button YButton = m_goAlarmUI.transform.Find("YesButton").GetComponent<Button>();
        Button XButton = m_goAlarmUI.transform.Find("NoButton").GetComponent<Button>();

        YButton.onClick.AddListener(i_YBtnAction);
        XButton.onClick.AddListener(i_XBtnAction);
    }

    public void ClearAlarmButtonListener()
    {
        Button YButton = m_goAlarmUI.transform.Find("YesButton").GetComponent<Button>();
        Button XButton = m_goAlarmUI.transform.Find("NoButton").GetComponent<Button>();

        YButton.onClick.RemoveAllListeners();
        XButton.onClick.RemoveAllListeners();
    }

    public IEnumerator ActiveSystemMsg(string i_strTitle, float i_fWaitTime)
    {
        m_goSystemUI.SetActive(true);

        Text titleTxt = m_goSystemUI.transform.Find("TitleText").GetComponent<Text>();

        titleTxt.text = i_strTitle;

        yield return new WaitForSeconds(i_fWaitTime);

        m_goSystemUI.SetActive(false);
    }
    
    public void RefreshMyMoneyHud()
    {
        Text moneyTxt = m_goMoneyHUD.transform.Find("MoneyText").GetComponent<Text>();

        moneyTxt.text = PlayerController.pc.m_sPlayerInfo.myMoney.ToString();
    }

    void InvenButtonClick()
    {
        m_stateMachine.ChangeUIState(UISTATE.INGAME_INVEN);
    }

    void SettingButtonClick()
    {

    }

    public void ActiveFieldHUD(bool i_bActive)
    {
        m_btSetting.gameObject.SetActive(i_bActive);
        m_btInven.gameObject.SetActive(i_bActive);
    }

    public void ActiveRandomItem(bool i_isActive)
    {
        m_goRandomCardHUD.SetActive(i_isActive);

        if (i_isActive)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }
}
