using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BattleState : BattleBaseState
{
    [Tooltip("컨트롤할 오브젝트")]
    GameObject m_goControl;

    [Tooltip("이동할 목표지점")]
    Transform m_trTarget;

    int iCurRound;

    SELECTINFO m_sCurInfo;

    public override void OnEnterState()
    {
        if(BattleSystem.bs.m_iCurBattleRound == BattleSystem.bs.m_iFinalBattleRound)
        {
            //다시 준비 상태로 돌아간다
            //이때 리스트도 클리어 해주어야 한다.
            BattleSystem.bs.listSelectInfo.Clear();
            BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.READY);
            return;
        }

        iCurRound = BattleSystem.bs.m_iCurBattleRound;

        ////test
        ////공격 순서 설정해야됨 //이미 설정되어있고 펫, 에너미 공격 로직 만들어야됨
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
                //본인이 죽었을 경우 넘겨야됨
                if (PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
                {
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                }

                //선택한 에너미가 죽었을 경우 변경해야됨
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

                    //전원 죽었을 경우 걍 넘어가도록 해야됨
                    if(DeathCnt == enemyNum)
                    {
                        BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                        break;
                    }
                }

                //안죽었을 경우 선택 그대로
                else
                {
                    PlayerController.pc.m_fsmEnemy = m_sCurInfo.goSelected.GetComponent<EnemyFSM>();
                }

                PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.BATTLE_NORMAL_ATTACK);
            break;

            case SELECTTYPE.PLAYER_DEFENSE:
                //본인이 죽었을 경우 넘겨야됨
                if (PlayerController.pc.m_eCurAction != PLAYERSTATE.NORMAL_DEAD)
                {
                    PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.BATTLE_NORMAL_DEFENSE);
                }

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
            break;

            case SELECTTYPE.PET_ATTACK:
                //본인이 죽었을 경우 넘겨야됨
                if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
                {
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                }

                    //선택한 에너미가 죽었을 경우 변경해야됨
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

                    //전원 죽었을 경우 걍 넘어가도록 해야됨
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
                //본인이 죽었을 경우 넘겨야됨
                if (PetController.pc.m_eCurAction != PETSTATE.BATTLE_DEATH)
                    PetController.pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_DEFENSE);

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                break;

            case SELECTTYPE.ENEMY_ATTACK:
                //본인이 죽었을 경우 넘겨야됨
                if (m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_eCurAction == ENEMYSTATE.BATTLE_DEATH)
                {
                    BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                    break;
                } 

                //선택한 플레이어나, 펫이 죽었을 경우 변경해야됨
                if (m_sCurInfo.eSelected == BATTLEENTRY.PLAYER &&
                   PlayerController.pc.m_eCurAction == PLAYERSTATE.NORMAL_DEAD)
                {
                    //플레이어가 죽었을 경우는 공격안하고 넘어간다.
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
                //본인이 죽었을 경우 넘겨야됨
                if (m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_eCurAction != ENEMYSTATE.BATTLE_DEATH)
                    m_sCurInfo.goSelector.GetComponent<EnemyFSM>().m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_DEFENSE);

                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                break;
        }
    }
}
