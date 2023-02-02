using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Sprite[] undoSprites;
    public Image undo;
    public Image next;
    public Image prev;
    public GameObject pLevelNameText;
    public GameObject screen;
    public static event Action OnSkip;
    public static event Action OnUndo;
    public bool playerMoving;
    public int undosAvailable;
    private void Awake()
    {
        Instance = this;
        SetUndos(0);
    }
    public void SetArrows()
    {
        //int maxLevel = int.Parse(SerializationManager.Load("maxLevel"));
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            prev.color = new Color32(64, 64, 64, 255);
        }
        else
        {
            prev.color = new Color32(116, 116, 116, 255);
        }
        if (SceneManager.GetActiveScene().buildIndex < 10)
        {
            next.color = new Color32(64, 64, 64, 255);
        }
        else
        {
            next.color = new Color32(116, 116, 116, 255);
        }
    }
    public MaskTextMeshProUGUI SpawnLevelNumber(int level)
    {
        MaskTextMeshProUGUI text =  Instantiate(pLevelNameText, screen.transform).GetComponent<MaskTextMeshProUGUI>();
        text.text = level.ToString();
        return text;
    }
    public void Retry()
    {
        if(!GameManager.Instance.transition && GameManager.Instance.state != GameManager.GameState.Loose)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.Retry);
        }
    }
    public void Skip()
    {
        if(GameManager.Instance.state == GameManager.GameState.PlayerTurn && !GameManager.Instance.transition)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.EnemyTurn);
        }
    }
    public void Undo()
    {
        if (undosAvailable > 0 && GameManager.Instance.state == GameManager.GameState.PlayerTurn && !GameManager.Instance.transition && !playerMoving)
        {
            OnUndo?.Invoke();
        }
    }
    public void IncrementUndo()
    {
        if(undosAvailable < 3)
        {
            SetUndos(++undosAvailable);
        }
    }
    public void DecrementUndo()
    {
        if(undosAvailable > 0)
        {
            SetUndos(--undosAvailable);
        }
    }
    private void SetUndos(int n)
    {
        undo.sprite = undoSprites[n];
        if (n > 0)
        {
            undo.color = new Color32(64, 64, 64, 255);
        }
        else
        { 
            undo.color = new Color32(116, 116, 116, 255);
        }
    }
    public void NextLevel()
    {
        //int maxLevel = int.Parse(SerializationManager.Load("maxLevel"));
        if (!GameManager.Instance.transition && GameManager.Instance.state != GameManager.GameState.Loose /*&& SceneManager.GetActiveScene().buildIndex < maxLevel*/)
        {
            GameManager.Instance.TranstionTo(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    public void PreviousLevel()
    {
        if (!GameManager.Instance.transition && GameManager.Instance.state != GameManager.GameState.Loose && SceneManager.GetActiveScene().buildIndex > 1)
        {
            GameManager.Instance.TranstionTo(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

}
