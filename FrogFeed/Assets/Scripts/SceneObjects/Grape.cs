using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grape : MonoBehaviour
{
    [SerializeField] private Vector3 directionVector;
    [SerializeField] private Vector3 destinationPosition;
    public void GoToFrog(List<Vector3> _MovementPositions, float _Speed)
    {
        if (!gameObject.activeSelf)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
        }
        transform.parent = null;
        StartCoroutine(Move(_MovementPositions, _Speed));
    }
    IEnumerator Move(List<Vector3> _MovementPositions, float _Speed)
    {
        _MovementPositions[_MovementPositions.Count - 1] = new Vector3(_MovementPositions[_MovementPositions.Count - 1].x, transform.position.y, _MovementPositions[_MovementPositions.Count - 1].z);
        directionVector = _MovementPositions[_MovementPositions.Count - 1] - transform.position;
        destinationPosition = transform.position + directionVector;
        bool decreasePositionCount = false;
        while (true)
        {
            if (decreasePositionCount)
            {
                _MovementPositions.RemoveAt(_MovementPositions.Count - 1);
                decreasePositionCount = false;
                if (_MovementPositions.Count == 0)
                {
                    Destroy(gameObject);
                    break;
                }
                _MovementPositions[_MovementPositions.Count - 1] = new Vector3(_MovementPositions[_MovementPositions.Count - 1].x, transform.position.y, _MovementPositions[_MovementPositions.Count - 1].z);
                directionVector = _MovementPositions[_MovementPositions.Count - 1] - transform.position;
                destinationPosition = transform.position + directionVector;
            }
            transform.position += directionVector * _Speed * Time.deltaTime;
            if (_MovementPositions.Count == 1)
            {
                transform.localScale -= Vector3.one * _Speed * Time.deltaTime;
                if (transform.localScale.x < 0)
                {
                    transform.localScale = Vector3.zero;
                }
            }
            if ((transform.position - destinationPosition).sqrMagnitude <= 0.01f)
            {
                transform.position = destinationPosition;
                decreasePositionCount = true;
            }
            yield return null;
        }
    }

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
