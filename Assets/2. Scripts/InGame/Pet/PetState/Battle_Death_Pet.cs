using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Death_Pet : PetBaseState
{
    PetController m_pc;

    public Battle_Death_Pet(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PETSTATE.BATTLE_DEATH;
        m_pc.m_petAnim.SetTrigger("Death");
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
