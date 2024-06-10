using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class FrogProperty
{
    public int frogDirection;
    public Vector3 frogDirectionVector;
    public EColor frogColor;

    public FrogProperty(int _FrogDirection, EColor _FrogColor)
    {
        frogDirection = _FrogDirection;
        if (_FrogDirection == 0)
            frogDirectionVector = Vector3.forward;
        else if (_FrogDirection == 1)
            frogDirectionVector = Vector3.right;
        else if (_FrogDirection == 2)
            frogDirectionVector = Vector3.back;
        else
            frogDirectionVector = Vector3.left;
        frogColor = _FrogColor;
    }
}

public class Frog : MonoBehaviour
{
    //[SerializeField] private Vector2[] TonguePositions;
    [SerializeField] private LineRenderer tongue;
    private BoxCollider boxCollider;
    [SerializeField] private float tongueSpeed = 2.5f;
    [SerializeField] private Vector3 destinationPosition;
    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private FrogProperty property;
    [SerializeField] private Vector3 tongueDirectionVector;
    [SerializeField] private List<Vector3> controlTonguePositions;
    [SerializeField] private List<Vector3> CollectedGrapes;
    [SerializeField] private int MoveStartedGrapeCount;

    List<Vector3> positions;
    float DestinationY;

    public void SetProperty(FrogProperty _Property)
    {
        property = _Property;
        Material[] materials = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials;
        materials[0].SetTexture("_MainTex", GridManager.Instance.GetObjectProperties().CellDatas[((int)property.frogColor)].FrogColor);
        transform.Rotate(Vector3.up, property.frogDirection * 90);
        tongueDirectionVector = property.frogDirectionVector;

        tongue = GetComponent<LineRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void TongueLaunch()
    {
        boxCollider.enabled = false;
        tongue.positionCount = 1;
        tongue.SetPosition(0, transform.position + new Vector3(0, 0.165f, 0));
        currentPosition = transform.position + new Vector3(0, 0.165f, 0);
        tongueDirectionVector = property.frogDirectionVector;
        destinationPosition = currentPosition + tongueDirectionVector;
        StartCoroutine(GoTongue());
        GridManager.Instance.DecreaseMoveCount();
    }

    IEnumerator CollectGrapes()
    {
        float distanceControl = 0.45f * 0.45f;
        Grape lastGrape;
        Node ControlNode;
        if (CollectedGrapes.Count > 0)
        {
            ControlNode = GridManager.Instance.GetNode(Mathf.FloorToInt(currentPosition.x) * GridManager.Instance.GetGridCount() + Mathf.FloorToInt(currentPosition.z));
            positions = new List<Vector3>();
            for (int i = 0; i < controlTonguePositions.Count - 1; i++)
            {
                positions.Add(controlTonguePositions[i]);
            }
            lastGrape = ControlNode.GetLastCell().GetGrape();
            lastGrape.GoToFrog(positions, tongueSpeed + 0.25f);
            MoveStartedGrapeCount++;

            while (CollectedGrapes.Count > 1)
            {
                Vector3 controlCollectedGrape = CollectedGrapes[CollectedGrapes.Count - 1 - MoveStartedGrapeCount];
                if ((lastGrape.transform.position - controlCollectedGrape).sqrMagnitude <= distanceControl)
                {
                    ControlNode = GridManager.Instance.GetNode(Mathf.FloorToInt(controlCollectedGrape.x) * GridManager.Instance.GetGridCount() + Mathf.FloorToInt(controlCollectedGrape.z));
                    positions = new List<Vector3>();
                    for (int i = 0; i < controlTonguePositions.Count; i++)
                    {
                        if (controlTonguePositions[i].x == controlCollectedGrape.x && controlTonguePositions[i].z == controlCollectedGrape.z)
                        {
                            break;
                        }
                        positions.Add(controlTonguePositions[i]);
                    }
                    lastGrape = ControlNode.GetLastCell().GetGrape();
                    lastGrape.GoToFrog(positions, tongueSpeed + 0.25f);
                    MoveStartedGrapeCount++;
                    if (CollectedGrapes.Count == MoveStartedGrapeCount)
                    {
                        break;
                    }
                }
                yield return null;
            }
        }
    }

    IEnumerator ReturnTongue(bool _CollectedGrapes)
    {
        tongueDirectionVector = tongue.GetPosition(tongue.positionCount - 2) - currentPosition;
        destinationPosition = currentPosition + tongueDirectionVector;
        bool DecreasePositionCount = false;
        bool CellMinimize = true;
        Vector2 objectToBeDestroy = new Vector2(currentPosition.x, currentPosition.z);
        float distance;
        float distanceControl = 0.35f * 0.35f;
        while (true)
        {
            if (DecreasePositionCount)
            {
                tongue.positionCount -= 1;
                DecreasePositionCount = false;
                if (tongue.positionCount == 1)
                {
                    if(_CollectedGrapes)
                    {
                        GridManager.Instance.GetNode(Mathf.FloorToInt(transform.position.x) * GridManager.Instance.GetGridCount() + Mathf.FloorToInt(transform.position.z)).RemoveLastCell();
                    }
                    else
                    {
                        boxCollider.enabled = true;
                        GridManager.Instance.DecreaseActiveTongueFrogCount();
                    }
                    break;
                }
                tongueDirectionVector = tongue.GetPosition(tongue.positionCount - 2) - currentPosition;
                destinationPosition = currentPosition + tongueDirectionVector;
            }
            currentPosition += tongueDirectionVector * tongueSpeed * Time.deltaTime;
            tongue.SetPosition(tongue.positionCount - 1, currentPosition);

            distance = (currentPosition - destinationPosition).sqrMagnitude;

            if (_CollectedGrapes && CellMinimize == true && distance <= distanceControl)
            {
                CellMinimize = false;
                GridManager.Instance.GetNode(Mathf.FloorToInt(objectToBeDestroy.x) * GridManager.Instance.GetGridCount() + Mathf.FloorToInt(objectToBeDestroy.y)).RemoveLastCell();
            }

            if (distance <= 0.03f)
            {
                tongue.SetPosition(tongue.positionCount - 1, destinationPosition);
                currentPosition = destinationPosition;
                objectToBeDestroy = new Vector2(currentPosition.x, currentPosition.z);
                DecreasePositionCount = true;
                CellMinimize = true;
            }
            yield return null;
        }
    }

    IEnumerator GoTongue()
    {
        bool IncreasePositionCount = true;
        bool bIsSetY = false;
        Node ControlNode;
        GridManager.Instance.IncreaseActiveTongueFrogCount();
        while (true)
        {
            if (IncreasePositionCount)
            {
                if (destinationPosition.x > GridManager.Instance.GetGridCount() - 1 ||
                    destinationPosition.z > GridManager.Instance.GetGridCount() - 1 ||
                    destinationPosition.x < 0 ||
                    destinationPosition.z < 0)
                {
                    for (int i = 0; i < tongue.positionCount; i++)
                    {
                        controlTonguePositions.Add(tongue.GetPosition(i));
                    }
                    StartCoroutine(CollectGrapes());
                    StartCoroutine(ReturnTongue(true));
                    break;
                    //The process of picking grapes and returning
                }
                ControlNode = GridManager.Instance.GetNode(Mathf.FloorToInt(destinationPosition.x) * GridManager.Instance.GetGridCount() + Mathf.FloorToInt(destinationPosition.z));
                if (ControlNode.GetLastCell() == null)
                {
                    for (int i = 0; i < tongue.positionCount; i++)
                    {
                        controlTonguePositions.Add(tongue.GetPosition(i));
                    }
                    StartCoroutine(CollectGrapes());
                    StartCoroutine(ReturnTongue(true));
                    //The process of picking grapes and returning
                    break;
                }
                CellProperty cellProperty = ControlNode.GetLastCell().GetCellProperty();
                Cell ControlCell = ControlNode.ControlCellColor(property.frogColor);
                if (!ControlCell || (ControlCell && ControlCell.GetCellProperty().Type == EType.Frog))
                {
                    for (int i = 0; i < tongue.positionCount; i++)
                    {
                        controlTonguePositions.Add(tongue.GetPosition(i));
                    }
                    StartCoroutine(CollectGrapes());
                    StartCoroutine(ReturnTongue(true));
                    break;
                    //The process of picking grapes and returning
                }
                tongue.positionCount += 1;
                DestinationY = ControlCell.GetCellHeight();
                IncreasePositionCount = false;
                bIsSetY = false;
            }
            currentPosition += tongueDirectionVector * tongueSpeed * Time.deltaTime;
            if (!bIsSetY)
            {
                float yDistance = DestinationY - currentPosition.y;
                if (yDistance < -0.1f)
                {
                    currentPosition += Vector3.down * tongueSpeed * 1 * Time.deltaTime;
                }
                else if (yDistance > 0.1f)
                {
                    currentPosition += Vector3.up * tongueSpeed * 1 * Time.deltaTime;
                }
                else
                {
                    bIsSetY = true;
                    currentPosition = new Vector3(currentPosition.x, DestinationY, currentPosition.z);
                }
            }
            tongue.SetPosition(tongue.positionCount - 1, currentPosition);
            Vector2 distControl = new Vector2(destinationPosition.x - currentPosition.x, destinationPosition.z - currentPosition.z);
            if (distControl.sqrMagnitude <= 0.01f)
            {
                currentPosition = destinationPosition;
                IncreasePositionCount = true;

                Node CurrentNode = GridManager.Instance.GetNode(Mathf.FloorToInt(currentPosition.x) * GridManager.Instance.GetGridCount() + Mathf.FloorToInt(currentPosition.z));
                CellProperty cellProperty = CurrentNode.GetLastCell().GetCellProperty();
                tongue.SetPosition(tongue.positionCount - 1, new Vector3(destinationPosition.x, CurrentNode.GetLastCell().GetCellHeight(), destinationPosition.z));

                if (cellProperty.Type == EType.Grape)
                {
                    if (cellProperty.Color == property.frogColor)
                    {
                        destinationPosition = currentPosition + tongueDirectionVector;
                        CollectedGrapes.Add(currentPosition);
                    }
                    else
                    {
                        StartCoroutine(ReturnTongue(false));
                        break;
                        //the process of returning without picking the grapes
                    }
                }
                else if (cellProperty.Type == EType.Frog)
                {
                    StartCoroutine(ReturnTongue(false));
                    break;
                    //the process of returning without picking the grapes
                }
                else if (cellProperty.Type == EType.Arrow && cellProperty.Color == property.frogColor)
                {
                    tongueDirectionVector = cellProperty.GetDirection();
                    destinationPosition = currentPosition + tongueDirectionVector;
                }
                else
                {
                    StartCoroutine(ReturnTongue(false));
                    break;
                    //the process of returning without picking the grapes
                }
            }
            yield return null;
        }
    }

    public void Maximize(float _ScalingSpeed)
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        StartCoroutine(MaximizeLoop(_ScalingSpeed));
        GetComponent<FrogEye>().StartEyeOpenClose();
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
