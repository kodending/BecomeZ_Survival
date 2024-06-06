using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWinResultState : BattleBaseState
{
    public override void OnEnterState()
    {
        Debug.Log("³Ê ÀÌ°å¾î!");

        StartCoroutine(ResultCount());
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

    IEnumerator ResultCount()
    {
        yield return new WaitForSeconds(2f);

        BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.END);
    }
}
