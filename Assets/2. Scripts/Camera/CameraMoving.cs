using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public enum CAMERATYPE
{
    BATTLE_TYPE,
    MAZE_TYPE,
    COSTUME_TYPE,
    PET_TYPE,
    INVEN_TYPE,
    FIELD,
    _MAX_
}

public class CameraMoving : MonoBehaviour
{
    public static CameraMoving cm;

    [Tooltip("ī�޶� Ÿ��")]
    public Transform m_trTarget;

    [Tooltip("Ÿ�ٰ��� ���� ���� offsetY")]
    public float m_fOffsetY;

    [Tooltip("Ÿ�ٰ��� ���� �Ÿ� ���� ��")]
    public Vector3 m_vOffset;

    [Tooltip("������� pointerID�� �ٸ�")]
    private int m_iPointerID;

    [Tooltip("���콺 or ��ġ ��ư ���Ⱚ �ޱ�")]
    private float m_fMouseX;

    [Tooltip("ī�޶� �����ִ��� Ȯ�ο�")]
    private bool m_bRotCam;

    private bool m_bClickUI;

    [Tooltip("��Ʋ Ÿ���϶� ī�޶� ��������")]
    [SerializeField]
    private Transform m_trBattle;

    [Tooltip("ī�޶����ٴ� ������Ʈ")]
    [SerializeField]
    private Transform m_trCamPos;

    [Tooltip("ī�޶� �����̴� �ñ�")]
    [HideInInspector]
    public bool m_bMoveTimeCam;

    public List<Transform> m_trCamList;

    public CAMERATYPE m_eCurCamType;

    Quaternion m_qInitCamRot;
    Vector3    m_vInitCamPos;

    private void Awake()
    {
        cm = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        m_iPointerID = -1; //PC�� ����Ƽ �󿡼��� -1
#elif UNITY_IOS || UNITY_IPHONE
        m_iPointerID = 0;  // �޴����̳� �̿ܿ��� ��ġ �󿡼��� 0 
#endif

        //Ÿ���߽� ���� ī�޶� �Ÿ� ���
        transform.position = m_trTarget.position + m_vOffset;
        m_qInitCamRot = transform.rotation;
        m_vInitCamPos = transform.position;

        m_eCurCamType = CAMERATYPE.FIELD;
    }

    private void LateUpdate()
    {
        OnMoveCamera(m_eCurCamType);
    }

    private void RotationCamera(float i_fMouseX)
    {
        if (i_fMouseX == 0) return;

        transform.position = m_trTarget.position + m_vOffset;
        transform.RotateAround(m_trTarget.position,
                               i_fMouseX < 0 ? Vector3.up : Vector3.down,
                               100f * Time.deltaTime);
        m_vOffset = transform.position - m_trTarget.position;

        m_qInitCamRot = transform.rotation;
        m_vInitCamPos = transform.position;
    }

    public IEnumerator OnInitCameraCoroutine(float i_fTime)
    {
        yield return new WaitForSeconds(i_fTime);

        Vector3 curTargetPos = m_trTarget.position;
        Vector3 curPos = m_vInitCamPos;
        transform.position = new Vector3(curPos.x, curTargetPos.y + m_fOffsetY, curPos.z);

        transform.rotation = m_qInitCamRot;
    }

    public void FieldCameraRot()
    {
        transform.rotation = m_qInitCamRot;
        transform.position = m_vInitCamPos;
    }

    //phase�� ���� ī�޶� ���������� �̵��ϵ��� �ϴ� ����
    private void OnMoveCamera(CAMERATYPE i_eCurType)
    {
        switch (i_eCurType)
        {
            case CAMERATYPE.FIELD:

                if (Input.GetMouseButtonDown(0) &&
                    EventSystem.current.IsPointerOverGameObject(m_iPointerID))
                {
                    m_bClickUI = true;
                }

                //ī�޶� ȸ����ų �� ����
                if (Input.GetMouseButton(0) &&
                    !m_bRotCam &&
                    !m_bClickUI &&
                   //UI �̺�Ʈ ��ġ�� �����ϴ� �Լ�
                   !EventSystem.current.IsPointerOverGameObject(m_iPointerID))
                {
                    m_fMouseX = Input.GetAxis("Mouse X");

                    if (m_fMouseX != 0)
                    {
                        m_bRotCam = true;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    m_bRotCam = false;
                    m_bClickUI = false;
                    m_fMouseX = 0f;

                    FieldCameraRot();
                }

                RotationCamera(m_fMouseX);

                //ī�޶� ���� ����
                Vector3 curTargetPos = m_trTarget.position;
                Vector3 curPos = transform.position;
                transform.position = new Vector3(curPos.x, curTargetPos.y + m_fOffsetY, curPos.z);

                transform.position = m_trTarget.position + m_vOffset;
                break;

            case CAMERATYPE.BATTLE_TYPE:

                if (!m_bMoveTimeCam) return;

                curTargetPos = m_trTarget.position + m_trCamList[(int)i_eCurType].position;
                transform.rotation = m_trCamList[(int)i_eCurType].rotation;
                transform.position = Vector3.Lerp(transform.position, curTargetPos, 0.01f);

                break;

            case CAMERATYPE.MAZE_TYPE:

                //ī�޶� ���� ����
                curTargetPos = m_trTarget.position;
                transform.rotation = m_trCamList[(int)i_eCurType].rotation;
                transform.position = new Vector3(m_trTarget.position.x, curTargetPos.y + 7f, m_trTarget.position.z);

                break;

            case CAMERATYPE.COSTUME_TYPE:
            case CAMERATYPE.PET_TYPE:
            case CAMERATYPE.INVEN_TYPE:

                transform.rotation = m_trCamList[(int)i_eCurType].rotation;
                transform.position = Vector3.Lerp(transform.position, m_trCamList[(int)i_eCurType].position, 0.1f);

            break;
        }
    }
}
