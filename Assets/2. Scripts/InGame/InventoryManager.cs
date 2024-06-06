using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager im;

    [Tooltip("필드 인벤")]
    [SerializeField]
    GameObject m_goInvenField;

    public List<Sprite> m_itemSpriteList;

    public List<Sprite> m_weaponSpriteList;

    [SerializeField]
    GameObject m_goPlayerDummy;

    Animator m_curDummyAnim;

    GameObject m_goLeftHand;
    GameObject m_goRightHand;

    [SerializeField]
    List<GameObject> m_goInvenSlotList;

    public GameObject m_goLeftSlot;
    public GameObject m_goRightSlot;

    [Tooltip("더미 플레이어 애니메이션 관리")]
    public List<GameObject> m_listDummyCostume;

    [HideInInspector]
    public int m_iCurRightIdx;

    [HideInInspector]
    public int m_iCurLeftIdx;

    private void Awake()
    {
        im = this;
    }

    private void Start()
    {
        RefreshDummyInfo();
        InitInvenInfo();
    }

    public void InitInven()
    {
        RefreshDummyInfo();
        InitInvenInfo();
    }

    public void RefreshDummyInfo()
    {
        //먼저 활성화된 더미캐릭터 꺼버리고
        //()
        for(int idx = 0; idx < m_listDummyCostume.Count; idx++)
        {
            if(m_goPlayerDummy.transform.GetChild(idx).gameObject.activeSelf)
            {
                m_goPlayerDummy.transform.GetChild(idx).gameObject.SetActive(false);
                break;
            }
        }

        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        //캐릭터정보 가져오기
        m_iCurLeftIdx = int.Parse(playerInfo[0]["LEFTWEAPON"].ToString());
        m_iCurRightIdx = int.Parse(playerInfo[0]["RIGHTWEAPON"].ToString());

        int costumeIdx = int.Parse(playerInfo[0]["COSTUME"].ToString());

        //더미 캐릭터 애니메이션 활성화
        m_goPlayerDummy.transform.GetChild(costumeIdx).gameObject.SetActive(true);

        m_curDummyAnim = m_listDummyCostume[costumeIdx].GetComponent<Animator>();

        RefreshFindHand(costumeIdx, m_iCurLeftIdx, m_iCurRightIdx, true);
    }

    void RefreshFindHand(int i_iCostumeIdx, int i_iLeftIdx, int i_iRightIdx, bool i_isInit)
    {
        //이때 한번 착용한 아이템이 있으면 다 없앤다.
        DestroyWeapon(m_goLeftHand);
        DestroyWeapon(m_goRightHand);

        m_goRightHand = m_goPlayerDummy.transform.GetChild(i_iCostumeIdx).gameObject;
        m_goLeftHand = m_goPlayerDummy.transform.GetChild(i_iCostumeIdx).gameObject;

        foreach (var go in PlayerController.pc.m_strRHandList)
        {
            m_goRightHand = m_goRightHand.transform.Find(go).gameObject;
        }

        foreach (var go in PlayerController.pc.m_strLHandList)
        {
            m_goLeftHand = m_goLeftHand.transform.Find(go).gameObject;
        }

        //무기장착 함수 만들기
        EquipWeapon(i_iLeftIdx, i_iRightIdx, i_isInit);
    }

    public void RefreshEquipWeapon(int i_iLeftIdx, int i_iRightIdx, bool i_isInit = false)
    {
        //이때 한번 착용한 아이템이 있으면 다 없앤다.
        DestroyWeapon(m_goLeftHand);
        DestroyWeapon(m_goRightHand);

        m_iCurLeftIdx = i_iLeftIdx;
        m_iCurRightIdx = i_iRightIdx;

        //무기장착 함수 만들기
        EquipWeapon(i_iLeftIdx, i_iRightIdx, i_isInit);
    }

    void EquipWeapon(int i_iLeftIdx, int i_iRightIdx, bool i_isInit)
    {
        List<Dictionary<string, object>> weaponList = CSVManager.instance.m_dicData[LOADTYPE.WEAPON].recordDataList;

        PLAYERPOSTURE eDummyPosture = PLAYERPOSTURE.NONE;

        if (i_iRightIdx != 0)
        {
            string strWeaponIdx = "WEAPON/" + i_iRightIdx.ToString();

            //해당 웨폰 오브젝트를 가져오고
            GameObject go = Resources.Load(strWeaponIdx) as GameObject;

            Instantiate(go, m_goRightHand.transform);

            eDummyPosture = (PLAYERPOSTURE)int.Parse(weaponList[i_iRightIdx - 1]["POSTURE"].ToString());

            //내가 뭘 장착하고 있는지 플레이어한테 알려주고 저장시켜야한다.
            PlayerController.pc.SavePlayerInfo(0, "RIGHTWEAPON", i_iRightIdx);

            if(i_iLeftIdx == 0)
                PlayerController.pc.SavePlayerInfo(0, "LEFTWEAPON", 0);

            //아이템 장착 이미지 및 정보 넣기
            if (i_isInit)
            {
                DraggableItem dragItem = m_goRightSlot.transform.Find("Item").GetComponent<DraggableItem>();

                dragItem.gameObject.SetActive(true);

                dragItem.RefreshItemInfo(LOADTYPE.WEAPON, i_iRightIdx);
            }
        }


        if (i_iLeftIdx != 0)
        {
            string strWeaponIdx = "WEAPON/" + i_iLeftIdx.ToString();

            //해당 웨폰 오브젝트를 가져오고
            GameObject go = Resources.Load(strWeaponIdx) as GameObject;

            Instantiate(go, m_goLeftHand.transform);

            eDummyPosture = (PLAYERPOSTURE)int.Parse(weaponList[i_iLeftIdx - 1]["POSTURE"].ToString());

            //내가 뭘 장착하고 있는지 플레이어한테 알려주고 저장시켜야한다.
            PlayerController.pc.SavePlayerInfo(0, "LEFTWEAPON", i_iLeftIdx);

            if (i_iRightIdx == 0)
                PlayerController.pc.SavePlayerInfo(0, "RIGHTWEAPON", 0);

            if (i_isInit)
            {
                //아이템 장착 이미지 및 정보 넣기
                DraggableItem dragItem = m_goLeftSlot.transform.Find("Item").GetComponent<DraggableItem>();

                dragItem.gameObject.SetActive(true);

                dragItem.RefreshItemInfo(LOADTYPE.WEAPON, i_iLeftIdx);
            }
        }

        if (i_iRightIdx == 0 && i_iLeftIdx == 0)
        {
            eDummyPosture = PLAYERPOSTURE.NONE;

            PlayerController.pc.SavePlayerInfo(0, "RIGHTWEAPON", i_iRightIdx);
            PlayerController.pc.SavePlayerInfo(0, "LEFTWEAPON", i_iLeftIdx);

            if(i_isInit)
            {
                DraggableItem dragItem = m_goLeftSlot.transform.Find("Item").GetComponent<DraggableItem>();
                dragItem.gameObject.SetActive(false);

                dragItem = m_goRightSlot.transform.Find("Item").GetComponent<DraggableItem>();
                dragItem.gameObject.SetActive(false);
            }
        }

        ActivePostureAnim(eDummyPosture, "Idle");
        PlayerController.pc.RefreshInfo();
    }

    void ActivePostureAnim(PLAYERPOSTURE i_ePosture, string i_strAnim)
    {
        //스트링으로 받고
        //스트링 합치고
        string strAnimName = i_ePosture.ToString() + "_" + i_strAnim;
        //그 스트링 Anim 나오도록 한다.
        m_curDummyAnim.SetTrigger(strAnimName);
    }

    void DestroyWeapon(GameObject parentObj)
    {
        if (parentObj == null) return;

        Transform[] childList =
            parentObj.GetComponentsInChildren<Transform>(true);
        if (childList != null)
        {
            for (int idx = 2; idx < childList.Length; idx++)
            {
                if (childList[idx] != transform)
                {
                    Destroy(childList[idx].gameObject);
                }
            }
        }
    }

    public void InitInvenInfo()
    {
        List<Dictionary<string, object>> invenList = CSVManager.instance.m_dicData[LOADTYPE.INVENINFO].recordDataList;

        for(int idx = 0; idx < invenList.Count; idx++)
        {
            if (int.Parse(invenList[idx]["ITEMTYPE"].ToString()) != 7)
            {
                DraggableItem dragItem = m_goInvenSlotList[idx].transform.Find("Item").GetComponent<DraggableItem>();

                dragItem.gameObject.SetActive(true);

                dragItem.RefreshItemInfo((LOADTYPE)invenList[idx]["ITEMTYPE"], int.Parse(invenList[idx]["ITEMINDEX"].ToString()));
            }
        }
    }

    //인벤 빈자리에 아이템을 넣기위한 함수
    public void InputInvenItem(LOADTYPE i_eLoadType, int i_iIdx)
    {
        for (int idx = 0; idx < m_goInvenSlotList.Count; idx++)
        {
            //아이템이 없다면
            if (!m_goInvenSlotList[idx].transform.Find("Item").gameObject.activeSelf)
            {
                DraggableItem dragItem = m_goInvenSlotList[idx].transform.Find("Item").GetComponent<DraggableItem>();

                dragItem.gameObject.SetActive(true);

                dragItem.RefreshItemInfo(i_eLoadType, i_iIdx);

                break;
            }
        }
    }

    //착용중인 아이템을 수동으로 빈자리에 옮겨놓기 위한 함수
    public void PutdownWeapon(GameObject i_goWeapon)
    {
        for (int idx = 0; idx < m_goInvenSlotList.Count; idx++)
        {
            //아이템이 없다면

            if (m_goInvenSlotList[idx].transform.Find("Item") == null) continue;

            GameObject go = m_goInvenSlotList[idx].transform.Find("Item").gameObject;

            if (!go.activeSelf)
            {
                go.transform.SetParent(i_goWeapon.transform.parent.transform);
                go.transform.position = i_goWeapon.transform.parent.transform.position;

                i_goWeapon.transform.SetParent(m_goInvenSlotList[idx].transform);

                break;
            }
        }
    }

    public void SaveInvenInfo()
    {
        List<Dictionary<string, object>> invenList = CSVManager.instance.m_dicData[LOADTYPE.INVENINFO].recordDataList;

        for (int idx = 0; idx < m_goInvenSlotList.Count; idx++)
        {
            if (m_goInvenSlotList[idx].transform.Find("Item").gameObject.activeSelf)
            {
                DraggableItem dragItem = m_goInvenSlotList[idx].transform.Find("Item").GetComponent<DraggableItem>();

                invenList[idx]["ITEMTYPE"] = (int)dragItem.m_curItemInfo.eType;
                invenList[idx]["ITEMINDEX"] = dragItem.m_curItemInfo.iIdx;
                invenList[idx]["AMOUNT"] = dragItem.m_curItemInfo.iStack;
            }

            else
            {
                invenList[idx]["ITEMTYPE"] = 7;
                invenList[idx]["ITEMINDEX"] = 0;
                invenList[idx]["AMOUNT"] = 0;
            }
        }

        CSVManager.instance.SaveFile(LOADTYPE.INVENINFO, invenList);
    }

    public void ClearWeapon()
    {
        m_iCurLeftIdx = 0;
        m_iCurRightIdx = 0;

        RefreshEquipWeapon(0, 0);
    }
}