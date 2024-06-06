using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Hit_Pet : PetBaseState
{
    PetController m_pc;

    //���� ���潺 �ɷ�ġ �����س���
    int m_iDef;

    public Battle_Hit_Pet(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        if(m_pc.m_eCurAction == PETSTATE.BATTLE_DEFENSE)
        {
            //���� ��Ƣ�� ��Ű��
            m_iDef = m_pc.m_sPetInfo.def;
            m_pc.m_sPetInfo.def = (int)(m_pc.m_sPetInfo.def * 1.5f);
        }

        m_pc.m_petAnim.SetTrigger("Hit");

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
        //�� ���·� �ǵ�����
        m_pc.m_sPetInfo.def = m_iDef;
    }

    IEnumerator HitToIdle()
    {
        yield return new WaitForSeconds(1f);

        //ü���� ������ ���
        if(m_pc.m_sPetInfo.curHp <= 0)
            m_pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_DEATH);

        //�����¿����� Defense
        else if (m_pc.m_eCurAction == PETSTATE.BATTLE_DEFENSE)
            m_pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_DEFENSE);

        //�� �� ���ĵ����
        else
            m_pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);
    }
}
