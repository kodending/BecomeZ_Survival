using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] //�ν����� â�� Ŭ������ ����ü ���� �����ϱ� ����

public class GameData
{
    //���� ������ ��������ϴ� ��

    //���� ������ ����Ʈȭ

    public List<Dictionary<string, object>> recordDataList = new List<Dictionary<string, object>>();

    //���� ������ ī�װ� Ÿ��Ʋ ����
    public List<Dictionary<int, object>> recordTitleList = new List<Dictionary<int, object>>();
}
