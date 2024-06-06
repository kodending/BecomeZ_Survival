using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public struct ITEMINFO
{
    public LOADTYPE eType;
    public PLAYERPOSTURE ePosture;
    public string strName;
    public int iIdx;
    public int iHeal;
    public int iStack;
    public int iAtk;
    public int iDef;
    public int iLeft;
    public int iRight;
    public bool isTwoHanded;
}



public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;

    //������ �̹����� �����ϱ� ����
    [HideInInspector]
    public Transform parentAfterDrag;

    //�ش� ������ ������ �־��ִ� �Լ��� �����.
    //�׸��� �����ϴ� �����̳ʸ� �����.
    public ITEMINFO m_curItemInfo;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    public void RefreshItemInfo(LOADTYPE i_eType, int i_iIdx)
    {
        m_curItemInfo.eType = i_eType;
        m_curItemInfo.iIdx = i_iIdx;

        switch (i_eType)
        {
            case LOADTYPE.ITEM:
                List<Dictionary<string, object>> itemList = CSVManager.instance.m_dicData[LOADTYPE.ITEM].recordDataList;

                //���� ���õ� ������ 0���� �ʱ�ȭ
                m_curItemInfo.iAtk = 0;
                m_curItemInfo.iDef = 0;
                m_curItemInfo.iLeft = 0;
                m_curItemInfo.iRight = 0;
                m_curItemInfo.ePosture = 0;
                m_curItemInfo.isTwoHanded = false;

                //������ ���� �ֱ�
                m_curItemInfo.strName = itemList[i_iIdx - 1]["NAME"].ToString();
                m_curItemInfo.iHeal = int.Parse(itemList[i_iIdx - 1]["HEAL"].ToString());
                m_curItemInfo.iStack = int.Parse(itemList[i_iIdx - 1]["STACK"].ToString());

                image.sprite = InventoryManager.im.m_itemSpriteList[i_iIdx - 1];

                break;

            case LOADTYPE.WEAPON:
                List<Dictionary<string, object>> weaponList = CSVManager.instance.m_dicData[LOADTYPE.WEAPON].recordDataList;

                //�Ҹ𼺾����� ���õ� ������ 0���� �ʱ�ȭ
                m_curItemInfo.iHeal = 0;
                m_curItemInfo.iStack = 1;

                //���� ���� �ֱ�
                m_curItemInfo.strName   = weaponList[i_iIdx - 1]["NAME"].ToString();
                m_curItemInfo.iAtk      = int.Parse(weaponList[i_iIdx - 1]["ATK"].ToString());
                m_curItemInfo.iDef      = int.Parse(weaponList[i_iIdx - 1]["DEF"].ToString());
                m_curItemInfo.iLeft     = int.Parse(weaponList[i_iIdx - 1]["LEFT"].ToString());
                m_curItemInfo.iRight    = int.Parse(weaponList[i_iIdx - 1]["RIGHT"].ToString());
                m_curItemInfo.ePosture  = (PLAYERPOSTURE)int.Parse(weaponList[i_iIdx - 1]["POSTURE"].ToString());
                m_curItemInfo.isTwoHanded  = Convert.ToBoolean(int.Parse(weaponList[i_iIdx - 1]["TWOHANDED"].ToString()));

                image.sprite = InventoryManager.im.m_weaponSpriteList[i_iIdx - 1];

                break;
        }
    }
}
