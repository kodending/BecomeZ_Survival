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
        CurrentPlayerState?.OnExitState(); // ���� ���¸� �����ϰ�
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

    //���¸ӽ� ��������
    [HideInInspector]
    public PlayerStateMachine m_stateMachine { get; private set; }

    [Tooltip("���� ��ƽ")]
    [SerializeField]
    public FixedJoystick m_fixJoy;

    [Tooltip("�̵� �ӵ�")]
    public float m_fMoveSpeed;

    [Tooltip("�÷��̾� �ִϸ��̼� ����")]
    public List<GameObject> m_listPlayerCostume;

    [Tooltip("���� �÷��̾� ĳ���� �ִϸ��̼�")]
    [HideInInspector]
    public Animator m_curAnim;

    [Tooltip("���� �÷��̾� ĳ���� Ʈ������")]
    [HideInInspector]
    public Transform m_curTransform;

    [Tooltip("ī�޶��� �������� ���")]
    public Transform m_trCam;

    [Tooltip("�� üũ�� box")]
    [SerializeField]
    private Transform m_trGroundBox;

    [SerializeField]
    private LayerMask m_lmGround;

    [Tooltip("���� �÷��̾��� ����")]
    [HideInInspector] public PLAYERSTATE m_eCurAction;

    public PLAYERINFO m_sPlayerInfo;

    [Tooltip("���� �� ���ƿ� �� �����ڸ�")]
    public Vector3 m_vReturn;

    [Tooltip("���� �� ���ƿ� �� ���ι���")]
    public Quaternion m_rotReturn;

    [Tooltip("��밡 ���� ���� �� �ڸ�")]
    public Transform m_trAttackPoint;

    [Tooltip("���� ����� ������Ʈ �ޱ�")]
    public EnemyFSM m_fsmEnemy;

    [HideInInspector]
    [Tooltip("������ �� ������Ʈ")]
    public GameObject m_goRightHand;

    [Tooltip("������ �� ã������ ��Ʈ�� ����Ʈ")]
    public List<string> m_strRHandList;

    [HideInInspector]
    [Tooltip("���� �� ������Ʈ")]
    public GameObject m_goLeftHand;

    [Tooltip("���� �� ã������ ��Ʈ�� ����Ʈ")]
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
            //�� ���·� �ٲ۴�
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


    //���̽�ƽ ��Ȱ��ȭ��Ű��
    public void OnSetActiveJoy(bool i_bActive)
    {
        m_fixJoy.input = Vector2.zero;
        m_fixJoy.gameObject.SetActive(i_bActive);
    }

    //ĳ���� ���� ��ġ��Ű��
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

        //�������� ��������
        m_sPlayerInfo.leftWeaponIndex = int.Parse(playerInfo[0]["LEFTWEAPON"].ToString());
        m_sPlayerInfo.rightWeaponIndex = int.Parse(playerInfo[0]["RIGHTWEAPON"].ToString());
        m_sPlayerInfo.isTwoHanded = Convert.ToBoolean(int.Parse(playerInfo[0]["TWOHANDED"].ToString()));

        transform.GetChild(m_sPlayerInfo.costume).gameObject.SetActive(true);

        m_curAnim = m_listPlayerCostume[m_sPlayerInfo.costume].GetComponent<Animator>();
        m_curTransform = m_listPlayerCostume[m_sPlayerInfo.costume].GetComponent<Transform>();

        //���� Ŀ������ �� ��������
        RefreshFindHand(m_sPlayerInfo.costume);
        RefreshStatus();
    }

    public void RefreshFindHand(int i_iCostumeIdx)
    {
        //�̶� �ѹ� ������ �������� ������ �� ���ش�.
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

        //�������� �Լ� �����
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
        //�̰� ���߿� �� CSVManager�� ����Ʈ�� �� �޾� ���߰ڴ�.
        List<Dictionary<string, object>> weaponList = CSVManager.instance.m_dicData[LOADTYPE.WEAPON].recordDataList;

        List<Dictionary<string, object>> playerInfo = CSVManager.instance.m_dicData[LOADTYPE.CHARINFO].recordDataList;

        if (m_sPlayerInfo.rightWeaponIndex != 0)
        {
            string strWeaponIdx = "WEAPON/" + m_sPlayerInfo.rightWeaponIndex.ToString();

            //�ش� ���� ������Ʈ�� ��������
            GameObject go = Resources.Load(strWeaponIdx) as GameObject;

            //�����ؾ��� ������ �ڽ��� �� �ڽĿ�����Ʈ�� �ҷ�����
            m_sPlayerInfo.goRWeapon = Instantiate(go, m_goRightHand.transform);

            //��������Ʈ �ҷ��ͼ� �ڼ� �����ߵ�
            int postureIdx = int.Parse(weaponList[m_sPlayerInfo.rightWeaponIndex - 1]["POSTURE"].ToString());
            m_sPlayerInfo.ePosture = (PLAYERPOSTURE)postureIdx;

            //�ɷ�ġ ���� ������ߵ�
            m_sPlayerInfo.atk = int.Parse(playerInfo[0]["ATK"].ToString()) + 
                                int.Parse(weaponList[postureIdx - 1]["ATK"].ToString());
            
            m_sPlayerInfo.def = int.Parse(playerInfo[0]["DEF"].ToString()) +
                                int.Parse(weaponList[postureIdx - 1]["DEF"].ToString());
        }


        if(m_sPlayerInfo.leftWeaponIndex != 0)
        {
            string strWeaponIdx = "WEAPON/" + m_sPlayerInfo.leftWeaponIndex.ToString();

            //�ش� ���� ������Ʈ�� ��������
            GameObject go = Resources.Load(strWeaponIdx) as GameObject;

            //�����ؾ��� ������ �ڽ��� �� �ڽĿ�����Ʈ�� �ҷ�����
            m_sPlayerInfo.goLWeapon = Instantiate(go, m_goLeftHand.transform);

            //��������Ʈ �ҷ��ͼ� �ڼ� �����ߵ�
            int postureIdx = int.Parse(weaponList[m_sPlayerInfo.leftWeaponIndex - 1]["POSTURE"].ToString());
            m_sPlayerInfo.ePosture = (PLAYERPOSTURE)postureIdx;

            //�ɷ�ġ ���� ������ߵ�
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
        //��Ʈ������ �ް�
        //��Ʈ�� ��ġ��
        string strAnimName = m_sPlayerInfo.ePosture.ToString() + "_" + i_strAnim;
        //�� ��Ʈ�� Anim �������� �Ѵ�.
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
