using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Battle_Attack_Pet : PetBaseState
{
    PetController m_pc;

    Transform AttackPoint;

    float fTime = 0;

    public enum ATTACKMOTION
    {
        FORWARD,
        RETURN,
        IDLE,
        ATTACK,
        END,
        NONE,
    }

    ATTACKMOTION m_eCurAttackMotion;

    public Battle_Attack_Pet(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PETSTATE.BATTLE_ATTACK;
        m_pc.m_petAnim.SetTrigger("Running");
        AttackPoint = m_pc.m_fsmEnemy.m_trAttackPoint;
        m_eCurAttackMotion = ATTACKMOTION.FORWARD;
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {
        OnMotion();
    }

    public override void OnExitState()
    {

    }

    void OnMotion()
    {
        switch (m_eCurAttackMotion)
        {
            case ATTACKMOTION.FORWARD:

                Vector3 vLookDir = m_pc.m_fsmEnemy.transform.position - m_pc.transform.position;
                m_pc.transform.position = Vector3.MoveTowards(m_pc.transform.position, AttackPoint.position, 0.1f);
                m_pc.transform.rotation = Quaternion.Lerp(m_pc.transform.rotation, Quaternion.LookRotation(vLookDir), Time.deltaTime * 10f);

                if (Vector3.Distance(m_pc.transform.position, AttackPoint.position) < 0.1f)
                {
                    m_pc.m_petAnim.SetTrigger("Attacking");
                    m_eCurAttackMotion = ATTACKMOTION.ATTACK;
                }

                return;

            case ATTACKMOTION.RETURN:
                Vector3 vReturnLookDir = m_pc.m_vReturn - m_pc.transform.position;
                m_pc.transform.position = Vector3.Lerp(m_pc.transform.position, m_pc.m_vReturn, Time.deltaTime * 1.5f);
                m_pc.transform.rotation = Quaternion.Lerp(m_pc.transform.rotation, Quaternion.LookRotation(vReturnLookDir), Time.deltaTime * 10f);

                if (Vector3.Distance(m_pc.transform.position, m_pc.m_vReturn) < 0.5f)
                {
                    m_pc.m_petAnim.SetTrigger("Idle");
                    m_eCurAttackMotion = ATTACKMOTION.IDLE;
                }
                return;

            case ATTACKMOTION.IDLE:
                m_pc.transform.rotation = Quaternion.Lerp(m_pc.transform.rotation, m_pc.m_rotReturn, Time.deltaTime * 10f);

                fTime += Time.deltaTime;

                if (fTime >= 1.0f)
                {
                    m_eCurAttackMotion = ATTACKMOTION.END;
                }

                return;

            case ATTACKMOTION.ATTACK:
                m_pc.StartCoroutine(ATTACKCAL());
                m_eCurAttackMotion = ATTACKMOTION.NONE;
                return;

            case ATTACKMOTION.END:
                //엔드로 돌아오고
                //배틀 다음으로 넘긴다.
                fTime = 0;
                m_pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);
                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                return;

            case ATTACKMOTION.NONE:

                return;
        }
    }

    IEnumerator ATTACKCAL()
    {
        bool isDefense = false;
        if (m_pc.m_fsmEnemy.m_eCurAction == ENEMYSTATE.BATTLE_DEFENSE)
            isDefense = true;

        int iDamage = AttackCal.ac.DamageCalculate(m_pc.m_sPetInfo.atk,
                                                   m_pc.m_fsmEnemy.m_sEnemyInfo.def,
                                                   isDefense);

        m_pc.m_fsmEnemy.m_sEnemyInfo.curHP -= iDamage;

        yield return new WaitForSeconds(1f);

        //UI를 띄워줘야됨(만들어야됨)
        DamageTextManager.dtm.SpawnText(m_pc.m_fsmEnemy.transform.position + new Vector3(0, 1f, 0),
                                        iDamage.ToString());

        m_pc.m_fsmEnemy.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_HIT);

        yield return new WaitForSeconds(2f);

        m_pc.m_petAnim.SetTrigger("Running");
        m_eCurAttackMotion = ATTACKMOTION.RETURN;
        m_pc.m_fsmEnemy.UnSelected();
    }
}
