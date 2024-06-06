using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Defense_Pet : PetBaseState
{
    PetController m_pc;

    //���� ���潺 �ɷ�ġ �����س���
    int m_iDef;

    public Battle_Defense_Pet(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PETSTATE.BATTLE_DEFENSE;
        m_pc.m_petAnim.SetTrigger("Idle");

        //���� ��Ƣ�� ��Ű��
        m_iDef = m_pc.m_sPetInfo.def;
        m_pc.m_sPetInfo.def = (int)(m_pc.m_sPetInfo.def * 1.5f);
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        //������ ��������
        m_pc.m_sPetInfo.def = m_iDef;
    }
}
