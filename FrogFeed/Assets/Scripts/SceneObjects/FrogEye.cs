using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEye : MonoBehaviour
{
    SkinnedMeshRenderer m_Renderer;
    bool bIsOpen = true;
    [SerializeField] private float EyeOpenCloseSpeed = 100;
    private void Awake()
    {
        m_Renderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
    }

    public void StartEyeOpenClose()
    {
        StartCoroutine(EyeOpenClose());
    }
    IEnumerator EyeOpenClose()
    {
        while (true)
        {
            if (bIsOpen == true)
            {
                if (m_Renderer.GetBlendShapeWeight(0) < 100.0f)
                {
                    m_Renderer.SetBlendShapeWeight(0, m_Renderer.GetBlendShapeWeight(0) + EyeOpenCloseSpeed * Time.deltaTime);
                }
                else
                {
                    m_Renderer.SetBlendShapeWeight(0, 100);
                    bIsOpen = false;
                }
            }
            else
            {
                if (m_Renderer.GetBlendShapeWeight(0) > 0.0f)
                {
                    m_Renderer.SetBlendShapeWeight(0, m_Renderer.GetBlendShapeWeight(0) - EyeOpenCloseSpeed * Time.deltaTime);
                }
                else
                {
                    m_Renderer.SetBlendShapeWeight(0, 0);
                    bIsOpen = true;
                    yield return new WaitForSeconds(Random.Range(3,7));
                }
            }
            yield return null;
        }
    }
}
