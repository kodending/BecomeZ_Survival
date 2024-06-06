using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_RollState : PlayerBaseState
{
    PlayerController m_pc;

    public Normal_RollState(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PLAYERSTATE.NORMAL_ROLL;
        m_pc.m_curAnim.SetTrigger("Common_Roll");
    }

    public override void OnUpdateState()
    {
        
    }

    public override void OnFixedUpdateState()
    {
        if (m_pc.IsGround())
            m_pc.transform.Translate(Vector3.forward * m_pc.m_fMoveSpeed * 1.25f * Time.deltaTime);
    }

    public override void OnExitState()
    {

    }
}
