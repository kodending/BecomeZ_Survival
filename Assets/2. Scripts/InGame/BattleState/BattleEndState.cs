using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndState : BattleBaseState
{
    public override void OnEnterState()
    {
        //End관련 함수 넣고

        //다시 원래 필드상태로 돌아가기
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.FIELD;

        //조이스틱 활성화 시키기
        PlayerController.pc.OnSetActiveJoy(true);

        //에너미, 플레이어 위치 시켰던 최상위 노드를 없앤다
        //그럼 에너미도 사라지겠지
        Destroy(BattleSystem.bs.m_goRootSpawner);

        //펫이 만약 죽었다면 비활성화 시킨다.
        if (PetController.pc.m_eCurAction == PETSTATE.BATTLE_DEATH)
            PetController.pc.gameObject.SetActive(false);

        //카메라 필드로 바꾸기
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.FIELD;

        //UI 필드로 바꾸기
        UIManager.instance.ActiveFieldHUD(true);

        //플레이어 위치 옮기기
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
