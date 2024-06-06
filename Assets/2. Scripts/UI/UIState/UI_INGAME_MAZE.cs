using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public enum DRAWCARDTYPE
{
    HP,
    ATK,
    SPD,
    DEF,
    WEAPON,
    _MAX_
}

public class UI_INGAME_MAZE : UIBaseState
{
    private Text m_txtMin;
    private Text m_txtSec;

    Button m_btnFirst;
    Button m_btnSecond;
    Button m_btnThird;

    List<DRAWCARDTYPE> m_eCardTypeList;

    bool m_isFirst;

    public override void OnEnterState()
    {
        //Ä«µå¼±ÅÃ °ü·Ã ÇÔ¼ö ¸¸µé±â
        m_btnFirst = UIManager.instance.m_goRandomCardHUD.transform.Find("WindowImg").
                            transform.Find("Grid").transform.Find("FirstCard").GetChild(0).GetComponent<Button>();
        m_btnFirst.onClick.AddListener(FirstButtonClick);

        m_btnSecond = UIManager.instance.m_goRandomCardHUD.transform.Find("WindowImg").
                    transform.Find("Grid").transform.Find("SecondCard").GetChild(0).GetComponent<Button>();
        m_btnSecond.onClick.AddListener(SecondButtonClick);

        m_btnThird = UIManager.instance.m_goRandomCardHUD.transform.Find("WindowImg").
                    transform.Find("Grid").transform.Find("ThirdCard").GetChild(0).GetComponent<Button>();
        m_btnThird.onClick.AddListener(ThirdButtonClick);

        GameObject rootTimer = UIManager.instance.m_goCurCanvas.transform.Find("Timer").gameObject;
        rootTimer.SetActive(true);

        if(!m_isFirst)
            InitUI();
    }

    void InitUI()
    {
        m_isFirst = true;

        //Maze ½ÃÀÛ Text ¶ç¿ì°í
        UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg("Å»Ãâ±¸¸¦ Ã£À¸¼¼¿ä", 2f));

        //Maze ½Ã°£ Å¸ÀÌ¸Ó ¶ç¿ì°í
        GameObject rootTimer = UIManager.instance.m_goCurCanvas.transform.Find("Timer").gameObject;
        m_txtMin = rootTimer.transform.Find("Minute").GetComponent<Text>();
        m_txtSec = rootTimer.transform.Find("Second").GetComponent<Text>();

        m_eCardTypeList = new List<DRAWCARDTYPE>();

        DrawPickCard();
    }

    public override void OnUpdateState()
    {
        RefreshTimer();
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        m_btnFirst.onClick.RemoveAllListeners();
        m_btnSecond.onClick.RemoveAllListeners();
        m_btnThird.onClick.RemoveAllListeners();

        GameObject rootTimer = UIManager.instance.m_goCurCanvas.transform.Find("Timer").gameObject;
        rootTimer.SetActive(false);
    }

    void RefreshTimer()
    {
        GameManagerInGame.gm.m_fMazeTimer -= Time.deltaTime;

        m_txtSec.text = ((int)GameManagerInGame.gm.m_fMazeTimer % 60).ToString("00");
        m_txtMin.text = ((int)GameManagerInGame.gm.m_fMazeTimer / 60).ToString("00");
    }

    void FirstButtonClick()
    {
       SelectedCard(m_eCardTypeList[0]);
    }

    void SecondButtonClick()
    {
        SelectedCard(m_eCardTypeList[1]);
    }

    void ThirdButtonClick()
    {
        SelectedCard(m_eCardTypeList[2]);
    }

    void SelectedCard(DRAWCARDTYPE i_eDraw)
    {
        int idx = 0;

        switch (i_eDraw)
        {
            case DRAWCARDTYPE.HP:
            case DRAWCARDTYPE.ATK:
            case DRAWCARDTYPE.SPD:
            case DRAWCARDTYPE.DEF:
                idx = ActiveStatProb();
                ApplyCardInfo(idx, i_eDraw);
            break;

            case DRAWCARDTYPE.WEAPON:
                idx = ActiveWeaponProb();
                ApplyCardInfo(idx, i_eDraw);
             break;
        }
    }

    void DrawPickCard()
    {
        if(m_eCardTypeList.Count != 0)
        {
            m_eCardTypeList.Clear();
        }

        for(int idx = 0; idx < 3; idx++)
        {
            DRAWCARDTYPE eDraw = ActiveTypeProb();

            m_eCardTypeList.Add(eDraw);
        }
    }

    DRAWCARDTYPE ActiveTypeProb()
    {
        int RandNum = Random.Range(1, 10001);

        switch (RandNum)
        {
            case <= 10: //È®·üÀÌ 0.1%
                return DRAWCARDTYPE.WEAPON;

            case int n when (11 <= n && n <= 1010): //È®·ü 10%
                return DRAWCARDTYPE.HP;

            case int n when (1011 <= n && n <= 3010): //È®·ü 20%
                return DRAWCARDTYPE.ATK;

            case int n when (3011 <= n && n <= 6010): //È®·ü 30%
                return DRAWCARDTYPE.DEF;

            case int n when (6011 <= n && n <= 10000): //È®·ü 39.9%
                return DRAWCARDTYPE.SPD;
        }

        return DRAWCARDTYPE.WEAPON;
    }

    int ActiveStatProb()
    {
        int RandNum = Random.Range(1, 10001);

        switch (RandNum)
        {
            case <= 10: //È®·üÀÌ 0.1%
                return 10;

            case int n when (11 <= n && n <= 210): //È®·ü 2%
                return 8;

            case int n when (211 <= n && n <= 710): //È®·ü 5%
                return 6;

            case int n when (711 <= n && n <= 1510): //È®·ü 8%
                return 5;

            case int n when (1511 <= n && n <= 4010): //È®·ü 25%
                return 3;

            case int n when (4011 <= n && n <= 7010): //È®·ü 30%
                return 2;

            case int n when (7011 <= n && n <= 10000): //È®·ü 29.9%
                return 1;
        }


        return 0;
    }

    int ActiveWeaponProb()
    {
        int RandNum = Random.Range(1, 10001);

        switch (RandNum)
        {
            case int n when (1 <= n && n <= 5000): //
                return 1;   //´ë°Å // È®·ü 50%

            case int n when (5001 <= n && n <= 5400): //Ä«Å¸³ª // È®·ü 4%
                return 2;

            case int n when (5401 <= n && n <= 5500): //±¤¼±°Ë // È®·ü 1%
                return 3;

            case int n when (5501 <= n && n <= 6500): //Ã¶¹æÆÐ // È®·ü 10%
                return 4;

            case int n when (6501 <= n && n <= 8500): //³ª¹«¹æÆÐ // È®·ü 20%
                return 5;

            case int n when (8501 <= n && n <= 10000): //ÇÑ¼Õ°Ë // È®·ü 15%
                return 6;
        }

        return 0;
    }

    void ApplyCardInfo(int stats, DRAWCARDTYPE i_eDraw)
    {
        switch (i_eDraw)
        {
            case DRAWCARDTYPE.HP:
                PlayerController.pc.m_sTempStat.hp = stats;
                PlayerController.pc.RefreshStatus();
                UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                    ("Ã¼·ÂÀÌ " + stats.ToString() + "¸¸Å­ ¿Ã¶ú½À´Ï´Ù.", 2f));
                break;

            case DRAWCARDTYPE.ATK:
                PlayerController.pc.m_sTempStat.atk = stats;
                PlayerController.pc.RefreshStatus();
                UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                    ("°ø°Ý·ÂÀÌ " + stats.ToString() + "¸¸Å­ ¿Ã¶ú½À´Ï´Ù.", 2f));
                break;

            case DRAWCARDTYPE.SPD:
                PlayerController.pc.m_sTempStat.spd = stats;
                PlayerController.pc.RefreshStatus();
                UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                    ("½ºÇÇµå°¡ " + stats.ToString() + "¸¸Å­ ¿Ã¶ú½À´Ï´Ù.", 2f));
                break;

            case DRAWCARDTYPE.DEF:
                PlayerController.pc.m_sTempStat.def = stats;
                PlayerController.pc.RefreshStatus();
                UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                    ("¹æ¾î·ÂÀÌ " + stats.ToString() + "¸¸Å­ ¿Ã¶ú½À´Ï´Ù.", 2f));
                break;  

            case DRAWCARDTYPE.WEAPON:
                InventoryManager.im.InputInvenItem(LOADTYPE.WEAPON, stats);

                List<Dictionary<string, object>> weaponList = CSVManager.instance.m_dicData[LOADTYPE.WEAPON].recordDataList;

                string weaponName = weaponList[stats - 1]["NAME"].ToString();

                UIManager.instance.StartCoroutine(UIManager.instance.ActiveSystemMsg
                    (weaponName + "À»(¸¦) È¹µæÇß´Ù!", 2f));

                InventoryManager.im.SaveInvenInfo();

                break;
        }

        DrawPickCard();
        UIManager.instance.ActiveRandomItem(false);
    }
}
