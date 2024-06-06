using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Defense_State : PlayerBaseState
{
    PlayerController m_pc;

    //원래 디펜스 능력치 저장해놓기
    int m_iDef;

    public Battle_Defense_State(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        //현재 플레이어 상태 지정
        m_pc.m_eCurAction = PLAYERSTATE.BATTLE_NORMAL_DEFENSE;

        m_pc.ActivePostureAnim("Idle");

        //방어스탯 뻥튀기 시키기
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
        //끝나면 돌려놓기
        m_pc.m_sPlayerInfo.def = m_iDef;
    }
}
