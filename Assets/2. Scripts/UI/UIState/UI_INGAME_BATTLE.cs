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

    [Tooltip("����, ���, ������ ��� UI �г� go")]
    GameObject m_goSelectPanel;

    [Tooltip("�÷��̾�, �� �ൿ���� �ؽ�Ʈ go")]
    GameObject m_goSelectText;

    [Tooltip("���ʹ� ���� �ؽ�Ʈ go")]
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
            m_goSelectText.GetComponent<Text>().text = "�÷��̾��� �ൿ�� �����ϼ���";
        else
            m_goSelectText.GetComponent<Text>().text = "���� �ൿ�� �����ϼ���";

        m_goEnemySelectText = UIManager.instance.m_goCurCanvas.transform.Find("EnemySelectText").gameObject;
    }

    private void OnInitBtn(GameObject i_goSelect)
    {
        //��ư ����
        i_goSelect.transform.Find("AttackBtn").GetComponent<Button>().onClick.AddListener(OnAttackBtn);
        i_goSelect.transform.Find("DefenseBtn").GetComponent<Button>().onClick.AddListener(OnDefenseBtn);
        i_goSelect.transform.Find("ItemBtn").GetComponent<Button>().onClick.AddListener(OnItemBtn);
    }

    private void OnAttackBtn()
    {
        //������ ������ ���͸� �����ϵ��� ��ȣ�� �Ѱܾ���
        switch(BattleSystem.bs.m_curSelectType)
        {
            case SELECTTYPE.PLAYER_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PLAYER_ATTACK);
            break;

            case SELECTTYPE.PET_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PET_ATTACK);
            break;
        }

        //Select UI�� ������
        m_goSelectPanel.SetActive(false);
        //SelectText UI ����
        m_goSelectText.SetActive(false);
        //���ʹ� �����϶�� UI Ű��
        m_goEnemySelectText.SetActive(true);
    }

    private void OnDefenseBtn()
    {
        //������ ������ ���͸� �����ϵ��� ��ȣ�� �Ѱܾ���
        switch (BattleSystem.bs.m_curSelectType)
        {
            case SELECTTYPE.PLAYER_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PLAYER_DEFENSE);
                break;

            case SELECTTYPE.PET_PICK:
                BattleSystem.bs.OnSelect(SELECTTYPE.PET_DEFENSE);
                break;
        }

        //Select UI�� ������
        m_goSelectPanel.SetActive(false);
        //SelectText UI ����
        m_goSelectText.SetActive(false);
    }

    private void OnItemBtn()
    {

    }
}
