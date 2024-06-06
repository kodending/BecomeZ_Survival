using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_IdleState : PlayerBaseState
{
    [Tooltip("이동 방향 설정")]
    Vector3 m_vecMove;
    float m_fMoveX = 0.0f;
    float m_fMoveY = 0.0f;

    PlayerController m_pc;

    public Normal_IdleState(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        //현재 플레이어 상태 지정
        m_pc.m_eCurAction = PLAYERSTATE.NORMAL_IDLE;

        m_pc.ActivePostureAnim("Idle");
    }

    public override void OnUpdateState()
    {
        m_pc.OnButtonRoll();
    }

    public override void OnFixedUpdateState()
    {
        if (GameManagerInGame.gm.m_gtCurType == GAMETYPE.BATTLE) return;

        m_fMoveX = m_pc.m_fixJoy.Horizontal;
        m_fMoveY = m_pc.m_fixJoy.Vertical;

        m_vecMove = new Vector3(m_fMoveX, 0, m_fMoveY);

        if (m_vecMove.sqrMagnitude == 0)
            return;    //움직임이 없다면 리턴해줘라

        //그게 아니라면 움직임이 있으니까 run으로 바꿔준다
        m_pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_RUN);
    }

    public override void OnExitState()
    {

    }
}
