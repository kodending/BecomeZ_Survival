using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhorfMaze : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //���� ������ �޼���â �ѹ� ����.
            

            //������ ���� ����
            StartCoroutine(MazeGenerator.mg.InitMaze());
        }
    }
}
