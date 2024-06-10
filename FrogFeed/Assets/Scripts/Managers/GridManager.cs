using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private ObjectPropertiesScriptableObject ObjectProperties;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Vector3 defaultCameraPos = new Vector3(1, 4.72f, -2.25f);
    [SerializeField] private GameObject NodeObject;
    [SerializeField] private int MaxMoveCount;
    [SerializeField] private int CurrentMoveCount;
    [SerializeField, Range(3, 6)] private int GridCount;
    [SerializeField, ReadOnly] private float XIncreaseCount = 0.5f;
    [SerializeField, ReadOnly] private float ZIncreaseCount = 0.75f;
    [SerializeField, ReadOnly] private float ScalingSpeed = 3.0f;
    [SerializeField, ReadOnly] private List<Node> Nodes;
    [SerializeField, ReadOnly] private int FrogCount = 0;
    [SerializeField, ReadOnly] private int ActiveTongueFrogCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        RegenerateGrid();
        UIManager.Instance.SetLevelText(GameManager.Instance.GetCurrentLevel() + 1);
    }

    public void RegenerateGrid()
    {
        if (Nodes.Count > 0)
        {
            int nodeCount = GridCount * GridCount;
            for (int i = 0; i < nodeCount; i++)
            {
                Destroy(Nodes[i].gameObject);
            }
            Nodes.Clear();
            FrogCount = 0;
            MainCamera.transform.position = defaultCameraPos;
        }
        MaxMoveCount = GameManager.Instance.GetLevelData().MaxMoveCount;
        CurrentMoveCount = MaxMoveCount;
        UIManager.Instance.SetMoveText(CurrentMoveCount);
        GridCount = GameManager.Instance.GetLevelData().GridCount;
        MainCamera.orthographicSize = GridCount + 2.0f;
        MainCamera.transform.position = MainCamera.transform.position + new Vector3((GridCount - 3) * XIncreaseCount, 0, (GridCount - 3) * ZIncreaseCount);
        for (int x = 0; x < GridCount; x++)
        {
            for (int z = 0; z < GridCount; z++)
            {
                Node NewNode = Instantiate(NodeObject, new Vector3(x, 0, z), Quaternion.identity).GetComponent<Node>();
                Nodes.Add(NewNode);
                NewNode.SetProperties(GameManager.Instance.GetLevelData().Nodes[x * GridCount + z]);
            }
        }
    }

    public void IncreaseActiveTongueFrogCount()
    {
        ActiveTongueFrogCount++;
    }

    public void DecreaseActiveTongueFrogCount()
    {
        ActiveTongueFrogCount--;
        if (ActiveTongueFrogCount == 0 && (CurrentMoveCount < FrogCount && CurrentMoveCount != 0))
        {
            GameManager.Instance.OpenRestartMenu();
        }
    }
    public void IncreaseFrogCount()
    {
        FrogCount++;
    }

    public void DecreaseFrogCount()
    {
        FrogCount--;
        if (FrogCount == 0)
        {
            GameManager.Instance.OpenNextLevelMenu();
        }
    }

    public void DecreaseMoveCount()
    {
        CurrentMoveCount--;
        UIManager.Instance.SetMoveText(CurrentMoveCount);
    }

    public int GetCurrentMoveCount() { return CurrentMoveCount; }
    public int GetFrogCount() { return FrogCount; }

    public ObjectPropertiesScriptableObject GetObjectProperties() { return ObjectProperties; }
    public Node GetNode(int _Index) { return Nodes[_Index]; }
    public int GetGridCount() { return GridCount; }
    public float GetScalingSpeed() { return ScalingSpeed; }
}
