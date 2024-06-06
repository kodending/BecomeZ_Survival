using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Death_State : PlayerBaseState
{
    PlayerController m_pc;

    public Battle_Death_State(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        //현재 플레이어 상태 지정
        m_pc.m_eCurAction = PLAYERSTATE.BATTLE_NORMAL_DEAD;
        m_pc.m_curAnim.SetTrigger("Common_Death");
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
