using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Death_Enemy : EnemyBaseState
{
    EnemyFSM m_ef;

    public Battle_Death_Enemy(EnemyFSM ef) : base(ef)
    {
        m_ef = ef;
    }

    public override void OnEnterState()
    {
        m_ef.m_eCurAction = ENEMYSTATE.BATTLE_DEATH;
        m_ef.ClearSelected();
        m_ef.m_curAnim.SetTrigger("Death");
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
