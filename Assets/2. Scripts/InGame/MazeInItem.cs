using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeInItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //�÷��̾�� �ε�����
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //UI â ����
            //������Ʈ ���ְ�
            //�ð����߰�

            UIManager.instance.ActiveRandomItem(true);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 100f * Time.deltaTime, 0));
    }
}
