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
    [Tooltip("���ʹ� �Ϸù�ȣ")]public int idx;
    [Tooltip("���ʹ� �̸�")]public string name;
    [Tooltip("���ʹ� �ִ�HP")]public int maxHP;
    [Tooltip("���ʹ� ����HP")]public int curHP;
    [Tooltip("���ʹ� ���ݷ�")]public int atk;
    [Tooltip("���ʹ� ���ǵ�")]public int spd;
    [Tooltip("���ʹ� ����")]public int def;
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
        CurrentEnemyState?.OnExitState(); // ���� ���¸� �����ϰ�
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
    //���ʹ� FSM���� �ʿ��Ѱ�
    //1.animator ����
    //2.enemy ����(�������ݷµ�)
    //3.����ü��
    //���..

    [HideInInspector]
    public EnemyStateMachine m_stateMachine { get; private set; }

    [Tooltip("���� ���ʹ��� ����")]
    [HideInInspector] public ENEMYSTATE m_eCurAction;

    //anim ���� �ޱ�
    [Tooltip("anim ���� �ޱ�")]
    public Animator m_curAnim;

    [Tooltip("���ʹ� �������� ����ü")]
    public ENEMYINFO m_sEnemyInfo;

    [Tooltip("���� �� ���ƿ� �� �����ڸ�")]
    public Vector3 m_vReturn;

    [Tooltip("���� �� ���ƿ� �� ���ι���")]
    public Quaternion m_rotReturn;

    [Tooltip("��밡 ���� ���� �� �ڸ�")]
    public Transform m_trAttackPoint;

    [Tooltip("���� ��� gameObject ����")]
    public GameObject m_goAttackTarget;

    [Tooltip("���� ����� ������ �˾ƾߵ�")]
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
