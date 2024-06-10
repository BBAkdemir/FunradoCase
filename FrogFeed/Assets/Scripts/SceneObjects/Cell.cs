using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public enum EType
{
    DeactiveCell,
    Arrow,
    Frog,
    Grape
}
[System.Serializable]
public enum EColor
{
    Blue,
    Green,
    Purple,
    Red
}
[System.Serializable]
public enum EDirection
{
    Forward,
    Right,
    Back,
    Left
}

[System.Serializable]
public class CellProperty
{
    public EType Type;
    public EColor Color;
    public EDirection Direction;
    public bool Active;

    public CellProperty(EType _Type, EColor _Color, EDirection _Direction, bool _Active)
    {
        Type = _Type;
        Color = _Color;
        Direction = _Direction;
        Active = _Active;
    }

    public Vector3 GetDirection()
    {
        if (Direction == EDirection.Forward)
            return Vector3.forward;
        else if (Direction == EDirection.Right)
            return Vector3.right;
        else if (Direction == EDirection.Back)
            return Vector3.back;
        else
            return Vector3.left;
    }
}
public class Cell : MonoBehaviour
{
    CellProperty property;
    Material[] Materials;
    [SerializeField] private Grape grape;
    [SerializeField] private Frog frog;
    [SerializeField] private Arrow arrow;
    bool bIsFrog = false;
    private void Awake()
    {
        grape = transform.GetChild(1).GetComponent<Grape>();
        frog = transform.GetChild(0).GetComponent<Frog>();
        arrow = transform.GetChild(2).GetComponent<Arrow>();
    }
    public void SetCellProperty(CellProperty _CellProperty)
    {
        property = _CellProperty;
        if (property.Type == EType.Grape)
        {
            grape.gameObject.SetActive(property.Active);
            Materials = grape.gameObject.GetComponent<MeshRenderer>().materials;
            Materials[0].SetTexture("_MainTex", GridManager.Instance.GetObjectProperties().CellDatas[((int)property.Color)].GrapeColor);

            Destroy(frog.gameObject);
            Destroy(arrow.gameObject);
        }
        else if (property.Type == EType.Frog)
        {
            GridManager.Instance.IncreaseFrogCount();
            bIsFrog = true;
            frog.gameObject.SetActive(property.Active);
            if (property.Active)
            {
                frog.GetComponent<FrogEye>().StartEyeOpenClose();
            }
            FrogProperty newFrogProperty = new FrogProperty((int)property.Direction, property.Color);
            frog.SetProperty(newFrogProperty);

            Destroy(grape.gameObject);
            Destroy(arrow.gameObject);
        }
        else if (property.Type == EType.Arrow)
        {
            arrow.gameObject.SetActive(property.Active);
            arrow.GetComponent<SpriteRenderer>().sprite = GridManager.Instance.GetObjectProperties().CellDatas[((int)property.Color)].ArrowColor;
            arrow.transform.Rotate(Vector3.back, ((int)property.Direction) * 90);

            Destroy(grape.gameObject);
            Destroy(frog.gameObject);
        }
        Materials = transform.GetComponent<MeshRenderer>().materials;
        Materials[0].SetTexture("_MainTex", GridManager.Instance.GetObjectProperties().CellDatas[((int)property.Color)].CellColor);
    }

    public CellProperty GetCellProperty() { return property; }
    public Grape GetGrape() { return grape; }
    public Frog GetFrog() { return frog; }
    public Arrow GetArrow() { return arrow; }

    public IEnumerator MinimizeAndDestroy(Cell _NextCell, float _ScalingSpeed)
    {
        while (true)
        {
            if (transform.localScale.x > 0.01f)
            {
                transform.localScale -= Vector3.one * _ScalingSpeed * Time.deltaTime;
            }
            else
            {
                if (_NextCell)
                {
                    _NextCell.Activate(_ScalingSpeed);
                }
                if (bIsFrog)
                {
                    GridManager.Instance.DecreaseFrogCount();
                    GridManager.Instance.DecreaseActiveTongueFrogCount();
                    frog.GetComponent<FrogEye>().StopAllCoroutines();
                }
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }

    public void Activate(float _ScalingSpeed)
    {
        property.Active = true;
        if (property.Type == EType.Grape)
        {
            grape.Maximize(_ScalingSpeed);
        }
        else if (property.Type == EType.Frog)
        {
            frog.Maximize(_ScalingSpeed);
        }
        else if (property.Type == EType.Arrow)
        {
            arrow.Maximize(_ScalingSpeed);
        }
    }

    public float GetCellHeight()
    {
        return transform.position.y + 0.3f;
    }
}
