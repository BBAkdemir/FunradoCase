using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private GameObject restartLevelUI;
    [SerializeField] private GameObject nextLevelUI;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private Text moveText;
    [SerializeField] private Text levelText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        canvas = transform.GetChild(0).gameObject;
        restartLevelUI = canvas.transform.GetChild(0).gameObject;
        nextLevelUI = canvas.transform.GetChild(1).gameObject;
        mainUI = canvas.transform.GetChild(2).gameObject;
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        moveText = mainUI.transform.GetChild(0).GetComponent<Text>();
        levelText = mainUI.transform.GetChild(1).GetComponent<Text>();
    }

    public void ActivateRestartLevelMenu()
    {
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;
        graphicRaycaster.blockingMask = LayerMask.GetMask("Everything");
        restartLevelUI.SetActive(true);
        nextLevelUI.SetActive(false);
        mainUI.SetActive(false);
        Time.timeScale = 0;
    }

    public void ActivateNextLevelMenu()
    {
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;
        graphicRaycaster.blockingMask = LayerMask.GetMask("Everything");
        nextLevelUI.SetActive(true);
        restartLevelUI.SetActive(false);
        mainUI.SetActive(false);
        Time.timeScale = 0;
    }

    public void ActivateMainMenu()
    {
        Time.timeScale = 1;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        graphicRaycaster.blockingMask = LayerMask.GetMask("Nothing");
        mainUI.SetActive(true);
        nextLevelUI.SetActive(false);
        restartLevelUI.SetActive(false);
    }

    public void RestartLevel()
    {
        GameManager.Instance.RestartLevel();
        ActivateMainMenu();
    }
    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
        ActivateMainMenu();
    }

    public void SetMoveText(int _CurrentMoveCount)
    {
        moveText.text = _CurrentMoveCount.ToString() + " MOVES";
    }

    public void SetLevelText(int _CurrentLevel)
    {
        levelText.text = "LEVEL " + _CurrentLevel.ToString();
    }
}
