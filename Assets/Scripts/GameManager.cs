using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public GameObject maskCircle;
    public Level currentLevel;
    public Level transitionLevel;
    public Vector3 playerPos;
    public bool transition;
    private int transitionIndex;
    public int playerMoves = 0;
    public int enemyMoves = 0;
    public Stack<int> nPlayerMoves = new Stack<int>();
    public Stack<int> nEnemyMoves = new Stack<int>();
    public delegate void PlayerUndo(int nMoves);
    public event PlayerUndo OnPlayerUndo;
    public delegate void EnemyUndo(int nMoves);
    public event PlayerUndo OnEnemyUndo;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Player.OnPlayerChangePos += Player_OnEndMove;
        if (string.IsNullOrEmpty(SerializationManager.Load("currentLevel")))
        {
            SerializationManager.Save("currentLevel", 1);
        }
        //if (string.IsNullOrEmpty(SerializationManager.Load("maxLevel")))
        //{
        //    SerializationManager.Save("maxLevel", int.Parse(SerializationManager.Load("currentLevel")));

        //}
        UIManager.OnUndo += OnUndo;
    }


    private void OnDestroy()
    {
        Player.OnPlayerChangePos -= Player_OnEndMove;
        UIManager.OnUndo -= OnUndo;
    }



    private void Start()
    {
        UpdateGameState(GameState.PlayerTurn);
        int level = int.Parse(SerializationManager.Load("currentLevel"));
        LoadInitialLevel(level);
    }

    public void OnCycleEnd()
    {
        if((playerMoves > 0 || enemyMoves > 0))
        {
            UIManager.Instance.IncrementUndo();
            nPlayerMoves.Push(playerMoves);
            nEnemyMoves.Push(enemyMoves);
            playerMoves = 0;
            enemyMoves = 0;
        }
    }
    public void OnUndo()
    {
        UIManager.Instance.DecrementUndo();
        OnPlayerUndo?.Invoke(nPlayerMoves.Pop());
        OnEnemyUndo?.Invoke(nEnemyMoves.Pop());
    }

    public void LoadInitialLevel(int level)
    {
        SceneManager.LoadScene(level);
        StartCoroutine(FirstSceneLoad(level));
    }
    IEnumerator FirstSceneLoad(int level)
    {
        if (!SceneManager.GetSceneByBuildIndex(level).isLoaded)
        {
            yield return null;
        }
        UIManager.Instance.SetArrows();
        currentLevel.SetLevelNumberText(level);
        currentLevel.SetLevelInteractionNone();
    }

    private void Player_OnEndMove(Vector3 obj)
    {

        playerPos = obj;
    }
    private void ResetUndos()
    {
        playerMoves = 0;
        enemyMoves = 0;
        nPlayerMoves.Clear();
        nEnemyMoves.Clear();
    }
    
    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Win:
                if (SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1)
                {
                    //SerializationManager.Save("maxLevel", SceneManager.GetActiveScene().buildIndex + 1);
                    TranstionTo(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    TranstionTo(1);
                }
                break;
            case GameState.Loose:
                StartCoroutine(ReloadLevel(0.5f));
                break;
            case GameState.Retry:
                TranstionTo(SceneManager.GetActiveScene().buildIndex);
                break;
            default:
                break;
        }
        OnGameStateChanged?.Invoke(newState);
    }
    public void TranstionTo(int levelIndex)
    {
        ResetUndos();
        transition = true;
        transitionIndex = levelIndex;
        SerializationManager.Save("currentLevel", levelIndex);
        currentLevel.SetVisibleOutside();
        maskCircle.transform.position = playerPos;
        AsyncOperation loadLevelOperation = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);
        loadLevelOperation.completed += LoadLevelOperation_completed;
    }

    public void LoadLevelOperation_completed(AsyncOperation obj)
    {
        float scale = GetCircleScale(maskCircle.transform.position, CalculateOrthoSize(transitionLevel.tilemap, transitionLevel.expand));
        CameraManager.Instance.SetCamera(transitionLevel.tilemap, transitionLevel.expand);
        maskCircle.transform.DOScale(scale, 0.6f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            
            AsyncOperation unloadLevelOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            unloadLevelOperation.completed += UnloadLevelOperation_completed;
        });
    }

    public void UnloadLevelOperation_completed(AsyncOperation obj)
    {
        UIManager.Instance.SetArrows();
        currentLevel = transitionLevel;
        currentLevel.SetNone();
        float scale = GetCircleScale(playerPos);
        maskCircle.transform.position = playerPos;
        maskCircle.transform.localScale = new Vector3(scale, scale, scale);
        maskCircle.transform.DOScale(Vector3.zero, 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transition = false;
            UpdateGameState(GameState.PlayerTurn);
        });
    }

    public void SetLevel(Level level)
    {
        if (transition)
        {
            transitionLevel = level;
            transitionLevel.SetLevelNumberText(transitionIndex);
        }
        else
        {
            currentLevel = level;
            level.SetNone();
        }
    }
    private float CalculateOrthoSize(Tilemap tilemap, float buffer)
    {
        var bound = new Bounds();
        tilemap.CompressBounds();
        var tilemapBounds = tilemap.localBounds;
        bound.Encapsulate(tilemapBounds);
        bound.Expand(buffer);
        var vertical = bound.size.y;
        var horizontal = bound.size.x * CameraManager.Instance.cam.pixelHeight / CameraManager.Instance.cam.pixelWidth;
        var size = Mathf.Max(vertical, horizontal) * 0.5f;
        var center3d = bound.center + tilemap.transform.position;
        var center2d = new Vector3(center3d.x, center3d.y, -10);
        return size;
    }
    private IEnumerator ReloadLevel(float wait)
    {
        
        yield return new WaitForSeconds(wait);
        TranstionTo(SceneManager.GetActiveScene().buildIndex);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateGameState(GameState.Retry);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UpdateGameState(GameState.EnemyTurn);
        }

    }

    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        Win,
        Loose,
        Retry,
    }
    float GetCircleScale(Vector2 pos, float orthoSize)
    {
        Vector2 cameraExtends = new Vector2(orthoSize * 1080 / 1920, orthoSize);
        Vector2 topRight = cameraExtends;
        Vector2 topLeft = new Vector2(-cameraExtends.x, cameraExtends.y);
        Vector2 bottomRight = new Vector2(cameraExtends.x, -cameraExtends.y);
        Vector2 bottomLeft = new Vector2(-cameraExtends.x, -cameraExtends.y);
        float distTopRight = Vector2.Distance(topRight, pos);
        float distTopLeft = Vector2.Distance(topLeft, pos);
        float distBottomRight = Vector2.Distance(bottomRight, pos);
        float distBottonLeft = Vector2.Distance(bottomLeft, pos);
        float maxDist = Mathf.Max(Mathf.Max(distTopLeft, distTopRight), Mathf.Max(distBottonLeft, distBottomRight));
        return (maxDist / 2.5f) + 0.8f;
    }
    float GetCircleScale(Vector2 pos)
    {
        Vector2 cameraExtends = new Vector2(CameraManager.Instance.cam.orthographicSize * 1080 / 1920, CameraManager.Instance.cam.orthographicSize);
        Vector2 topRight = cameraExtends;
        Vector2 topLeft = new Vector2(-cameraExtends.x, cameraExtends.y);
        Vector2 bottomRight = new Vector2(cameraExtends.x, -cameraExtends.y);
        Vector2 bottomLeft = new Vector2(-cameraExtends.x, -cameraExtends.y);
        float distTopRight = Vector2.Distance(topRight, pos);
        float distTopLeft = Vector2.Distance(topLeft, pos);
        float distBottomRight = Vector2.Distance(bottomRight, pos);
        float distBottonLeft = Vector2.Distance(bottomLeft, pos);
        float maxDist = Mathf.Max(Mathf.Max(distTopLeft, distTopRight), Mathf.Max(distBottonLeft, distBottomRight));
        return (maxDist / 2.5f) + 0.8f;
    }
}