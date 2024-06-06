using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager dtm;

    public GameObject TextPrefab;

    private void Awake()
    {
        dtm = this;
    }

    public void SpawnText(Vector3 spawnPoint, string text)
    {
        GameObject spawnedText = Instantiate(TextPrefab);
        spawnedText.transform.position = spawnPoint;
        spawnedText.GetComponent<MoveDamageText>().SetupText(text, spawnPoint);
    }

}
