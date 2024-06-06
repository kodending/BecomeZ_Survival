using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Hit_Enemy : EnemyBaseState
{
    EnemyFSM m_ef;

    //���� ���潺 �ɷ�ġ �����س���
    int m_iDef;

    public Battle_Hit_Enemy(EnemyFSM ef) : base(ef)
    {
        m_ef = ef;
    }

    public override void OnEnterState()
    {
        if(m_ef.m_eCurAction == ENEMYSTATE.BATTLE_DEFENSE)
        {
            //���� ��Ƣ�� ��Ű��
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
        //���� �������� �ǵ�����
        m_ef.m_sEnemyInfo.def = m_iDef;
    }

    IEnumerator HitToIdle()
    {
        yield return new WaitForSeconds(1f);

        //ü���� ������ ���
        if (m_ef.m_sEnemyInfo.curHP <= 0)
            m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_DEATH);

        //�����¿����� Defense
        else if (m_ef.m_eCurAction == ENEMYSTATE.BATTLE_DEFENSE)
            m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_DEFENSE);

        //�� �� ���ĵ����
        else
        {
            m_ef.m_curAnim.SetTrigger("Idle");
            m_ef.m_stateMachine.ChangeEnemyState(ENEMYSTATE.BATTLE_STAND);
        }
    }
}
