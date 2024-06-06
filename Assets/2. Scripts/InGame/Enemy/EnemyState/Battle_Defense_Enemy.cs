using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Defense_Enemy : EnemyBaseState
{
    EnemyFSM m_ef;

    //원래 디펜스 능력치 저장해놓기
    int m_iDef;

    public Battle_Defense_Enemy(EnemyFSM ef) : base(ef)
    {
        m_ef = ef;
    }

    public override void OnEnterState()
    {
        m_ef.m_eCurAction = ENEMYSTATE.BATTLE_DEFENSE;
        m_ef.m_curAnim.SetTrigger("Idle");

        //방어스탯 뻥튀기 시키기
        m_iDef = m_ef.m_sEnemyInfo.def;
        m_ef.m_sEnemyInfo.def = (int)(m_ef.m_sEnemyInfo.def * 1.5f);
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        //끝나면 돌려놓기
        m_ef.m_sEnemyInfo.def = m_iDef;
    }
}
