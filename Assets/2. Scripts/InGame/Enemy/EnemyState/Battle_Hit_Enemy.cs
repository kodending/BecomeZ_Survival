using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Hit_Enemy : EnemyBaseState
{
    EnemyFSM m_ef;

    //원래 디펜스 능력치 저장해놓기
    int m_iDef;

    public Battle_Hit_Enemy(EnemyFSM ef) : base(ef)
    {
        m_ef = ef;
    }

    public override void OnEnterState()
    {
        if(m_ef.m_eCurAction == ENEMYSTATE.BATTLE_DEFENSE)
        {
            //방어스탯 뻥튀기 시키기
            m_iDef = m_ef.m_sEnemyInfo.def;
            m_ef.m_sEnemyInfo.def = (int)(m_ef.m_sEnemyInfo.def * 1.5f);
        }

        m_ef.m_curAnim.SetTrigger("Hit");

        m_ef.StartCoroutine(HitToIdle());
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        //원래 방어력으로 되돌리기
        m_ef.m_sEnemyInfo.def = m_iDef;
    }

    IEnumerator HitToIdle()
    {
        yield return new WaitForSeconds(1f);

        //체력이 없으면 사망
        if (m_ef.m_sEnemyInfo.curHP <= 0)
            m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_DEATH);

        //방어상태였으면 Defense
        else if (m_ef.m_eCurAction == ENEMYSTATE.BATTLE_DEFENSE)
            m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_DEFENSE);

        //그 외 스탠드상태
        else
        {
            m_ef.m_curAnim.SetTrigger("Idle");
            m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_STAND);
        }
    }
}
