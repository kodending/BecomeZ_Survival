using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearState : BattleBaseState
{
    public override void OnEnterState()
    {
        StartCoroutine(InitBattle());

        //battle �ؽ�Ʈ UI Ȱ��ȭ ���º���
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
        //��Ʋ�ý����� ���۵Ǿ����� ����Ÿ���� ����
        GameManagerInGame.gm.m_gtCurType = GAMETYPE.BATTLE;

        //1. �� ������ �´� ���� ����

        //2. ���� ��ġ�� ������ ���� ����
        //�ش� ������Ʈ���� �ش� ��Ʋ������ Instantiate�� ���־�� �Ѵ�.

        BattleSystem.bs.m_goRootSpawner = Instantiate(BattleSystem.bs.m_goSpawnPrefab, BattleSystem.bs.m_goForestMap.transform);

        GameObject enemyParent = BattleSystem.bs.m_goRootSpawner.GetComponent<Transform>().GetChild(0).gameObject;
        GameObject friendlyParent = BattleSystem.bs.m_goRootSpawner.GetComponent<Transform>().GetChild(1).gameObject;

        //3. ��������ġ�� ���ʹ�, �÷��̾�, �� ��ġ��Ű��

        //������ �����ؼ� �ڽı��� ��ҳ�, 0��°�� �����ؾߵ�
        //enemy��ġ ��Ű��
        BattleSystem.bs.m_listEnemySpawn = enemyParent.GetComponentsInChildren<Transform>();
        OnEnemyLeveling();

        //�÷��̾� ��ġ��Ű��
        BattleSystem.bs.m_trPlayerSpawn = friendlyParent.GetComponent<Transform>().GetChild(0);

        PlayerController.pc.OnTeleportPlayer(BattleSystem.bs.m_trPlayerSpawn);
        PlayerController.pc.OnBattlePhase();
        PlayerController.pc.m_trAttackPoint = BattleSystem.bs.m_trPlayerSpawn.GetChild(0);

        //ī�޶� ��ġ��Ű��
        //ī�޶� Ÿ�� �ٲٱ�
        CameraMoving.cm.m_eCurCamType = CAMERATYPE.BATTLE_TYPE;

        //�� ��ġ��Ű��
        BattleSystem.bs.m_trPetSpawn = friendlyParent.GetComponent<Transform>().GetChild(1);

        //�� ���� �޾ƿͼ� ��ġ���Ѿߵ�
        PetController.pc.OnTeleportPet(BattleSystem.bs.m_trPetSpawn);
        PetController.pc.OnBattlePhase();
        PetController.pc.m_trAttackPoint = BattleSystem.bs.m_trPetSpawn.GetChild(0);

        //4. ���̽�ƽ ���� ���ϵ��� ��Ȱ��ȭ
        PlayerController.pc.OnSetActiveJoy(false);

        //5. ��Ʋ UI�� ����
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
        //���� ���ʹ̸���Ʈ�� �ѹ� ����
        BattleSystem.bs.listEnemy.Clear();

        //1. ��� ������ Ȯ���ϰ�
        int playerLV = 1;

        //���ʹ� ����Ʈ�� �ް�
        List<Dictionary<string, object>> enemyInfoList = CSVManager.instance.m_dicData[LOADTYPE.MONSTER].recordDataList;
        //���ʹ� ������ �� ��� �ִ��� Ȯ��
        int enemyTypeMax = enemyInfoList.Count;
        //2. ���ʹ̸� �󸶳� ��ȯ���� Ȯ���ϰ�
        int randNum = Random.Range(1, 11); //���ʹ� �̾Ƴ� ���ڸ� ���Ѵ� * ������ 2�Ѱ� ���ʹ̼�����
        //test
        //int randNum = Random.Range(1, 3); //���ʹ� �̾Ƴ� ���ڸ� ���Ѵ� * ������ 2�Ѱ� ���ʹ̼�����
        for(int idx = 1; idx < randNum + 1;)
        {
            //3. ��� ������ ���� ���ʹ̸� ��� ��ȯ���� ���ϰ�
            int enemyIndex = Random.Range(1, playerLV + 3);
            if (enemyIndex >= enemyTypeMax) enemyIndex = enemyTypeMax;
            //��Ʋ�ý��ۿ� ���ʹ� ����Ʈ ��������
            GameObject goEnemy;
            string strEnemyIdx = "MONSTER/" + enemyInfoList[enemyIndex]["INDEX"].ToString();
            goEnemy = Instantiate(Resources.Load(strEnemyIdx) as GameObject, BattleSystem.bs.m_listEnemySpawn[idx]);

            //���ʹ� ����Ʈ�� ���ʹ� ���� �ִ´�
            EnemyFSM enemyFSM = BattleSystem.bs.m_listEnemySpawn[idx].GetComponent<EnemyFSM>();
            enemyFSM.m_curAnim = goEnemy.GetComponent<Animator>();

            //�ʱ����� �ֱ�
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

            //�� ���͸��� ���� �־�ߵ� ���⿡
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
