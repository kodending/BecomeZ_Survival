using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Defense_State : PlayerBaseState
{
    PlayerController m_pc;

    //���� ���潺 �ɷ�ġ �����س���
    int m_iDef;

    public Battle_Defense_State(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        //���� �÷��̾� ���� ����
        m_pc.m_eCurAction = PLAYERSTATE.BATTLE_NORMAL_DEFENSE;

        m_pc.ActivePostureAnim("Idle");

        //���� ��Ƣ�� ��Ű��
        m_iDef = m_pc.m_sPlayerInfo.def;
        m_pc.m_sPlayerInfo.def = (int)(m_pc.m_sPlayerInfo.def * 1.5f);
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
        m_pc.m_sPlayerInfo.def = m_iDef;
    }
}
