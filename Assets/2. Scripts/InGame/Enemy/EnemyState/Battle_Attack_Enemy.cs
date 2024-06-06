using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Battle_Attack_Enemy : EnemyBaseState
{
    EnemyFSM m_ef;

    Vector3 vAttackPoint;

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

    public Battle_Attack_Enemy(EnemyFSM ef) : base(ef)
    {
        m_ef = ef;
    }

    public override void OnEnterState()
    {
        m_ef.m_eCurAction = ENEMYSTATE.BATTLE_ATTACK;
        m_ef.m_curAnim.SetTrigger("Running");

        vAttackPoint = m_ef.m_goAttackTarget.transform.position + new Vector3(1f, 0, 1f);
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

                Vector3 vLookDir = m_ef.m_goAttackTarget.transform.position - m_ef.transform.position;
                m_ef.transform.position = Vector3.MoveTowards(m_ef.transform.position, vAttackPoint, 0.1f);
                m_ef.transform.rotation = Quaternion.Lerp(m_ef.transform.rotation, Quaternion.LookRotation(vLookDir), Time.deltaTime * 10f);

                if (Vector3.Distance(m_ef.transform.position, vAttackPoint) < 0.1f)
                {
                    m_ef.m_curAnim.SetTrigger("Attacking");
                    m_eCurAttackMotion = ATTACKMOTION.ATTACK;
                }

                return;

            case ATTACKMOTION.RETURN:
                Vector3 vReturnLookDir = m_ef.m_vReturn - m_ef.transform.position;
                m_ef.transform.position = Vector3.Lerp(m_ef.transform.position, m_ef.m_vReturn, Time.deltaTime * 1.5f);
                m_ef.transform.rotation = Quaternion.Lerp(m_ef.transform.rotation, Quaternion.LookRotation(vReturnLookDir), Time.deltaTime * 10f);

                if (Vector3.Distance(m_ef.transform.position, m_ef.m_vReturn) < 0.5f)
                {
                    m_ef.m_curAnim.SetTrigger("Idle");
                    m_eCurAttackMotion = ATTACKMOTION.IDLE;
                }
                return;

            case ATTACKMOTION.IDLE:
                m_ef.transform.rotation = Quaternion.Lerp(m_ef.transform.rotation, m_ef.m_rotReturn, Time.deltaTime * 10f);

                fTime += Time.deltaTime;

                if (fTime >= 1.0f)
                {
                    m_eCurAttackMotion = ATTACKMOTION.END;
                }

                return;

            case ATTACKMOTION.ATTACK:
                m_ef.StartCoroutine(ATTACKCAL());
                m_eCurAttackMotion = ATTACKMOTION.NONE;
                return;

            case ATTACKMOTION.END:
                //엔드로 돌아오고
                //배틀 다음으로 넘긴다.
                fTime = 0;
                m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_STAND);
                BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.BATTLE);
                return;

            case ATTACKMOTION.NONE:

                return;
        }
    }

    IEnumerator ATTACKCAL()
    {
        bool isDefense = false;

        //공격대상이 플레이어인지 펫인지 확인한다.
        if(m_ef.m_eCurTargetEntry == BATTLEENTRY.PLAYER)
        {
            if (PlayerController.pc.m_eCurAction == PLAYERSTATE.BATTLE_NORMAL_DEFENSE)
                isDefense = true;

            int iDamage = AttackCal.ac.DamageCalculate(m_ef.m_sEnemyInfo.atk,
                                           PlayerController.pc.m_sPlayerInfo.def,
                                           isDefense);

            PlayerController.pc.m_sPlayerInfo.curHp -= iDamage;

            //UI를 띄워줘야됨(만들어야됨)

            yield return new WaitForSeconds(1f);

            //UI를 띄워줘야됨(만들어야됨)
            DamageTextManager.dtm.SpawnText(PlayerController.pc.transform.position + new Vector3(0, 1f, 0),
                                            iDamage.ToString());

            //피격 모션취하게 해줘야됨
            PlayerController.pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.BATTLE_NORMAL_HIT);

            //공격대상자 HP가 0보다 작거나 같을 경우 죽음
            if (PlayerController.pc.m_sPlayerInfo.curHp <= 0)
            {
                //죽었다고 알려줘야됨
            }
        }

        else if(m_ef.m_eCurTargetEntry == BATTLEENTRY.PET)
        {
            if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEFENSE)
                isDefense = true;

            int iDamage = AttackCal.ac.DamageCalculate(m_ef.m_sEnemyInfo.atk,
                                           PetController.pc.m_sPetInfo.def,
                                           isDefense);

            PetController.pc.m_sPetInfo.curHp -= iDamage;

            //UI를 띄워줘야됨(만들어야됨)

            yield return new WaitForSeconds(1f);

            //UI를 띄워줘야됨(만들어야됨)
            DamageTextManager.dtm.SpawnText(PetController.pc.transform.position + new Vector3(0, 1f, 0),
                                            iDamage.ToString());

            //피격 모션취하게 해줘야됨
            PetController.pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_HIT);
        }

        yield return new WaitForSeconds(2f);

        m_ef.m_curAnim.SetTrigger("Running");
        m_eCurAttackMotion = ATTACKMOTION.RETURN;
    }
}
