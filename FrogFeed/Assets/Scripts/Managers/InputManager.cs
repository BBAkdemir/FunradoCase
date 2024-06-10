using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.transform.name);
                if (hit.collider != null)
                {
                    Frog touchedFrog = hit.transform.GetComponent<Frog>();
                    touchedFrog.TongueLaunch();
                    Debug.Log("Touched " + touchedFrog.transform.name);
                }
            }
        }
#else
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.transform.name);
                if (hit.collider != null)
                {
                    Frog touchedFrog = hit.transform.GetComponent<Frog>();
                    touchedFrog.TongueLaunch();
                    Debug.Log("Touched " + touchedFrog.transform.name);
                }
            }
        }
#endif
    }
}
