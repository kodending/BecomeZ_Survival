using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_FollowState : PetBaseState
{
    PetController m_pc;

    public Field_FollowState(PetController pc) : base(pc)
    {
        m_pc = pc;
    }

    public override void OnEnterState()
    {
        m_pc.m_eCurAction = PETSTATE.FIELD_FOLLOW;
        m_pc.m_petAnim.SetTrigger("Running");
    }

    public override void OnUpdateState()
    {
        if(GameManagerInGame.gm.m_gtCurType == GAMETYPE.PETCHANGE)
        {
            m_pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);
        }
    }

    public override void OnFixedUpdateState()
    {
        Vector3 vTargetPos = new Vector3(m_pc.m_trTarget.position.x, 0.2f, m_pc.m_trTarget.position.z);
        Vector3 vPos = new Vector3(m_pc.transform.position.x, 0.2f, m_pc.transform.position.z);
        Vector3 vLookDir = vTargetPos - vPos;
        m_pc.transform.position = Vector3.Lerp(new Vector3(m_pc.transform.position.x, 0.2f, m_pc.transform.position.z),
                                          new Vector3(m_pc.m_trTarget.position.x, 0.2f, m_pc.m_trTarget.position.z), Time.deltaTime * 1.5f);
        m_pc.transform.rotation = Quaternion.Lerp(m_pc.transform.rotation, Quaternion.LookRotation(vLookDir), Time.deltaTime * 10f);
    }

    public override void OnExitState()
    {

    }
}
