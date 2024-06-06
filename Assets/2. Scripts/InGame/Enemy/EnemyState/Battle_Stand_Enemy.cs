using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Stand_Enemy : EnemyBaseState
{
    EnemyFSM m_ef;

    public Battle_Stand_Enemy(EnemyFSM ef) : base(ef)
    {
        m_ef = ef;
    }

    public override void OnEnterState()
    {
        m_ef.m_eCurAction = ENEMYSTATE.BATTLE_STAND;
        //m_ef.m_curAnim.SetTrigger("Idle");
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }
}
