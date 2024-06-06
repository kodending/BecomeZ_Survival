using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public enum BATTLESTATE
{
    APPEAR,
    READY,
    BATTLE,
    WIN_RESULT,
    DEFEAT_RESULT,
    END,
    _MAX_
}

public enum SELECTTYPE
{
    PLAYER_PICK,
    PLAYER_ATTACK,
    PLAYER_DEFENSE,
    PLAYER_ITEM,
    PET_PICK,
    PET_ATTACK,
    PET_DEFENSE,
    PET_ITEM,
    ENEMY_ATTACK,
    ENEMY_DEFENSE,
    BATTLE_STATE,
    _NONE_
}

public enum BATTLEENTRY
{
    PLAYER,
    PET,
    ENEMY,
    _NONE_
}


//��Ʋ���� �� �������� ���������� �����ϱ����� ����ü
public struct SELECTINFO
{
    public SELECTTYPE   eSelectType;
    public GameObject   goSelector;
    public GameObject   goSelected;
    public int          iSpeed;
    public BATTLEENTRY  eSelector;
    public BATTLEENTRY  eSelected;
}



public abstract class BattleBaseState : MonoBehaviour
{
    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

public class BattleStateMachine
{
    public BattleBaseState CurrentBattleState { get; private set; }
    private Dictionary<BATTLESTATE, BattleBaseState> dicBattleStates =
                                                     new Dictionary<BATTLESTATE, BattleBaseState>();

    public BattleStateMachine(BATTLESTATE i_eBattleState, BattleBaseState i_sBattle)
    {
        AddState(i_eBattleState, i_sBattle);
        CurrentBattleState = GetBattleState(i_eBattleState);
    }

    public void AddState(BATTLESTATE i_eBattleState, BattleBaseState i_sBattle)
    {
        if (!dicBattleStates.ContainsKey(i_eBattleState))
        {
            dicBattleStates.Add(i_eBattleState, i_sBattle);
        }
    }

    public BattleBaseState GetBattleState(BATTLESTATE i_eBattleState)
    {
        if(dicBattleStates.TryGetValue(i_eBattleState, out BattleBaseState state))
        {
            return state;
        }

        return null;
    }

    public void DeleteBattleState(BATTLESTATE i_eBattleState)
    {
        if(dicBattleStates.ContainsKey(i_eBattleState))
        {
            dicBattleStates.Remove(i_eBattleState);
        }
    }

    public void ChangeBattleState(BATTLESTATE i_eBattleState)
    {
        CurrentBattleState?.OnExitState(); //���� ���¸� �����ϴ� �޼ҵ带 �����ϰ�
        if (dicBattleStates.TryGetValue(i_eBattleState, out BattleBaseState state))
        {
            CurrentBattleState = state;
        }

        CurrentBattleState?.OnEnterState(); //���� ���� ���� �޼��� ����
    }

    public void UpdateState()
    {
        CurrentBattleState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentBattleState?.OnFixedUpdateState();
    }
}

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem bs;

    //���¸ӽ� ��������
    [HideInInspector]
    public BattleStateMachine m_stateMachine { get; private set; }

    //��Ʋ������ ���� ���� // Ŭ������ ���� �ľߵǴ°ɱ�?..

    // 1. ��Ʋ�� ���� ������� ����
    // 2. ���ݼ��� ����
    // 3. ���⿡ ���� �Ϲݰ���, Ư������, ��� ��������
    // 4. ��ȭ�� ���� ������ ������ �ø� �� ����
    // 5. Power, HP, Speed, Defense,
    // 6. enemy�� ������ġ

    [Tooltip("��ȯ ������ �ֻ��� ���")]
    public GameObject m_goSpawnPrefab;

    [Tooltip("instantiate�� �ֻ��� ���")]
    public GameObject m_goRootSpawner;

    //enemy Transform
    [HideInInspector]
    public Transform[] m_listEnemySpawn;

    //�÷��̾� Transform
    [HideInInspector]
    public Transform m_trPlayerSpawn;

    //�� Transform
    [HideInInspector]
    public Transform m_trPetSpawn;

    //�׽�Ʈ�� ���� �� ����
    public GameObject m_goForestMap;

    public struct BATTLEMAPINFO
    {
        public MAPTYPE mapType;
        public GameObject goBattleMap;
    }

    private Dictionary<MAPTYPE, BATTLEMAPINFO> m_dicMapInfo;

    [Tooltip("���� �����ؾ��� �ڰ� �������� �˷��ִ� ��ǥ")]
    public SELECTTYPE m_curSelectType;

    public Camera m_cam;

    public Material m_matOutline;

    [Tooltip("��Ʋ �� ������ �������� �����ϱ� ���� ����Ʈ")]
    public List<SELECTINFO> listSelectInfo;

    [Tooltip("������ ���ʹ̵��� ����Ʈ�� ��Ƶд�.")]
    public List<GameObject> listEnemy;

    //���� ��Ʋ����
    public int m_iCurBattleRound;

    //���� ��Ʋ����Ȯ�ο�
    public int m_iFinalBattleRound;

    private void Awake()
    {
        bs = this;
    }

    private void Start()
    {
        m_curSelectType = SELECTTYPE._NONE_;
        listSelectInfo = new List<SELECTINFO>();
        listEnemy = new List<GameObject>();
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
        m_stateMachine = new BattleStateMachine(BATTLESTATE.APPEAR, gameObject.AddComponent<AppearState>());
        m_stateMachine.AddState(BATTLESTATE.READY, gameObject.AddComponent<ReadyState>());
        m_stateMachine.AddState(BATTLESTATE.BATTLE, gameObject.AddComponent<BattleState>());
        m_stateMachine.AddState(BATTLESTATE.WIN_RESULT, gameObject.AddComponent<BattleWinResultState>());
        m_stateMachine.AddState(BATTLESTATE.DEFEAT_RESULT, gameObject.AddComponent<BattleDefeatResultState>());
        m_stateMachine.AddState(BATTLESTATE.END, gameObject.AddComponent<BattleEndState>());


        m_stateMachine.CurrentBattleState?.OnEnterState();
    }

    public void OnSelect(SELECTTYPE i_eSelectType)
    {
        m_curSelectType = i_eSelectType;
    }
}
