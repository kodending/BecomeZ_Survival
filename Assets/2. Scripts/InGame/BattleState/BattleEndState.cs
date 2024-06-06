using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndState : BattleBaseState
{
    public override void OnEnterState()
    {
        //End���� �Լ� �ְ�

        //�ٽ� ���� �ʵ���·� ���ư���
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;

        //���̽�ƽ Ȱ��ȭ ��Ű��
        PlayerController.pc.OnSetActiveJoy(true);

        //���ʹ�, �÷��̾� ��ġ ���״� �ֻ��� ��带 ���ش�
        //�׷� ���ʹ̵� ���������
        Destroy(BattleSystem.bs.m_goRootSpawner);

        //���� ���� �׾��ٸ� ��Ȱ��ȭ ��Ų��.
        if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
            PetController.pc.gameObject.SetActive(false);

        //ī�޶� �ʵ�� �ٲٱ�
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;

        //UI �ʵ�� �ٲٱ�
        UIManager.instance.ActiveFieldHUD(true);

        //�÷��̾� ��ġ �ű��
        PlayerController.pc.OnTeleportPlayer(CameraMoving.cm.m_trCamList[3]);

        CameraMoving.cm.FieldCameraRot();
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
