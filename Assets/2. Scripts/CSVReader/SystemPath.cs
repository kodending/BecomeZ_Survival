using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SystemPath
{
    public static string GetPath(string fileName)
    {
        //�÷������� �����ġ�� �ٲ�� ������ �װ� ���߱� ����
        string path = GetPath();
        return Path.Combine(GetPath(), fileName);
    }

    public static string GetPath()
    {
        //����ġ������ �÷������� �´� ��� ����
        string path = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(Application.persistentDataPath, "Resources/");
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Assets", "Resources/");
            case RuntimePlatform.WindowsEditor:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Assets", "Resources/");
            default:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Resources/");
        }
    }
}
