using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private LevelPropertiesScriptableObect Levels;
    [SerializeField] private int CurrentLevel = 0;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        CurrentLevel = PlayerPrefs.GetInt("Level", 0);
    }

    public LevelData GetLevelData()
    {
        return Levels.LevelDatas[CurrentLevel % Levels.LevelDatas.Length];
    }

    public void OpenNextLevelMenu()
    {
        UIManager.Instance.ActivateNextLevelMenu();
    }

    public void OpenRestartMenu()
    {
        UIManager.Instance.ActivateRestartLevelMenu();
    }

    public void NextLevel()
    {
        CurrentLevel++;
        UIManager.Instance.SetLevelText(CurrentLevel + 1);
        GridManager.Instance.RegenerateGrid();
        PlayerPrefs.SetInt("Level", CurrentLevel);
    }

    public void RestartLevel()
    {
        GridManager.Instance.RegenerateGrid();
    }
    public int GetCurrentLevel()
    {
        return CurrentLevel;
    }
}
