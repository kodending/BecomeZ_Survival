using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Normal_Hit_State : PlayerBaseState
{
    PlayerController m_pc;

    //���� ���潺 �ɷ�ġ �����س���
    int m_iDef;

    public Battle_Normal_Hit_State(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        if(m_pc.m_eCurAction == PLAYERSTATE.BATTLE_NORMAL_DEFENSE)
        {
            //���� ��Ƣ�� ��Ű��
            m_iDef = m_pc.m_sPlayerInfo.def;
            m_pc.m_sPlayerInfo.def = (int)(m_pc.m_sPlayerInfo.def * 1.5f);
        }

        m_pc.m_curAnim.SetTrigger("Common_Hit");

        m_pc.StartCoroutine(HitToIdle());
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

    IEnumerator HitToIdle()
    {
        yield return new WaitForSeconds(1f);

        //ü���� ������ ���
        if (m_pc.m_sPlayerInfo.curHp <= 0)
            m_pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_DEAD);

        //�����¿����� Defense
        else if (m_pc.m_eCurAction == PLAYERSTATE.BATTLE_NORMAL_DEFENSE)
            m_pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.BATTLE_NORMAL_DEFENSE);

        //�� �� ���ĵ����
        else
            m_pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_IDLE);
    }
}
