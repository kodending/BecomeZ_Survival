using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PETSTATE
{
    FIELD_IDLE,
    FIELD_FOLLOW,
    BATTLE_STAND,
    BATTLE_RUN,
    BATTLE_ATTACK,
    BATTLE_DEFENSE,
    BATTLE_AVOID,
    BATTLE_COUNTER,
    BATTLE_DEATH,
    BATTLE_HIT,
    _MAX_
}

public struct PETINFO
{
    public string name;
    public int hp;
    public int curHp;
    public int atk;
    public int spd;
    public int def;
    public int lv;
    public int nextExp;
    public int curExp;
}


public abstract class PetBaseState
{
    protected PetController petController { get; private set; }

    public PetBaseState(PetController pc)
    {
        this.petController = pc;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

public class PetStateMachine
{
    public PetBaseState CurrentPetState { get; private set; }
    private Dictionary<PETSTATE, PetBaseState> dicPetStates =
                                                    new Dictionary<PETSTATE, PetBaseState>();

    public PetStateMachine(PETSTATE i_ePetState, PetBaseState i_sPet)
    {
        AddState(i_ePetState, i_sPet);
        CurrentPetState = GetPetState(i_ePetState);
    }

    public void AddState(PETSTATE i_ePetState, PetBaseState i_sPet)
    {
        if (!dicPetStates.ContainsKey(i_ePetState))
        {
            dicPetStates.Add(i_ePetState, i_sPet);
        }
    }

    public PetBaseState GetPetState(PETSTATE i_ePetState)
    {
        if (dicPetStates.TryGetValue(i_ePetState, out PetBaseState state))
        {
            return state;
        }

        return null;
    }

    public void DeletePetState(PETSTATE i_ePetState)
    {
        if (dicPetStates.ContainsKey(i_ePetState))
        {
            dicPetStates.Remove(i_ePetState);
        }
    }

    public void ChangePetState(PETSTATE i_ePetState)
    {
        CurrentPetState?.OnExitState(); // 현재 상태를 종료하고
        if (dicPetStates.TryGetValue(i_ePetState, out PetBaseState state))
        {
            CurrentPetState = state;
        }

        CurrentPetState?.OnEnterState();
    }

    public void UpdateState()
    {
        CurrentPetState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentPetState?.OnFixedUpdateState();
    }
}

public class PetController : MonoBehaviour
{
    public static PetController pc;

    public Transform m_trTarget;
    Vector3 m_vInitPos;

    [HideInInspector]
    public PetStateMachine m_stateMachine { get; private set; }

    [Tooltip("현재 펫의 상태")]
    [HideInInspector] public PETSTATE m_eCurAction;

    [HideInInspector]
    public Animator m_petAnim;

    public PETINFO m_sPetInfo;

    [Tooltip("전투 후 돌아올 때 본인자리")]
    public Vector3 m_vReturn;

    [Tooltip("전투 후 돌아올 때 본인방향")]
    public Quaternion m_rotReturn;

    [Tooltip("상대가 나를 때릴 때 자리")]
    public Transform m_trAttackPoint;

    [Tooltip("공격 대상자 오브젝트 받기")]
    public EnemyFSM m_fsmEnemy;

    [Tooltip("현재 펫 게임오브젝트")]
    public GameObject m_goCurPet;

    private void Awake()
    {
        pc = this;
    }

    private void Start()
    {
        m_vInitPos = new Vector3(0, 0.2f, -4.0f);
        m_sPetInfo = new PETINFO();
        InitPos();
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

    //첫 위치 잡아주는거
    public void InitPos()
    {
        transform.position = m_trTarget.position + m_vInitPos;
        RefreshPetInfo();
    }

    public void OnTeleportPet(Transform i_trPet)
    {
        transform.position = i_trPet.position;
        transform.rotation = i_trPet.rotation;

        m_vReturn = i_trPet.position;
        m_rotReturn = i_trPet.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == LayerMask.NameToLayer("PetBoundary"))
        {
            if (!gameObject.activeSelf) return;
            if (GameManagerInGame.gm.m_gtCurType == GAMETYPE.BATTLE) return;

            m_stateMachine.ChangePetState(PETSTATE.FIELD_IDLE);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("PetBoundary"))
        {
            if (!gameObject.activeSelf) return;
            if (GameManagerInGame.gm.m_gtCurType != GAMETYPE.FIELD) return;

            m_stateMachine.ChangePetState(PETSTATE.FIELD_FOLLOW);
        }
    }

    public void InitStateMachine()
    {
        PetController pc = GetComponent<PetController>();
        m_stateMachine = new PetStateMachine(PETSTATE.FIELD_FOLLOW, new Field_FollowState(pc));
        m_stateMachine.AddState(PETSTATE.FIELD_IDLE, new Field_IdleState(pc));
        m_stateMachine.AddState(PETSTATE.BATTLE_STAND, new Battle_Stand_Pet(pc));
        m_stateMachine.AddState(PETSTATE.BATTLE_ATTACK, new Battle_Attack_Pet(pc));
        m_stateMachine.AddState(PETSTATE.BATTLE_DEFENSE, new Battle_Defense_Pet(pc));
        m_stateMachine.AddState(PETSTATE.BATTLE_HIT, new Battle_Hit_Pet(pc));
        m_stateMachine.AddState(PETSTATE.BATTLE_DEATH, new Battle_Death_Pet(pc));

        m_stateMachine.CurrentPetState?.OnEnterState();
    }

    public void OnBattlePhase()
    {
        m_stateMachine.ChangePetState(PETSTATE.BATTLE_STAND);
    }

    public void RefreshPetInfo()
    {
        List<Dictionary<string, object>> petInfoList = CSVManager.instance.m_dicData[LOADTYPE.MYPETINFO].recordDataList;

        for (int idx = 0; idx < petInfoList.Count; idx++)
        {
            if (int.Parse(petInfoList[idx]["FOLLOW"].ToString()) == 1)
            {
                GameObject followPet;
                string strPetIndex = "MONSTER/" + petInfoList[idx]["INDEX"].ToString();
                followPet = Resources.Load(strPetIndex) as GameObject;

                m_goCurPet = Instantiate(followPet, transform);
                m_petAnim = m_goCurPet.GetComponent<Animator>();

                m_sPetInfo.name = petInfoList[idx]["NAME"].ToString();
                m_sPetInfo.hp = int.Parse(petInfoList[idx]["HP"].ToString());
                m_sPetInfo.atk = int.Parse(petInfoList[idx]["ATK"].ToString());
                m_sPetInfo.spd = int.Parse(petInfoList[idx]["SPD"].ToString());
                m_sPetInfo.def = int.Parse(petInfoList[idx]["DEF"].ToString());
                m_sPetInfo.lv = int.Parse(petInfoList[idx]["LEVEL"].ToString());
                m_sPetInfo.curHp = int.Parse(petInfoList[idx]["CURHP"].ToString());
                m_sPetInfo.nextExp = int.Parse(petInfoList[idx]["NEXTEXP"].ToString());
                m_sPetInfo.curExp = int.Parse(petInfoList[idx]["CUREXP"].ToString());
            }
        }
    }
}
