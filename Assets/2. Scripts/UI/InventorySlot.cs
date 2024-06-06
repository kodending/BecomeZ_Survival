using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //먼저 드랍된 아이템 정보를 받고
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if(gameObject.name == "LeftWeaponSlot" || gameObject.name == "RightWeaponSlot")
        {
            if (draggableItem.m_curItemInfo.eType == LOADTYPE.ITEM) return;

            //왼쪽에 껴도 되는 무기가 아니면
            if (gameObject.name == "LeftWeaponSlot" && draggableItem.m_curItemInfo.iLeft != 1) return;

            //오른쪽에 껴도 되는 무기가 아니면
            if (gameObject.name == "RightWeaponSlot" && draggableItem.m_curItemInfo.iRight != 1) return;
        }

        //양손무기이면
        if(draggableItem.m_curItemInfo.isTwoHanded)
        {
            //왼쪽 무기에 끼는 템이면 왼쪽에 끼고
            if (gameObject.name == "LeftWeaponSlot" && draggableItem.m_curItemInfo.iLeft == 1)
            {
                //혹시 오른쪽에 무기를 착용하고 있었다면
                //빈 인벤 자리로 옮겨준다.
                if (InventoryManager.im.m_iCurRightIdx > 0)
                {
                    GameObject go = InventoryManager.im.m_goRightSlot.transform.Find("Item").gameObject;

                    InventoryManager.im.PutdownWeapon(go);
                }

                InventoryManager.im.m_iCurLeftIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, 0);
            }

            //오른쪽 무기에 끼는 템이면 왼쪽에 끼고
            if (gameObject.name == "RightWeaponSlot" && draggableItem.m_curItemInfo.iRight == 1)
            {
                //혹시 오른쪽에 무기를 착용하고 있었다면
                //빈 인벤 자리로 옮겨준다.
                if (InventoryManager.im.m_iCurLeftIdx > 0)
                {
                    GameObject go = InventoryManager.im.m_goLeftSlot.transform.Find("Item").gameObject;

                    InventoryManager.im.PutdownWeapon(go);
                }

                InventoryManager.im.m_iCurRightIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, 0);
            }
        }

        //양손무기가 아니면
        else
        {
            //왼쪽에 껴도 되는 무기면 정보저장
            if (gameObject.name == "LeftWeaponSlot" && draggableItem.m_curItemInfo.iLeft == 1)
            {
                InventoryManager.im.m_iCurLeftIdx = draggableItem.m_curItemInfo.iIdx;

                InventoryManager.im.RefreshEquipWeapon(InventoryManager.im.m_iCurLeftIdx, InventoryManager.im.m_iCurRightIdx);
            }

            //오른쪽에 껴도 되는 무기면 정보저장
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

        //그리고 드랍한 아이템의 위치를 바꿔준다.
        draggableItem.parentAfterDrag = transform;
        draggableItem.transform.position = transform.position;
    }
}
