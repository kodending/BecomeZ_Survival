using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BattleState : BattleBaseState
{
    [Tooltip("��Ʈ���� ������Ʈ")]
    GameObject m_goControl;

    [Tooltip("�̵��� ��ǥ����")]
    Transform m_trTarget;

    int iCurRound;

    SELECTINFO m_sCurInfo;

    public override void OnEnterState()
    {
        if(BattleSystem.bs.m_iCurBattleRound == BattleSystem.bs.m_iFinalBattleRound)
        {
            //�ٽ� �غ� ���·� ���ư���
            //�̶� ����Ʈ�� Ŭ���� ���־�� �Ѵ�.
            BattleSystem.bs.listSelectInfo.Clear();
            BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.READY);
            return;
        }

        iCurRound = BattleSystem.bs.m_iCurBattleRound;

        ////test
        ////���� ���� �����ؾߵ� //�̹� �����Ǿ��ְ� ��, ���ʹ� ���� ���� �����ߵ�
        //for (int idx = 0; idx < BattleSystem.bs.listSelectInfo.Count; idx++)
        //{
        //    if (BattleSystem.bs.listSelectInfo[idx].eSelector == BATTLEENTRY.PET)
        //    {
        //        iCurRound = idx;
        //    }
        //}

        m_sCurInfo = BattleSystem.bs.listSelectInfo[iCurRound];

        m_goControl = m_sCurInfo.goSelector;

        OnChangeState(m_sCurInfo.eSelectType);
    }

    public override void OnUpdateState()
    {
        
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        BattleSystem.bs.m_iCurBattleRound++;
    }

    void OnChangeState(SELECTTYPE i_sSelectType)
    {
        switch (i_sSelectType)
        {
            case SELECTTYPE.PLAYER_ATTACK:
                //������ �׾��� ��� �Ѱܾߵ�
                if (PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
                {
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                }

                //������ ���ʹ̰� �׾��� ��� �����ؾߵ�
                if(m_sCurInfo.goSelected.GetComponent<EnemyFSM>().m_eCurAction == ENEMYSTATE.BATTLE_DEATH)
                {
                    int enemyNum = BattleSystem.bs.listEnemy.Count;
                    int DeathCnt = 0;
                    for (int idx = 0; idx < enemyNum; idx++)
                    {
                        if (BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>().m_eCurAction
                            == ENEMYSTATE.BATTLE_DEATH)
                        {
                            DeathCnt++;
                            continue;
                        }

                        PlayerController.pc.m_fsmEnemy = BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>();
                        BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>().OnSelectedEnemy();
                        break;
                    }

                    //���� �׾��� ��� �� �Ѿ���� �ؾߵ�
                    if(DeathCnt == enemyNum)
                    {
                        BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                        break;
                    }
                }

                //���׾��� ��� ���� �״��
                else
                {
                    PlayerController.pc.m_fsmEnemy = m_sCurInfo.goSelected.GetComponent<EnemyFSM>();
                }

                PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.BATTLE_NORMAL_ATTACK);
            break;

            case SELECTTYPE.PLAYER_DEFENSE:
                //������ �׾��� ��� �Ѱܾߵ�
                if (PlayerController.pc.m_eCurAction != PLAYERSTATE.NORMAL_DEAD)
                {
                    PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.BATTLE_NORMAL_DEFENSE);
                }

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
            break;

            case SELECTTYPE.PET_ATTACK:
                //������ �׾��� ��� �Ѱܾߵ�
                if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                {
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                }

                    //������ ���ʹ̰� �׾��� ��� �����ؾߵ�
                if (m_sCurInfo.goSelected.GetComponent<EnemyFSM>().m_eCurAction == ENEMYSTATE.BATTLE_DEATH)
                {
                    int enemyNum = BattleSystem.bs.listEnemy.Count;
                    int DeathCnt = 0;
                    for (int idx = 0; idx < enemyNum; idx++)
                    {
                        if (BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>().m_eCurAction
                            == ENEMYSTATE.BATTLE_DEATH)
                        {
                            continue;
                        }

                        PetController.pc.m_fsmEnemy = BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>();
                        BattleSystem.bs.listEnemy[idx].GetComponent<EnemyFSM>().OnSelectedEnemy();
                        break;
                    }

                    //���� �׾��� ��� �� �Ѿ���� �ؾߵ�
                    if (DeathCnt == enemyNum)
                    {
                        BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                        break;
                    }
                }

                else
                {
                    PetController.pc.m_fsmEnemy = m_sCurInfo.goSelected.GetComponent<EnemyFSM>();
                }
                PetController.pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_ATTACK);
            break;

            case SELECTTYPE.PET_DEFENSE:
                //������ �׾��� ��� �Ѱܾߵ�
                if (PetController.pc.m_eCurAction != PETSTATE.BATTLE_DEATH)
                    PetController.pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_DEFENSE);

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                break;

            case SELECTTYPE.ENEMY_ATTACK:
                //������ �׾��� ��� �Ѱܾߵ�
                if (m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_eCurAction == ENEMYSTATE.BATTLE_DEATH)
                {
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                } 

                //������ �÷��̾, ���� �׾��� ��� �����ؾߵ�
                if (m_sCurInfo.eSelected == BATTLEENTRY.PLAYER &&
                   PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
                {
                    //�÷��̾ �׾��� ���� ���ݾ��ϰ� �Ѿ��.
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                }

                else if (m_sCurInfo.eSelected == BATTLEENTRY.PET &&
                    PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                    m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_goAttackTarget = PlayerController.pc.gameObject;

                else
                    m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_goAttackTarget = m_sCurInfo.goSelected;

                m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_ATTACK);
                break;

            case SELECTTYPE.ENEMY_DEFENSE:
                //������ �׾��� ��� �Ѱܾߵ�
                if (m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_eCurAction != ENEMYSTATE.BATTLE_DEATH)
                    m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_DEFENSE);

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                break;
        }
    }
}
