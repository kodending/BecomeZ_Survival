using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_IdleState : PetBaseState
{
    PetController m_pc;

    public Field_IdleState(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PETSTATE.FIELD_IDLE;
        m_pc.m_petAnim.SetTrigger("Idle");
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
