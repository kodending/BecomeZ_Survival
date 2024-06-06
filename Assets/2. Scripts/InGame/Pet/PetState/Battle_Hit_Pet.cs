using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Hit_Pet : PetBaseState
{
    PetController m_pc;

    //원래 디펜스 능력치 저장해놓기
    int m_iDef;

    public Battle_Hit_Pet(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        if(m_pc.m_eCurAction == PETSTATE.BATTLE_DEFENSE)
        {
            //방어스탯 뻥튀기 시키기
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
        //원 상태로 되돌리기
        m_pc.m_sPetInfo.def = m_iDef;
    }

    IEnumerator HitToIdle()
    {
        yield return new WaitForSeconds(1f);

        //체력이 없으면 사망
        if(m_pc.m_sPetInfo.curHp <= 0)
            m_pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_DEATH);

        //방어상태였으면 Defense
        else if (m_pc.m_eCurAction == PETSTATE.BATTLE_DEFENSE)
            m_pc.m_stateMachine.ChangePetState(PETSTATE.BATTLE_DEFENSE);

        //그 외 스탠드상태
        else
            m_pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);
    }
}
