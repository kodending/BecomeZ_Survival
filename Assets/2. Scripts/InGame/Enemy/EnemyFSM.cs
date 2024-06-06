using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition.Attributes;

public enum ENEMYSTATE
{
    BATTLE_STAND,
    BATTLE_RUN,
    BATTLE_ATTACK,
    BATTLE_DEFENSE,
    BATTLE_AVOID,
    BATTLE_COUNTER,
    BATTLE_DEATH,
    BATTLE_SPECIALATTACK,
    BATTLE_HIT,
    _MAX_
}

public struct ENEMYINFO
{
    [Tooltip("에너미 일련번호")]public int idx;
    [Tooltip("에너미 이름")]public string name;
    [Tooltip("에너미 최대HP")]public int maxHP;
    [Tooltip("에너미 현재HP")]public int curHP;
    [Tooltip("에너미 공격력")]public int atk;
    [Tooltip("에너미 스피드")]public int spd;
    [Tooltip("에너미 방어력")]public int def;
}

public abstract class EnemyBaseState
{
    protected EnemyFSM enemyFSM { get; private set; }

    public EnemyBaseState(EnemyFSM ef)
    {
        this.enemyFSM = ef;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

public class EnemyStateMachine
{
    public  EnemyBaseState CurrentEnemyState { get; private set; }
    private Dictionary<ENEMYSTATE, EnemyBaseState> dicEnemyStates =
                                                    new Dictionary<ENEMYSTATE, EnemyBaseState>();

    public EnemyStateMachine(ENEMYSTATE i_eEnemyState, EnemyBaseState i_sEnemy)
    {
        AddState(i_eEnemyState, i_sEnemy);
        CurrentEnemyState = GetEnemyState(i_eEnemyState);
    }

    public void AddState(ENEMYSTATE i_eEnemyState, EnemyBaseState i_sEnemy)
    {
        if (!dicEnemyStates.ContainsKey(i_eEnemyState))
        {
            dicEnemyStates.Add(i_eEnemyState, i_sEnemy);
        }
    }

    public EnemyBaseState GetEnemyState(ENEMYSTATE i_eEnemyState)
    {
        if (dicEnemyStates.TryGetValue(i_eEnemyState, out EnemyBaseState state))
        {
            return state;
        }

        return null;
    }

    public void DeleteEnemyState(ENEMYSTATE i_eEnemyState)
    {
        if (dicEnemyStates.ContainsKey(i_eEnemyState))
        {
            dicEnemyStates.Remove(i_eEnemyState);
        }
    }

    public void ChangeEnemyState(ENEMYSTATE i_eEnemyState)
    {
        CurrentEnemyState?.OnExitState(); // 현재 상태를 종료하고
        if (dicEnemyStates.TryGetValue(i_eEnemyState, out EnemyBaseState state))
        {
            CurrentEnemyState = state;
        }

        CurrentEnemyState?.OnEnterState();
    }

    public void UpdateState()
    {
        CurrentEnemyState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentEnemyState?.OnFixedUpdateState();
    }
}

public class EnemyFSM : MonoBehaviour
{
    //에너미 FSM에서 필요한거
    //1.animator 정보
    //2.enemy 정보(레벨공격력등)
    //3.현재체력
    //등등..

    [HideInInspector]
    public EnemyStateMachine m_stateMachine { get; private set; }

    [Tooltip("현재 에너미의 상태")]
    [HideInInspector] public ENEMYSTATE m_eCurAction;

    //anim 정보 받기
    [Tooltip("anim 정보 받기")]
    public Animator m_curAnim;

    [Tooltip("에너미 정보저장 구조체")]
    public ENEMYINFO m_sEnemyInfo;

    [Tooltip("전투 후 돌아올 때 본인자리")]
    public Vector3 m_vReturn;

    [Tooltip("전투 후 돌아올 때 본인방향")]
    public Quaternion m_rotReturn;

    [Tooltip("상대가 나를 때릴 때 자리")]
    public Transform m_trAttackPoint;

    [Tooltip("공격 대상 gameObject 저장")]
    public GameObject m_goAttackTarget;

    [Tooltip("공격 대상이 누군지 알아야됨")]
    public BATTLEENTRY m_eCurTargetEntry;

    public Material[] m_OrignalMats;

    int SelectedTime;

    private void Start()
    {
        InitStateMachine();

        SelectedTime = 0;
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
        EnemyFSM ef = GetComponent<EnemyFSM>();
        m_stateMachine = new EnemyStateMachine(ENEMYSTATE.BATTLE_STAND, new Battle_Stand_Enemy(ef));
        m_stateMachine.AddState(ENEMYSTATE.BATTLE_ATTACK, new Battle_Attack_Enemy(ef));
        m_stateMachine.AddState(ENEMYSTATE.BATTLE_DEFENSE, new Battle_Defense_Enemy(ef));
        m_stateMachine.AddState(ENEMYSTATE.BATTLE_DEATH, new Battle_Death_Enemy(ef));
        m_stateMachine.AddState(ENEMYSTATE.BATTLE_HIT, new Battle_Hit_Enemy(ef));

        m_stateMachine.CurrentEnemyState?.OnEnterState();
    }

    public void OnSelectedEnemy()
    {
        GameObject go;

        if (transform.GetChild(1).Find("Geometry") == null)
            go = transform.GetChild(1).GetChild(0).Find("Geometry").gameObject;
        else
            go = transform.GetChild(1).Find("Geometry").gameObject;

        go = go.transform.GetChild(0).gameObject;

        Material[] mat = new Material[2];
        mat[0] = go.GetComponent<SkinnedMeshRenderer>().materials[0];
        mat[1] = BattleSystem.bs.m_matOutline;

        go.GetComponent<SkinnedMeshRenderer>().materials = mat;

        SelectedTime++;
    }

    public void UnSelected()
    {
        SelectedTime--;

        if (SelectedTime > 0) return;

        GameObject go;

        if (transform.GetChild(1).Find("Geometry") == null)
            go = transform.GetChild(1).GetChild(0).Find("Geometry").gameObject;
        else
            go = transform.GetChild(1).Find("Geometry").gameObject;

        go = go.transform.GetChild(0).gameObject;

        go.GetComponent<SkinnedMeshRenderer>().materials = m_OrignalMats;
    }

    public void ClearSelected()
    {
        SelectedTime = 0;

        GameObject go;

        if (transform.GetChild(1).Find("Geometry") == null)
            go = transform.GetChild(1).GetChild(0).Find("Geometry").gameObject;
        else
            go = transform.GetChild(1).Find("Geometry").gameObject;

        go = go.transform.GetChild(0).gameObject;

        go.GetComponent<SkinnedMeshRenderer>().materials = m_OrignalMats;
    }
}
