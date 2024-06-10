using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeProperty
{
    public CellProperty[] CellProperties;
    public NodeProperty(CellProperty[] _CellProperties)
    {
        CellProperties = _CellProperties;
    }
}

public class Node : MonoBehaviour
{
    [SerializeField] private List<Cell> CellObjects;
    [SerializeField] private float CellPlacementDistance = 0.09f;
    [SerializeField] private GameObject CellPrefab;

    public void SetProperties(NodeProperty _NodeProperty)
    {
        for (int i = 0; i < _NodeProperty.CellProperties.Length; i++)
        {
            Cell NewCell = Instantiate(CellPrefab, transform).GetComponent<Cell>();
            CellObjects.Add(NewCell);
            if (i < _NodeProperty.CellProperties.Length - 1)
                _NodeProperty.CellProperties[i].Active = false;
            else
                _NodeProperty.CellProperties[i].Active = true;
            NewCell.SetCellProperty(_NodeProperty.CellProperties[i]);
            NewCell.transform.parent = transform;
            NewCell.transform.localPosition += new Vector3(0, CellPlacementDistance * (i + 1), 0);
        }
    }

    public Cell GetLastCell() 
    {
        if (CellObjects.Count == 0)
            return null;
        return CellObjects[CellObjects.Count - 1]; 
    }
    public Cell ControlCellColor(EColor _ControlColor) 
    {
        for (int i = 0; i < CellObjects.Count; i++) 
        {
            if (CellObjects[i].GetCellProperty().Color == _ControlColor)
            {
                return CellObjects[i];
            }
        }
        return null;
    }

    public void RemoveLastCell()
    {
        if (CellObjects.Count >= 2)
        {
            StartCoroutine(CellObjects[CellObjects.Count - 1].MinimizeAndDestroy(CellObjects[CellObjects.Count - 2],GridManager.Instance.GetScalingSpeed()));
        }
        else
        {
            StartCoroutine(CellObjects[CellObjects.Count - 1].MinimizeAndDestroy(null, GridManager.Instance.GetScalingSpeed()));
        }
        CellObjects.RemoveAt(CellObjects.Count - 1);
    }
}
