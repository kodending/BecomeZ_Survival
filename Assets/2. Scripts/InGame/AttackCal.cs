using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//���� ����ϴ� ��ũ��Ʈ

public class AttackCal : MonoBehaviour
{
    public static AttackCal ac;

    private void Awake()
    {
        ac = this;
    }

    //���ݰ��
    public int DamageCalculate(int i_iAtk, int i_iDef, bool i_bDefense)
    {
        int iDamage = 0;    //���� ������
        double dDef = 0;    //���� ����

        //�������Ȯ��
        if (i_bDefense)
            dDef = Mathf.Round(i_iDef * 1.5f); //�ݿø�
        else
            dDef = i_iDef;

        iDamage = i_iAtk - (int)dDef;

        if (iDamage <= 0) iDamage = 1;

        return iDamage;
    }
}
