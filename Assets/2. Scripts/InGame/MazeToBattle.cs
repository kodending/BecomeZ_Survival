using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeToBattle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManagerInGame.gm.StartBattle();
        }
    }
}
