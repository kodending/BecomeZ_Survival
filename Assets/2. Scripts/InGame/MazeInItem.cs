using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeInItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //플레이어랑 부딪히면
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //UI 창 띄우고
            //오브젝트 없애고
            //시간멈추고

            UIManager.instance.ActiveRandomItem(true);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 100f * Time.deltaTime, 0));
    }
}
