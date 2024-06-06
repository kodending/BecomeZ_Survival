using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//공격 계산하는 스크립트

public class AttackCal : MonoBehaviour
{
    public static AttackCal ac;

    private void Awake()
    {
        ac = this;
    }

    //공격계산
    public int DamageCalculate(int i_iAtk, int i_iDef, bool i_bDefense)
    {
        int iDamage = 0;    //계산된 데미지
        double dDef = 0;    //계산된 방어력

        //방어인지확인
        if (i_bDefense)
            dDef = Mathf.Round(i_iDef * 1.5f); //반올림
        else
            dDef = i_iDef;

        iDamage = i_iAtk - (int)dDef;

        if (iDamage <= 0) iDamage = 1;

        return iDamage;
    }
}
