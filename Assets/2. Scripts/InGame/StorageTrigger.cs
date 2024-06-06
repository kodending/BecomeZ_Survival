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
                //ĳ���� ��ġ��Ű��
                PlayerController.pc.OnTeleportPlayer(transform);
                PlayerController.pc.OnSetActiveJoy(false);

                //�� ��� ��Ȱ��ȭ ���ְ�
                PetController.pc.gameObject.SetActive(false);

                //���Ӹ�� �����ϰ�
                GameManagerInGame.gm.m_gtCurType = GAMETYPE.COSTUME;

                //UI�ٲ۴�.
                UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_COSTUME);

                //ī�޶� �ٲ۴�.
                CameraMoving.cm.m_eCurCamType = CAMERATYPE.COSTUME_TYPE;

                //Ʈ���� ��� ��Ȱ��ȭ ���ش�.
                //Ʈ���� ������Ʈ�� �˷��ش�.
                m_bActiveTrigger = true;
            }

           if(gameObject.name == "PetTrigger")
           {
                //ĳ���͵� ��� ��Ȱ��ȭ
                PlayerController.pc.gameObject.SetActive(false);

                //���̽�ƽ ��� ��Ȱ��ȭ�ϰ�
                PlayerController.pc.OnSetActiveJoy(false);

                //���� ��ġ��Ų��.
                PetController.pc.OnTeleportPet(transform);

                //�� Idle ���·� �ٲ��ش�.
                PetController.pc.m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);

                //���Ӹ�庯��
                GameManagerInGame.gm.m_gtCurType = GAMETYPE.PETCHANGE;

                //UI�� �ٲ۴�.
                UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_PET);

                //ī�޶� �ٲ۴�.
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
