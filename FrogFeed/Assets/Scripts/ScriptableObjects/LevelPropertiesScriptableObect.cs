using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int MaxMoveCount;
    [Range(3,6)] public int GridCount;
    public NodeProperty[] Nodes;
    public LevelData(int _MaxMoveCount, int _GridCount, NodeProperty[] _Nodes)
    {
        MaxMoveCount = _MaxMoveCount;
        GridCount = _GridCount;
        Nodes = _Nodes;
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelPropertiesScriptableObect", order = 1)]
public class LevelPropertiesScriptableObect : ScriptableObject
{
    [Header("Levels")]
    public LevelData[] LevelDatas;
}
