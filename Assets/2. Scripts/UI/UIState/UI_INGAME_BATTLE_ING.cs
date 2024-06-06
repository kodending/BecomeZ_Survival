using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_INGAME_BATTLE_ING : UIBaseState
{
    public override void OnEnterState()
    {
        UIManager.instance.m_goCurCanvas = GameObject.Find("Canvas");
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
}
