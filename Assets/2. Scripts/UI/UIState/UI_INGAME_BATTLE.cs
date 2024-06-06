using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SELECTORDER
{
    PLAYER,
    PET
}

public class UI_INGAME_BATTLE : UIBaseState
{
    GameObject m_goTxt;

    [Tooltip("공격, 방어, 아이템 사용 UI 패널 go")]
    GameObject m_goSelectPanel;

    [Tooltip("플레이어, 펫 행동결정 텍스트 go")]
    GameObject m_goSelectText;

    [Tooltip("에너미 선택 텍스트 go")]
    GameObject m_goEnemySelectText;


    public override void OnEnterState()
    {
        UIManager.instance.m_goCurCanvas = GameObject.Find("Canvas");
        m_goTxt = UIManager.instance.m_goCurCanvas.transform.Find("BattleText").gameObject;

        UIManager.instance.ActvieHPHud(true);

        StartCoroutine(OnBattleSelectText(m_goTxt));
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        if(m_goEnemySelectText != null)
            m_goEnemySelectText.SetActive(false);
    }

    IEnumerator OnBattleSelectText(GameObject go)
    {
        yield return new WaitForSeconds(1f);

        m_goSelectPanel = UIManager.instance.m_goCurCanvas.transform.Find("SelectPanel").gameObject;
        m_goSelectPanel.SetActive(true);
        OnInitBtn(m_goSelectPanel);

        m_goSelectText = UIManager.instance.m_goCurCanvas.transform.Find("SelectText").gameObject;
        m_goSelectText.SetActive(true);

        if(BattleSystem.bs.m_curSelectType != SELECTTYPE.PET_PICK)
            m_goSelectText.GetComponent<Text>().text = "플레이어의 행동을 선택하세요";
        else
            m_goSelectText.GetComponent<Text>().text = "펫의 행동을 선택하세요";

        m_goEnemySelectText = UIManager.instance.m_goCurCanvas.transform.Find("EnemySelectText").gameObject;
    }

    private void OnInitBtn(GameObject i_goSelect)
    {
        //버튼 세팅
        i_goSelect.transform.Find("AttackBtn").GetComponent<Button>().onClick.AddListener(OnAttackBtn);
        i_goSelect.transform.Find("DefenseBtn").GetComponent<Button>().onClick.AddListener(OnDefenseBtn);
        i_goSelect.transform.Find("ItemBtn").GetComponent<Button>().onClick.AddListener(OnItemBtn);
    }

    private void OnAttackBtn()
    {
        //공격을 지정할 몬스터를 선택하도록 신호를 넘겨야함
        switch(BattleSystem.bs.m_curSelectType)
        {
            case SELECTTYPE.PLAYER_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PLAYER_ATTACK);
            break;

            case SELECTTYPE.PET_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PET_ATTACK);
            break;
        }

        //Select UI를 꺼야함
        m_goSelectPanel.SetActive(false);
        //SelectText UI 끄자
        m_goSelectText.SetActive(false);
        //에너미 선택하라는 UI 키자
        m_goEnemySelectText.SetActive(true);
    }

    private void OnDefenseBtn()
    {
        //공격을 지정할 몬스터를 선택하도록 신호를 넘겨야함
        switch (BattleSystem.bs.m_curSelectType)
        {
            case SELECTTYPE.PLAYER_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PLAYER_DEFENSE);
                break;

            case SELECTTYPE.PET_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PET_DEFENSE);
                break;
        }

        //Select UI를 꺼야함
        m_goSelectPanel.SetActive(false);
        //SelectText UI 끄자
        m_goSelectText.SetActive(false);
    }

    private void OnItemBtn()
    {

    }
}
