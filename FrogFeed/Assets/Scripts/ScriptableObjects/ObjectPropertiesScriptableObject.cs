using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CellData
{
    public Texture2D CellColor;
    public Sprite ArrowColor;
    public Texture2D FrogColor;
    public Texture2D GrapeColor;

    public CellData(Texture2D _CellColor, Sprite _ArrowColor, Texture2D _FrogColor, Texture2D _GrapeColor)
    {
        CellColor = _CellColor;
        ArrowColor = _ArrowColor;
        FrogColor = _FrogColor;
        GrapeColor = _GrapeColor;
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectPropertiesScriptableObject", order = 1)]
public class ObjectPropertiesScriptableObject : ScriptableObject
{
    [Header("Cells")]
    public CellData[] CellDatas;
}
