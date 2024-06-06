using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;
using DG.Tweening.Plugins;
using System.Text;

public enum LOADTYPE
{
    MONSTER,
    WEAPON,
    ITEM,
    CHARINFO,
    INVENINFO,
    OPTION,
    MYPETINFO,
    _MAX_
}


public class CSVManager : MonoBehaviour
{
    static GameObject go;

    public static CSVManager instance;

    public static CSVManager Instance
    {
        get
        {
            //CSVManager �ν��Ͻ��� ������ �ȵǾ� ������ ����
            if (instance == null)
            {
                go = new GameObject();
                go.name = "CSVManager";
                instance = go.AddComponent<CSVManager>() as CSVManager;

                DontDestroyOnLoad(go); // Scene �̵�, �ε� �� �ı������ʴ� ������
            }

            return instance;
        }
    }

    [Tooltip("������ ��ųʸ�")]
    public Dictionary<LOADTYPE, GameData> m_dicData = new Dictionary<LOADTYPE, GameData>();

    private void Awake()
    {
        instance = this;

        LOADTYPE curDataType = LOADTYPE.MONSTER;
        while (curDataType != LOADTYPE._MAX_)
        {
            //���ӵ����� Ŭ���� �Ҵ�
            GameData data = new GameData();
            m_dicData.Add(curDataType, data);
            m_dicData[curDataType].recordDataList = CSVReader.Read(curDataType.ToString());
            m_dicData[curDataType].recordTitleList = CSVReader.ReadTitle(curDataType.ToString());

            curDataType++;
        }
    }

    //save�� ���� �˾ƾ��Ѵ�.
    public void SaveFile(LOADTYPE i_eLoadType, List<Dictionary<string, object>> i_listInfo)
    {
        List<Dictionary<int, object>> TitleList = CSVManager.instance.m_dicData[i_eLoadType].recordTitleList;

        List<string[]> data = new List<string[]>();
        string[] tempData =  new string[TitleList[0].Count];

        for (int idx = 0; idx < TitleList[0].Count; idx++)
        {
            tempData[idx] = TitleList[0][idx].ToString();
        }
        
        data.Add(tempData);

        for (int idx = 0; idx < i_listInfo.Count; idx++)
        {
            string[] alphaData = new string[TitleList[0].Count];

            for (int j = 0; j < TitleList[0].Count; j++)
            {
                alphaData[j] = i_listInfo[idx][tempData[j]].ToString();
            }

            data.Add(alphaData);
        }

        string[][] output = new string[data.Count][];

        for (int idx = 0; idx < output.Length; idx++)
        {
            output[idx] = data[idx];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }

        string filepath = SystemPath.GetPath();

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        string fileName = i_eLoadType.ToString() + ".csv";

        StreamWriter outStream = new StreamWriter(filepath + fileName, false, System.Text.Encoding.UTF8);
        //outStream = System.IO.File.CreateText(filepath + fileName);
        outStream.Write(sb);
        outStream.Close();
    }
}
