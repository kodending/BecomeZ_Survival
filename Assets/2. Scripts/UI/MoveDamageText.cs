using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class MoveDamageText : MonoBehaviour
{
    public TextMesh text;

    public void SetupText(string Text, Vector3 pos)
    {
        text.text = Text;

        float randomPosX = Random.Range(-1f, 1f);
        float randomPosY = Random.Range(1f, 2f);
        Vector3 tragetPos = new Vector3(randomPosX, randomPosY, 0);

        transform.DOMove(pos + tragetPos, 2.0f).SetEase(Ease.OutElastic);

        StartCoroutine(KillText());
    }

    private void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }

    private void Update()
    {
        Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + dirFromCamera, Camera.main.transform.up);
    } 

    IEnumerator KillText()
    {
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
