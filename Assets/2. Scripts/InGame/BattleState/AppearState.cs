using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearState : BattleBaseState
{
    public override void OnEnterState()
    {
        StartCoroutine(InitBattle());

        //battle 텍스트 UI 활성화 상태변경
        //UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);

        StartCoroutine(UIManager.instance.BattleTextOn());
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        
    }


    private IEnumerator InitBattle()
    {
        //배틀시스템이 시작되었으니 게임타입을 변경
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.BATTLE;

        //1. 맵 컨셉에 맞는 지형 선택

        //2. 지형 위치에 스포너 새로 생성
        //해당 오브젝트들을 해당 배틀맵으로 Instantiate를 해주어야 한다.

        BattleSystem.bs.m_goRootSpawner = Instantiate(BattleSystem.bs.m_goSpawnPrefab, BattleSystem.bs.m_goForestMap.transform);

        GameObject enemyParent = BattleSystem.bs.m_goRootSpawner.GetComponent<Transform>().GetChild(0).gameObject;
        GameObject friendlyParent = BattleSystem.bs.m_goRootSpawner.GetComponent<Transform>().GetChild(1).gameObject;

        //3. 스포너위치에 에너미, 플레이어, 펫 위치시키기

        //본인을 포함해서 자식까지 담았네, 0번째는 제외해야됨
        //enemy위치 시키기
        BattleSystem.bs.m_listEnemySpawn = enemyParent.GetComponentsInChildren<Transform>();
        OnEnemyLeveling();

        //플레이어 위치시키기
        BattleSystem.bs.m_trPlayerSpawn = friendlyParent.GetComponent<Transform>().GetChild(0);

        PlayerController.pc.OnTeleportPlayer(BattleSystem.bs.m_trPlayerSpawn);
        PlayerController.pc.OnBattlePhase();
        PlayerController.pc.m_trAttackPoint = BattleSystem.bs.m_trPlayerSpawn.GetChild(0);

        //카메라 위치시키기
        //카메라 타입 바꾸기
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.BATTLE_TYPE;

        //펫 위치시키기
        BattleSystem.bs.m_trPetSpawn = friendlyParent.GetComponent<Transform>().GetChild(1);

        //펫 정보 받아와서 위치시켜야됨
        PetController.pc.OnTeleportPet(BattleSystem.bs.m_trPetSpawn);
        PetController.pc.OnBattlePhase();
        PetController.pc.m_trAttackPoint = BattleSystem.bs.m_trPetSpawn.GetChild(0);

        //4. 조이스틱 조작 못하도록 비활성화
        PlayerController.pc.OnSetActiveJoy(false);

        //5. 배틀 UI로 변경
        UIManager.instance.m_stateMachine.ChangeUIState(UISTATE.INGAME_BATTLE);
        UIManager.instance.ActiveFieldHUD(false);

        yield return new WaitForSeconds(1.0f);
        CameraMoving.cm.m_bMoveTimeCam = true;

        yield return new WaitForSeconds(3.0f);

        CameraMoving.cm.m_bMoveTimeCam = false;

        BattleSystem.bs.m_stateMachine.ChangeBattleState(BATTLESTATE.READY);
    }

    private void OnEnemyLeveling()
    {
        //먼저 에너미리스트를 한번 비운다
        BattleSystem.bs.listEnemy.Clear();

        //1. 상대 레벨을 확인하고
        int playerLV = 1;

        //에너미 리스트를 받고
        List<Dictionary<string, object>> enemyInfoList = CSVManager.instance.m_dicData[LOADTYPE.MONSTER].recordDataList;
        //에너미 종류가 총 몇가지 있는지 확인
        int enemyTypeMax = enemyInfoList.Count;
        //2. 에너미를 얼마나 소환할지 확인하고
        int randNum = Random.Range(1, 11); //에너미 뽑아낼 숫자를 정한다 * 나누기 2한게 에너미숫자임
        //test
        //int randNum = Random.Range(1, 3); //에너미 뽑아낼 숫자를 정한다 * 나누기 2한게 에너미숫자임
        for(int idx = 1; idx < randNum + 1;)
        {
            //3. 상대 레벨에 따라 에너미를 어떤걸 소환할지 정하고
            int enemyIndex = Random.Range(1, playerLV + 3);
            if (enemyIndex >= enemyTypeMax) enemyIndex = enemyTypeMax;
            //배틀시스템에 에너미 리스트 가져오고
            GameObject goEnemy;
            string strEnemyIdx = "MONSTER/" + enemyInfoList[enemyIndex]["INDEX"].ToString();
            goEnemy = Instantiate(Resources.Load(strEnemyIdx) as GameObject, BattleSystem.bs.m_listEnemySpawn[idx]);

            //에너미 리스트에 에너미 정보 넣는다
            EnemyFSM enemyFSM = BattleSystem.bs.m_listEnemySpawn[idx].GetComponent<EnemyFSM>();
            enemyFSM.m_curAnim = goEnemy.GetComponent<Animator>();

            //초기정보 넣기
            enemyFSM.m_sEnemyInfo.idx   = int.Parse(enemyInfoList[enemyIndex]["INDEX"].ToString());
            enemyFSM.m_sEnemyInfo.name  = enemyInfoList[enemyIndex]["NAME"].ToString();
            enemyFSM.m_sEnemyInfo.maxHP = int.Parse(enemyInfoList[enemyIndex]["HP"].ToString());
            enemyFSM.m_sEnemyInfo.curHP = enemyFSM.m_sEnemyInfo.maxHP;
            enemyFSM.m_sEnemyInfo.atk   = int.Parse(enemyInfoList[enemyIndex]["ATK"].ToString());
            enemyFSM.m_sEnemyInfo.spd   = int.Parse(enemyInfoList[enemyIndex]["SPD"].ToString());
            enemyFSM.m_sEnemyInfo.def   = int.Parse(enemyInfoList[enemyIndex]["DEF"].ToString());

            enemyFSM.m_trAttackPoint = BattleSystem.bs.m_listEnemySpawn[idx].GetChild(0).transform;
            enemyFSM.m_vReturn = enemyFSM.transform.position;
            enemyFSM.m_rotReturn = enemyFSM.transform.rotation;

            GameObject go;

            //원 머터리얼 색상 넣어야됨 여기에
            if (enemyFSM.transform.GetChild(1).Find("Geometry") == null)
                go = enemyFSM.transform.GetChild(1).GetChild(0).Find("Geometry").gameObject;
            else
                go = enemyFSM.transform.GetChild(1).Find("Geometry").gameObject;

            go = go.transform.GetChild(0).gameObject;

            enemyFSM.m_OrignalMats = go.GetComponent<SkinnedMeshRenderer>().materials;

            BattleSystem.bs.listEnemy.Add(enemyFSM.gameObject);

            idx += 2;
        }
    }
}
