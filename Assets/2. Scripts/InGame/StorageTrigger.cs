using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTrigger : MonoBehaviour
{
    public bool m_bActiveTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
           if(gameObject.name == "CharTrigger" && !m_bActiveTrigger)
            {
                //캐릭터 위치시키고
                PlayerController.pc.OnTeleportPlayer(transform);
                PlayerController.pc.OnSetActiveJoy(false);

                //펫 잠깐 비활성화 해주고
                PetController.pc.gameObject.SetActive(false);

                //게임모드 변경하고
                GameManagerInGame.gm.m_gtCurType = GAMETYPE.COSTUME;

                //UI바꾼다.
                UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_COSTUME);

                //카메라 바꾼다.
                CameraMoving.cm.m_eCurCamType = CAMERATYPE.COSTUME_TYPE;

                //트리거 잠깐 비활성화 해준다.
                //트리거 오브젝트를 알려준다.
                m_bActiveTrigger = true;
            }

           if(gameObject.name == "PetTrigger")
           {
                //캐릭터도 잠깐 비활성화
                PlayerController.pc.gameObject.SetActive(false);

                //조이스틱 잠시 비활성화하고
                PlayerController.pc.OnSetActiveJoy(false);

                //펫을 위치시킨다.
                PetController.pc.OnTeleportPet(transform);

                //펫 Idle 상태로 바꿔준다.
                PetController.pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);

                //게임모드변경
                GameManagerInGame.gm.m_gtCurType = GAMETYPE.PETCHANGE;

                //UI를 바꾼다.
                UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_PET);

                //카메라 바꾼다.
                CameraMoving.cm.m_eCurCamType = CAMERATYPE.PET_TYPE;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (gameObject.name == "CharTrigger" && m_bActiveTrigger)
            {
                m_bActiveTrigger = false;
            }
        }
    }
}
