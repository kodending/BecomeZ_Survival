using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerController.pc.m_isWallCheck = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PlayerController.pc.m_isWallCheck = false;
        }
    }
}
