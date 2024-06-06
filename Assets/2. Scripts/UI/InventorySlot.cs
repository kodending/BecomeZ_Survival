using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //���� ����� ������ ������ �ް�
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if(gameObject.name == "LeftWeaponSlot" || gameObject.name == "RightWeaponSlot")
        {
            if (draggableItem.m_curItemInfo.eType == LOADTYPE.ITEM) return;

            //���ʿ� ���� �Ǵ� ���Ⱑ �ƴϸ�
            if (gameObject.name == "LeftWeaponSlot" && draggableItem.m_curItemInfo.iLeft != 1) return;

            //�����ʿ� ���� �Ǵ� ���Ⱑ �ƴϸ�
            if (gameObject.name == "RightWeaponSlot" && draggableItem.m_curItemInfo.iRight != 1) return;
        }

        //��չ����̸�
        if(draggableItem.m_curItemInfo.isTwoHanded)
        {
            //���� ���⿡ ���� ���̸� ���ʿ� ����
            if (gameObject.name == "LeftWeaponSlot" && draggableItem.m_curItemInfo.iLeft == 1)
            {
                //Ȥ�� �����ʿ� ���⸦ �����ϰ� �־��ٸ�
                //�� �κ� �ڸ��� �Ű��ش�.
                if (InventoryManager.im.m_iCurRightIdx > 0)
                {
                    GameObject go = InventoryManager.im.m_goRightSlot.transform.Find("Item").gameObject;

                    InventoryManager.im.PutdownWeapon(go);
                }

                InventoryManager.im.m_iCurLeftIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, 0);
            }

            //������ ���⿡ ���� ���̸� ���ʿ� ����
            if (gameObject.name == "RightWeaponSlot" && draggableItem.m_curItemInfo.iRight == 1)
            {
                //Ȥ�� �����ʿ� ���⸦ �����ϰ� �־��ٸ�
                //�� �κ� �ڸ��� �Ű��ش�.
                if (InventoryManager.im.m_iCurLeftIdx > 0)
                {
                    GameObject go = InventoryManager.im.m_goLeftSlot.transform.Find("Item").gameObject;

                    InventoryManager.im.PutdownWeapon(go);
                }

                InventoryManager.im.m_iCurRightIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, 0);
            }
        }

        //��չ��Ⱑ �ƴϸ�
        else
        {
            //���ʿ� ���� �Ǵ� ����� ��������
            if (gameObject.name == "LeftWeaponSlot" && draggableItem.m_curItemInfo.iLeft == 1)
            {
                InventoryManager.im.m_iCurLeftIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, InventoryManager.im.m_iCurRightIdx);
            }

            //�����ʿ� ���� �Ǵ� ����� ��������
            if (gameObject.name == "RightWeaponSlot" && draggableItem.m_curItemInfo.iRight == 1)
            {
                InventoryManager.im.m_iCurRightIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, InventoryManager.im.m_iCurRightIdx);
            }
        }

        if (transform.Find("Item") != null)
        {
            GameObject preItem = transform.Find("Item").gameObject;
            DraggableItem preDraggableItem = preItem.GetComponent<DraggableItem>();
            preDraggableItem.transform.SetParent(draggableItem.parentAfterDrag);
            preDraggableItem.transform.position = draggableItem.parentAfterDrag.transform.position;
        }

        if(draggableItem.parentAfterDrag.name == "LeftWeaponSlot")
        {
            InventoryManager.im.m_iCurLeftIdx = draggableItem.m_curItemInfo.iIdx;

            InventoryManager.im.RefreshEquipWeapon(0, InventoryManager.im.m_iCurRightIdx);
        }

        if (draggableItem.parentAfterDrag.name == "RightWeaponSlot")
        {
            InventoryManager.im.m_iCurRightIdx = draggableItem.m_curItemInfo.iIdx;

            InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, 0);
        }

        //�׸��� ����� �������� ��ġ�� �ٲ��ش�.
        draggableItem.parentAfterDrag = transform;
        draggableItem.transform.position = transform.position;
    }
}
