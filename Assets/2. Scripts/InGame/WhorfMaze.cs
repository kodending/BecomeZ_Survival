using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhorfMaze : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //원래 그전에 메세지창 한번 띄운다.
            

            //메이즈 러너 시작
            StartCoroutine(MazeGenerator.mg.InitMaze());
        }
    }
}
