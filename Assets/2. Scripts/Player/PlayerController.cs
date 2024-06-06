using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PLAYERSTATE
{
    NORMAL_IDLE,
    NORMAL_RUN,
    NORMAL_ROLL,
    NORMAL_DEAD,
    BATTLE_NORMAL_ATTACK,
    BATTLE_NORMAL_DEFENSE,
    BATTLE_NORMAL_AVOID,
    BATTLE_NORMAL_DEAD,
    BATTLE_NORMAL_HIT,
    _MAX_
}

public enum PLAYERPOSTURE
{
    NONE,
    DAGGER,
    KATANA,
    TWOHAND,
    SHIELD,
    _MAX_
}


public struct PLAYERINFO
{
    public int lv;
    public int hp;
    public int curHp;
    public int atk;
    public int spd;
    public int def;
    public int nextExp;
    public int curExp;
    public int costume;
    public int leftWeaponIndex;
    public int rightWeaponIndex;
    public bool isTwoHanded;
    public int myMoney;
    public GameObject goLWeapon;
    public GameObject goRWeapon;
    public PLAYERPOSTURE ePosture;
}

public struct TEMPSTATUS
{
    public int hp;
    public int atk;
    public int spd;
    public int def;
}



public abstract class PlayerBaseState
{ 
    protected PlayerController playerController { get; private set; }

    public PlayerBaseState(PlayerController pc)
    {
        this.playerController = pc;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

public class PlayerStateMachine
{
    public PlayerBaseState CurrentPlayerState { get; private set; }
    private Dictionary<PLAYERSTATE, PlayerBaseState> dicPlayerStates =
                                                    new Dictionary<PLAYERSTATE, PlayerBaseState>();

    public PlayerStateMachine(PLAYERSTATE i_ePlayerState, PlayerBaseState i_sPlayer)
    {
        AddState(i_ePlayerState, i_sPlayer);
        CurrentPlayerState = GetPlayerState(i_ePlayerState);
    }

    public void AddState(PLAYERSTATE i_ePlayerState, PlayerBaseState i_sPlayer)
    {
        if (!dicPlayerStates.ContainsKey(i_ePlayerState))
        {
            dicPlayerStates.Add(i_ePlayerState, i_sPlayer);
        }
    }

    public PlayerBaseState GetPlayerState(PLAYERSTATE i_ePlayerState)
    {
        if(dicPlayerStates.TryGetValue(i_ePlayerState, out PlayerBaseState state))
        {
            return state;
        }

        return null;
    }

    public void DeletePlayerState(PLAYERSTATE i_ePlayerState)
    {
        if(dicPlayerStates.ContainsKey(i_ePlayerState))
        {
            dicPlayerStates.Remove(i_ePlayerState);
        }
    }

    public void ChangePlayerState(PLAYERSTATE i_ePlayerState)
    {
        CurrentPlayerState?.OnExitState(); // 현재 상태를 종료하고
        if (dicPlayerStates.TryGetValue(i_ePlayerState, out PlayerBaseState state))
        {
            CurrentPlayerState = state;
        }

        CurrentPlayerState?.OnEnterState();
    }

    public void UpdateState()
    {
        CurrentPlayerState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentPlayerState?.OnFixedUpdateState();
    }
}



public class PlayerController : MonoBehaviour
{
    public static PlayerController pc;

    //상태머신 가져오기
    [HideInInspector]
    public PlayerStateMachine m_stateMachine { get; private set; }

    [Tooltip("조이 스틱")]
    [SerializeField]
    public FixedJoystick m_fixJoy;

    [Tooltip("이동 속도")]
    public float m_fMoveSpeed;

    [Tooltip("플레이어 애니메이션 관리")]
    public List<GameObject> m_listPlayerCostume;

    [Tooltip("현재 플레이어 캐릭터 애니메이션")]
    [HideInInspector]
    public Animator m_curAnim;

    [Tooltip("현재 플레이어 캐릭터 트랜스폼")]
    [HideInInspector]
    public Transform m_curTransform;

    [Tooltip("카메라의 방향정보 얻기")]
    public Transform m_trCam;

    [Tooltip("땅 체크용 box")]
    [SerializeField]
    private Transform m_trGroundBox;

    [SerializeField]
    private LayerMask m_lmGround;

    [Tooltip("현재 플레이어의 상태")]
    [HideInInspector] public PLAYERSTATE m_eCurAction;

    public PLAYERINFO m_sPlayerInfo;

    [Tooltip("전투 후 돌아올 때 본인자리")]
    public Vector3 m_vReturn;

    [Tooltip("전투 후 돌아올 때 본인방향")]
    public Quaternion m_rotReturn;

    [Tooltip("상대가 나를 때릴 때 자리")]
    public Transform m_trAttackPoint;

    [Tooltip("공격 대상자 오브젝트 받기")]
    public EnemyFSM m_fsmEnemy;

    [HideInInspector]
    [Tooltip("오른쪽 손 오브젝트")]
    public GameObject m_goRightHand;

    [Tooltip("오른쪽 손 찾기위한 스트링 리스트")]
    public List<string> m_strRHandList;

    [HideInInspector]
    [Tooltip("왼쪽 손 오브젝트")]
    public GameObject m_goLeftHand;

    [Tooltip("왼쪽 손 찾기위한 스트링 리스트")]
    public List<string> m_strLHandList;

    public Rigidbody m_rigidPlayer;

    [HideInInspector]
    public bool m_isWallCheck;

    public TEMPSTATUS m_sTempStat;

    private void Awake()
    {
        pc = this;
    }

    private void Start()
    {
        m_sTempStat = new TEMPSTATUS();

        RefreshInfo();
        InitStateMachine();
    }

    private void Update()
    {
        m_stateMachine?.UpdateState();
    }

    private void FixedUpdate()
    {
        m_stateMachine?.FixedUpdateState();
    }

    public void InitStateMachine()
    {
        PlayerController pc = GetComponent<PlayerController>();
        m_stateMachine = new PlayerStateMachine(PLAYERSTATE.NORMAL_IDLE, new Normal_IdleState(pc));
        m_stateMachine.AddState(PLAYERSTATE.NORMAL_RUN, new Normal_RunState(pc));
        m_stateMachine.AddState(PLAYERSTATE.NORMAL_ROLL, new Normal_RollState(pc));
        m_stateMachine.AddState(PLAYERSTATE.BATTLE_NORMAL_ATTACK, new Battle_Normal_Attack_State(pc));
        m_stateMachine.AddState(PLAYERSTATE.BATTLE_NORMAL_DEFENSE, new Battle_Defense_State(pc));
        m_stateMachine.AddState(PLAYERSTATE.BATTLE_NORMAL_HIT, new Battle_Normal_Hit_State(pc));
        m_stateMachine.AddState(PLAYERSTATE.BATTLE_NORMAL_DEAD, new Battle_Death_State(pc));

        m_stateMachine.CurrentPlayerState?.OnEnterState();
    }

    public bool IsGround()
    {
        return Physics.CheckBox(m_trGroundBox.position, m_trGroundBox.lossyScale,
                                transform.rotation, m_lmGround);
    }

    public void OnButtonRoll()
    {
        if (GameManagerInGame.gm.m_gtCurType == GAMETYPE.BATTLE ||
            GameManagerInGame.gm.m_gtCurType == GAMETYPE.MAZE) return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //롤 상태로 바꾼다
            StartCoroutine(RollActionCount());
        }
    }

    public void OnBattlePhase()
    {
        m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_IDLE);
    }

    private IEnumerator RollActionCount()
    {
        m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_ROLL);

        yield return new WaitForSeconds(0.9f);

        m_stateMachine.ChangePlayerState(PLAYERSTATE.NORMAL_IDLE);
    }


    //조이스틱 비활성화시키기
    public void OnSetActiveJoy(bool i_bActive)
    {
        m_fixJoy.input = Vector2.zero;
        m_fixJoy.gameObject.SetActive(i_bActive);
    }

    //캐릭터 강제 위치시키기
    public void OnTeleportPlayer(Transform i_trPlayer)
    {
        transform.position = i_trPlayer.position;
        transform.rotation = i_trPlayer.rotation;

        m_vReturn = i_trPlayer.position;
        m_rotReturn = i_trPlayer.rotation;
    }

    public void RefreshInfo()
    {
        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        m_sPlayerInfo.lv        = int.Parse(playerInfo[0]["LEVEL"].ToString());
        m_sPlayerInfo.hp        = int.Parse(playerInfo[0]["HP"].ToString());
        m_sPlayerInfo.curHp     = int.Parse(playerInfo[0]["CURHP"].ToString());
        m_sPlayerInfo.atk       = int.Parse(playerInfo[0]["ATK"].ToString());
        m_sPlayerInfo.spd       = int.Parse(playerInfo[0]["SPD"].ToString());
        m_sPlayerInfo.def       = int.Parse(playerInfo[0]["DEF"].ToString());
        m_sPlayerInfo.nextExp   = int.Parse(playerInfo[0]["NEXTEXP"].ToString());
        m_sPlayerInfo.curExp    = int.Parse(playerInfo[0]["CUREXP"].ToString());
        m_sPlayerInfo.costume   = int.Parse(playerInfo[0]["COSTUME"].ToString());
        m_sPlayerInfo.myMoney   = int.Parse(playerInfo[0]["MONEY"].ToString());

        //웨폰정보 가져오기
        m_sPlayerInfo.leftWeaponIndex = int.Parse(playerInfo[0]["LEFTWEAPON"].ToString());
        m_sPlayerInfo.rightWeaponIndex = int.Parse(playerInfo[0]["RIGHTWEAPON"].ToString());
        m_sPlayerInfo.isTwoHanded = Convert.ToBoolean(int.Parse(playerInfo[0]["TWOHANDED"].ToString()));

        transform.GetChild(m_sPlayerInfo.costume).gameObject.SetActive(true);

        m_curAnim = m_listPlayerCostume[m_sPlayerInfo.costume].GetComponent<Animator>();
        m_curTransform = m_listPlayerCostume[m_sPlayerInfo.costume].GetComponent<Transform>();

        //현재 커스텀의 손 가져오기
        RefreshFindHand(m_sPlayerInfo.costume);
        RefreshStatus();
    }

    public void RefreshFindHand(int i_iCostumeIdx)
    {
        //이때 한번 착용한 아이템이 있으면 다 없앤다.
        DestroyWeapon(m_goLeftHand);
        DestroyWeapon(m_goRightHand);

        m_goRightHand = transform.GetChild(i_iCostumeIdx).gameObject;
        m_goLeftHand = transform.GetChild(i_iCostumeIdx).gameObject;

        foreach (var go in m_strRHandList)
        {
            m_goRightHand = m_goRightHand.transform.Find(go).gameObject;
        }

        foreach (var go in m_strLHandList)
        {
            m_goLeftHand = m_goLeftHand.transform.Find(go).gameObject;
        }

        //무기장착 함수 만들기
        EquipWeapon();
    }

    void DestroyWeapon(GameObject parentObj)
    {
        if (parentObj == null) return;

        Transform[] childList =
            parentObj.GetComponentsInChildren<Transform>(true);
        if(childList != null)
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

    void EquipWeapon()
    {
        //이거 나중에 걍 CSVManager에 리스트로 쫙 받아 놔야겠다.
        List<Dictionary<string, object>> weaponList = CSVManager.instance.m_dicData[LOADTYPE.WEAPON].recordDataList;

        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        if (m_sPlayerInfo.rightWeaponIndex != 0)
        {
            string strWeaponIdx = "WEAPON/" + m_sPlayerInfo.rightWeaponIndex.ToString();

            //해당 웨폰 오브젝트를 가져오고
            GameObject go = Resources.Load(strWeaponIdx) as GameObject;

            //착용해야할 웨폰을 자신의 손 자식오브젝트로 불러오기
            m_sPlayerInfo.goRWeapon = Instantiate(go, m_goRightHand.transform);

            //웨폰리스트 불러와서 자세 잡아줘야됨
            int postureIdx = int.Parse(weaponList[m_sPlayerInfo.rightWeaponIndex - 1]["POSTURE"].ToString());
            m_sPlayerInfo.ePosture = (PLAYERPOSTURE)postureIdx;

            //능력치 뻠핑 시켜줘야됨
            m_sPlayerInfo.atk = int.Parse(playerInfo[0]["ATK"].ToString()) + 
                                int.Parse(weaponList[postureIdx - 1]["ATK"].ToString());
            
            m_sPlayerInfo.def = int.Parse(playerInfo[0]["DEF"].ToString()) +
                                int.Parse(weaponList[postureIdx - 1]["DEF"].ToString());
        }


        if(m_sPlayerInfo.leftWeaponIndex != 0)
        {
            string strWeaponIdx = "WEAPON/" + m_sPlayerInfo.leftWeaponIndex.ToString();

            //해당 웨폰 오브젝트를 가져오고
            GameObject go = Resources.Load(strWeaponIdx) as GameObject;

            //착용해야할 웨폰을 자신의 손 자식오브젝트로 불러오기
            m_sPlayerInfo.goLWeapon = Instantiate(go, m_goLeftHand.transform);

            //웨폰리스트 불러와서 자세 잡아줘야됨
            int postureIdx = int.Parse(weaponList[m_sPlayerInfo.leftWeaponIndex - 1]["POSTURE"].ToString());
            m_sPlayerInfo.ePosture = (PLAYERPOSTURE)postureIdx;

            //능력치 뻠핑 시켜줘야됨
            m_sPlayerInfo.atk = int.Parse(playerInfo[0]["ATK"].ToString()) +
                                int.Parse(weaponList[postureIdx - 1]["ATK"].ToString());

            m_sPlayerInfo.def = int.Parse(playerInfo[0]["DEF"].ToString()) +
                                int.Parse(weaponList[postureIdx - 1]["DEF"].ToString());
        }

        if(m_sPlayerInfo.leftWeaponIndex == 0 && m_sPlayerInfo.rightWeaponIndex == 0)
        {
            m_sPlayerInfo.ePosture = PLAYERPOSTURE.NONE;
        }
    }

    public void ActivePostureAnim(string i_strAnim)
    {
        //스트링으로 받고
        //스트링 합치고
        string strAnimName = m_sPlayerInfo.ePosture.ToString() + "_" + i_strAnim;
        //그 스트링 Anim 나오도록 한다.
        m_curAnim.SetTrigger(strAnimName);
    }

    public void SavePlayerInfo(int i_iSaveIdx, string i_strCate, int i_iChangeInfo)
    {
        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        playerInfo[i_iSaveIdx][i_strCate] = i_iChangeInfo;

        CSVManager.instance.SaveFile(LOADTYPE.CHARINFO, playerInfo);
    }

    public void RefreshStatus()
    {
        m_sPlayerInfo.hp = m_sPlayerInfo.hp + m_sTempStat.hp;
        m_sPlayerInfo.curHp = m_sPlayerInfo.curHp + m_sTempStat.hp;
        m_sPlayerInfo.atk = m_sPlayerInfo.atk + m_sTempStat.atk;
        m_sPlayerInfo.spd = m_sPlayerInfo.spd + m_sTempStat.spd;
        m_sPlayerInfo.def = m_sPlayerInfo.def + m_sTempStat.def;
    }

    public void ResetStatus()
    {
        m_sTempStat = new TEMPSTATUS();
    }
}
