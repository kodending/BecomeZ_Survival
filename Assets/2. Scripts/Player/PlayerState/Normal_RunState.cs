using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_RunState : PlayerBaseState
{
    [Tooltip("이동 방향 설정")]
    Vector3 m_vecMove;
    float m_fMoveX = 0.0f;
    float m_fMoveY = 0.0f;

    PlayerController m_pc;

    public Normal_RunState(PlayerController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PLAYERSTATE.NORMAL_RUN;

        m_pc.ActivePostureAnim("Run");
    }

    public override void OnUpdateState()
    {
        m_pc.OnButtonRoll();
    }

    public override void OnFixedUpdateState()
    {
        m_fMoveX = m_pc.m_fixJoy.Horizontal;
        m_fMoveY = m_pc.m_fixJoy.Vertical;

        m_vecMove = new Vector3(m_fMoveX, 0, m_fMoveY);

        if (m_vecMove.sqrMagnitude == 0)
        {
            m_pc.m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_IDLE);
            return;    //움직임이 없다면 idle 상태로 바꿔줘라
        }

        Vector3 conDirAngle = Quaternion.LookRotation(m_vecMove).eulerAngles;
        Vector3 camPivotAngle = m_pc.m_trCam.eulerAngles;

        Vector3 moveAngle = Vector3.up * (conDirAngle.y + camPivotAngle.y);

        m_pc.transform.rotation = Quaternion.Euler(moveAngle);

        if (m_pc.IsGround() && !m_pc.m_isWallCheck)
            m_pc.m_rigidPlayer.MovePosition(m_pc.transform.position + m_pc.transform.forward * m_pc.m_fMoveSpeed * Time.deltaTime);
    }

    public override void OnExitState()
    {

    }
}
