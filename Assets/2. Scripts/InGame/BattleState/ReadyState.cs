using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ReadyState : BattleBaseState
{
    RaycastHit hit;

    Collider curCollider;

    public override void OnEnterState()
    {
        //���࿡ ���� ��� �׾��ų�
        bool bClearEnemy = CheckClearEnemy();
        //�÷��̾�� ���� ��� �׾��� ���
        bool bClearPlayer = CheckClearPlayer();
        //��Ʋ������Ѿߵ�

        if (bClearEnemy || bClearPlayer)
        {
            if (bClearEnemy)
                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.WIN_RESULT);
            
            else if (bClearPlayer)
                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.DEFEAT_RESULT);

            return;
        }

        //Debug.Log("�غ� �ܰ�� �Դ�!");
        BattleSystem.bs.m_curSelectType = SELECTTYPE.PLAYER_PICK;

        //����� ������ ��� ������ Ȯ���ؼ�
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);
        //�־��ش�.
    }

    public override void OnUpdateState()
    {
        OnSelectAttack(BattleSystem.bs.m_curSelectType);
        OnSelectDefense(BattleSystem.bs.m_curSelectType);
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }

    void OnSelectAttack(SELECTTYPE i_curSelect)
    {
        switch (i_curSelect)
        {
            case SELECTTYPE.PLAYER_ATTACK:
                ClickEnemy(i_curSelect, PlayerController.pc.gameObject);
            return;

            case SELECTTYPE.PET_ATTACK:
                ClickEnemy(i_curSelect, PetController.pc.gameObject);
            return;
        }
    }

    void OnSelectDefense(SELECTTYPE i_curSelect)
    {
        SELECTINFO sSelectInfo;

        switch (i_curSelect)
        {
            case SELECTTYPE.PLAYER_DEFENSE:

                sSelectInfo.goSelector  = PlayerController.pc.gameObject;
                sSelectInfo.eSelectType = i_curSelect;
                sSelectInfo.goSelected  = null;
                sSelectInfo.iSpeed      = 0;
                sSelectInfo.eSelector = BATTLEENTRY.PLAYER;
                sSelectInfo.eSelected = BATTLEENTRY._NONE_;

                BattleSystem.bs.listSelectInfo.Add(sSelectInfo);

                //���� �׾��� ��� �ǳʶٰ� ��Ʋ���·� ���ߵ�
                if(PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                {
                    UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE_ING);

                    BattleSystem.bs.m_curSelectType = SELECTTYPE.BATTLE_STATE;

                    OnBattleCal();

                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                }

                else
                {
                    //�÷��̾� �����̸� ���� ������� �ǵ��� UI�� �ٽ� ����
                    UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);

                    //SelectType�� PET_PICK���� �ٲ���Ѵ�.
                    BattleSystem.bs.m_curSelectType = SELECTTYPE.PET_PICK;
                }

                return;
            case SELECTTYPE.PET_DEFENSE:

                sSelectInfo.goSelector  = PetController.pc.gameObject;
                sSelectInfo.eSelectType = i_curSelect;
                sSelectInfo.goSelected  = null;
                sSelectInfo.iSpeed      = 0;
                sSelectInfo.eSelector   = BATTLEENTRY.PET;
                sSelectInfo.eSelected   = BATTLEENTRY._NONE_;

                BattleSystem.bs.listSelectInfo.Add(sSelectInfo);

                UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE_ING);

                BattleSystem.bs.m_curSelectType = SELECTTYPE.BATTLE_STATE;

                OnBattleCal();

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                return;
        }
    }

    void ClickEnemy(SELECTTYPE i_curSelect, GameObject i_goSelector)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = BattleSystem.bs.m_cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                curCollider = hit.collider;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = BattleSystem.bs.m_cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") &&
                    curCollider == hit.collider)
                {
                    //���⼭ ���������� ����
                    //���õȳ��̴ϲ� ���̴� ����
                    //Debug.Log("�ɷȵ� ���");

                    //�׾����� ���� �ȵǰԲ� �ؾߵ�
                    if (hit.collider.GetComponent<EnemyFSM>().m_eCurAction == ENEMYSTATE.BATTLE_DEATH)
                        return;

                    hit.collider.GetComponent<EnemyFSM>().OnSelectedEnemy();

                    SELECTINFO sSelectInfo;

                    sSelectInfo.goSelector  = i_goSelector;
                    sSelectInfo.eSelectType = i_curSelect;
                    sSelectInfo.goSelected  = hit.collider.gameObject;
                    sSelectInfo.iSpeed      = hit.collider.GetComponent<EnemyFSM>().m_sEnemyInfo.spd;

                    //���������� ���������� �Ѱܾ��Ѵ�.
                    //���� ����� �׾����� Ȯ�� �ؾߵ� (���� �ȸ������) �׾����� ���� �ȵǰԲ� �ؾߵ�
                    if (i_curSelect == SELECTTYPE.PLAYER_ATTACK)
                    {
                        //SelectType�� PET_PICK���� �ٲ���Ѵ�.
                        sSelectInfo.eSelector = BATTLEENTRY.PLAYER;
                        sSelectInfo.eSelected = BATTLEENTRY.ENEMY;

                        BattleSystem.bs.listSelectInfo.Add(sSelectInfo);

                        //���� �׾��� ��� �ǳʶٰ� ��Ʋ���·� ���ߵ�
                        if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                        {
                            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE_ING);

                            BattleSystem.bs.m_curSelectType = SELECTTYPE.BATTLE_STATE;

                            OnBattleCal();

                            BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                        }

                        else
                        {
                            //�÷��̾� �����̸� ���� ������� �ǵ��� UI�� �ٽ� ����
                            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);
                            BattleSystem.bs.m_curSelectType = SELECTTYPE.PET_PICK;
                        }
                    }
                    
                    else
                    {
                        sSelectInfo.eSelector = BATTLEENTRY.PET;
                        sSelectInfo.eSelected = BATTLEENTRY.ENEMY;

                        BattleSystem.bs.listSelectInfo.Add(sSelectInfo);

                        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE_ING);
                        
                        BattleSystem.bs.m_curSelectType = SELECTTYPE.BATTLE_STATE;

                        OnBattleCal();

                        BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    }
                }
            }

            if (hit.collider != curCollider)
            {
                curCollider = null;
            }
        }
    }

    //��Ʋ ���� �������� ��Ʋ ���� ����ϴ°�
    void OnBattleCal()
    {
        //���͵� �������� ������� �����ؼ� �ְ�
        EnemyBattleSelect();
        //���ǵ尪������ ���ݼ����� ���Ѵ�.
        SortBattleOrder();
    }

    void EnemyBattleSelect()
    {
        int enemyNum = BattleSystem.bs.listEnemy.Count;
        for (int idx = 0; idx < enemyNum; idx++)
        {
            SELECTINFO sSelectInfo = new SELECTINFO();

            sSelectInfo.goSelector = BattleSystem.bs.listEnemy[idx];
            sSelectInfo.eSelectType = EnemySelectType();

            if (sSelectInfo.eSelectType == SELECTTYPE.ENEMY_ATTACK)
            {
                int randNum = Random.Range(0, 2);
                //int randNum = 1;
                sSelectInfo.goSelected = EnemySelectTarget(randNum);
                sSelectInfo.iSpeed = sSelectInfo.goSelector.GetComponent<EnemyFSM>().m_sEnemyInfo.spd;
                sSelectInfo.eSelector = BATTLEENTRY.ENEMY;


                //���� ����� �׾����� Ȯ�� �ؾߵ�

                if (randNum == 0)
                {
                    if(PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
                        sSelectInfo.eSelected = BATTLEENTRY.PET;
                    else
                        sSelectInfo.eSelected = BATTLEENTRY.PLAYER;
                }

                else
                {
                    if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                        sSelectInfo.eSelected = BATTLEENTRY.PLAYER;
                    else
                        sSelectInfo.eSelected = BATTLEENTRY.PET;
                }
            }
            else
            {
                sSelectInfo.goSelected = null;
                sSelectInfo.iSpeed = 0;
                sSelectInfo.eSelector = BATTLEENTRY.ENEMY;
                sSelectInfo.eSelected = BATTLEENTRY._NONE_;
            }

            //���ʹ� ���ݴ���ڰ� �������� �˾ƾߵ�
            sSelectInfo.goSelector.GetComponent<EnemyFSM>().m_eCurTargetEntry = sSelectInfo.eSelected;

            BattleSystem.bs.listSelectInfo.Add(sSelectInfo);
        }

        //���ʹ� ���ñ��� ������ ���带 ������
        BattleSystem.bs.m_iCurBattleRound = 0;
        BattleSystem.bs.m_iFinalBattleRound = BattleSystem.bs.listSelectInfo.Count;
    }

    SELECTTYPE EnemySelectType()
    {
        SELECTTYPE selectType = new SELECTTYPE();

        int randNum = Random.Range((int)SELECTTYPE.ENEMY_ATTACK, (int)SELECTTYPE.ENEMY_DEFENSE + 1);

        selectType = (SELECTTYPE)randNum;

        return selectType;
    }

    GameObject EnemySelectTarget(int i_randNum)
    {
        GameObject go = new GameObject();

        if (i_randNum == 0)
            if(PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
                go = PetController.pc.gameObject;
            else
                go = PlayerController.pc.gameObject;

        else
            if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                go = PlayerController.pc.gameObject;
            else
                go = PetController.pc.gameObject;

        return go;
    }

    void SortBattleOrder()
    {
        BattleSystem.bs.listSelectInfo.Sort(SpeedCompare);
    }

    int SpeedCompare(SELECTINFO a_info, SELECTINFO b_info)
    {
        //defense�� ģ������ ���� Ȯ���ϰ�

        //�׸��� ���ǵ尡 ���� ������� �����ؾߵ�

        if (a_info.iSpeed == 0) return -1;
        if (b_info.iSpeed == 0) return 1;

        return a_info.iSpeed > b_info.iSpeed ? -1 : 1;
    }

    bool CheckClearEnemy()
    {
        bool isClear = false;
        int iDeadCnt = 0;

        int enemyNum = BattleSystem.bs.listEnemy.Count;
        for (int idx = 0; idx < enemyNum; idx++)
        {
            if (BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>().m_eCurAction
                == ENEMYSTATE.BATTLE_DEATH)
            {
                iDeadCnt++;
            }
        }

        if (iDeadCnt == enemyNum)
            isClear = true;

        return isClear;
    }

    bool CheckClearPlayer()
    {
        bool isClear = false;

        if (PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
            isClear = true;

        return isClear;
    }
}
