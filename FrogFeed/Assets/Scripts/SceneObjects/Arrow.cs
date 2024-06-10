using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public void Maximize(float _ScalingSpeed)
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        StartCoroutine(MaximizeLoop(_ScalingSpeed));
    }

    public IEnumerator MaximizeLoop(float _ScalingSpeed)
    {
        while (true)
        {
            if (transform.localScale.x < 1.0f)
                transform.localScale += Vector3.one * Time.deltaTime;
            else
            {
                transform.localScale = Vector3.one;
                break;
            }
            yield return null;
        }
    }
}
