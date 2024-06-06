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


//배틀진행 시 공격자의 선택정보를 저장하기위한 구조체
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
        CurrentBattleState?.OnExitState(); //현재 상태를 종료하는 메소드를 실행하고
        if (dicBattleStates.TryGetValue(i_eBattleState, out BattleBaseState state))
        {
            CurrentBattleState = state;
        }

        CurrentBattleState?.OnEnterState(); //다음 상태 진입 메서드 실행
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

    //상태머신 가져오기
    [HideInInspector]
    public BattleStateMachine m_stateMachine { get; private set; }

    //배틀페이즈 관련 로직 // 클래스를 새로 파야되는걸까?..

    // 1. 배틀은 턴제 방식으로 진행
    // 2. 공격순서 설정
    // 3. 무기에 따라 일반공격, 특수공격, 방어 나뉘어짐
    // 4. 재화를 통해 본인의 스탯을 올릴 수 있음
    // 5. Power, HP, Speed, Defense,
    // 6. enemy는 랜덤배치

    [Tooltip("소환 포지션 최상위 노드")]
    public GameObject m_goSpawnPrefab;

    [Tooltip("instantiate한 최상위 노드")]
    public GameObject m_goRootSpawner;

    //enemy Transform
    [HideInInspector]
    public Transform[] m_listEnemySpawn;

    //플레이어 Transform
    [HideInInspector]
    public Transform m_trPlayerSpawn;

    //펫 Transform
    [HideInInspector]
    public Transform m_trPetSpawn;

    //테스트를 위한 맵 선택
    public GameObject m_goForestMap;

    public struct BATTLEMAPINFO
    {
        public MAPTYPE mapType;
        public GameObject goBattleMap;
    }

    private Dictionary<MAPTYPE, BATTLEMAPINFO> m_dicMapInfo;

    [Tooltip("현재 선택해야할 자가 누구인지 알려주는 지표")]
    public SELECTTYPE m_curSelectType;

    public Camera m_cam;

    public Material m_matOutline;

    [Tooltip("배틀 시 선택한 정보들을 저장하기 위한 리스트")]
    public List<SELECTINFO> listSelectInfo;

    [Tooltip("생성된 에너미들을 리스트에 담아둔다.")]
    public List<GameObject> listEnemy;

    //현재 배틀라운드
    public int m_iCurBattleRound;

    //최종 배틀라운드확인용
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
