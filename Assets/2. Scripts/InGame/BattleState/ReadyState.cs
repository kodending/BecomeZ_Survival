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
        //만약에 적이 모두 죽었거나
        bool bClearEnemy = CheckClearEnemy();
        //플레이어와 펫이 모두 죽었을 경우
        bool bClearPlayer = CheckClearPlayer();
        //배틀종료시켜야됨

        if (bClearEnemy || bClearPlayer)
        {
            if (bClearEnemy)
                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.WIN_RESULT);
            
            else if (bClearPlayer)
                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.DEFEAT_RESULT);

            return;
        }

        //Debug.Log("준비 단계로 왔다!");
        BattleSystem.bs.m_curSelectType = SELECTTYPE.PLAYER_PICK;

        //레디로 가야할 경우 조건을 확인해서
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);
        //넣어준다.
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

                //펫이 죽었을 경우 건너뛰고 배틀상태로 들어가야됨
                if(PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                {
                    UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE_ING);

                    BattleSystem.bs.m_curSelectType = SELECTTYPE.BATTLE_STATE;

                    OnBattleCal();

                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                }

                else
                {
                    //플레이어 공격이면 다음 펫공격이 되도록 UI를 다시 띄운다
                    UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);

                    //SelectType을 PET_PICK으로 바꿔야한다.
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
                    //여기서 공격자정보 저장
                    //선택된놈이니께 쉐이더 적용
                    //Debug.Log("걸렸따 요놈");

                    //죽었으면 선택 안되게끔 해야됨
                    if (hit.collider.GetComponent<EnemyFSM>().m_eCurAction == ENEMYSTATE.BATTLE_DEATH)
                        return;

                    hit.collider.GetComponent<EnemyFSM>().OnSelectedEnemy();

                    SELECTINFO sSelectInfo;

                    sSelectInfo.goSelector  = i_goSelector;
                    sSelectInfo.eSelectType = i_curSelect;
                    sSelectInfo.goSelected  = hit.collider.gameObject;
                    sSelectInfo.iSpeed      = hit.collider.GetComponent<EnemyFSM>().m_sEnemyInfo.spd;

                    //선택했으니 다음장으로 넘겨야한다.
                    //선택 대상이 죽었는지 확인 해야됨 (아직 안만들었음) 죽었으면 선택 안되게끔 해야됨
                    if (i_curSelect == SELECTTYPE.PLAYER_ATTACK)
                    {
                        //SelectType을 PET_PICK으로 바꿔야한다.
                        sSelectInfo.eSelector = BATTLEENTRY.PLAYER;
                        sSelectInfo.eSelected = BATTLEENTRY.ENEMY;

                        BattleSystem.bs.listSelectInfo.Add(sSelectInfo);

                        //펫이 죽었을 경우 건너뛰고 배틀상태로 들어가야됨
                        if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                        {
                            UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE_ING);

                            BattleSystem.bs.m_curSelectType = SELECTTYPE.BATTLE_STATE;

                            OnBattleCal();

                            BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                        }

                        else
                        {
                            //플레이어 공격이면 다음 펫공격이 되도록 UI를 다시 띄운다
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

    //배틀 상태 들어가기전에 배틀 최종 계산하는거
    void OnBattleCal()
    {
        //몬스터들 공격할지 방어할지 선택해서 넣고
        EnemyBattleSelect();
        //스피드값에따라 공격순서를 정한다.
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


                //선택 대상이 죽었는지 확인 해야됨

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

            //에너미 공격대상자가 누구인지 알아야됨
            sSelectInfo.goSelector.GetComponent<EnemyFSM>().m_eCurTargetEntry = sSelectInfo.eSelected;

            BattleSystem.bs.listSelectInfo.Add(sSelectInfo);
        }

        //에너미 선택까지 끝나면 라운드를 설정함
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
        //defense인 친구들을 먼저 확인하고

        //그리고 스피드가 높은 순서대로 정리해야됨

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
