using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_INGAME_BATTLE_RESULT : UIBaseState
{
    public override void OnEnterState()
    {
        //HP ���� HUD�� ��������.
        UIManager.instance.ActvieHPHud(false);
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
