using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Player : MonoBehaviour
{
    public Ease movementEase;
    public float timeToMove;
    public float raycastLength;
    public LayerMask mazeLayer;
    public static event Action<Vector3> OnPlayerChangePos;
    public float explosionForce;
    public GameObject[] brokenCirclePrefabs;
    private bool reachedExit;
    private GameObject broken;
    private Stack<Vector3> dirs = new Stack<Vector3>();
    private int movesToUndo;


    private void Awake()
    {
        InputManager.OnSwipe += OnSwipe;
        broken = Instantiate(brokenCirclePrefabs[UnityEngine.Random.Range(0, brokenCirclePrefabs.Length)], transform.position, Quaternion.identity);
        GameManager.Instance.OnPlayerUndo += Undo;
    }

    private void OnDestroy()
    {
        InputManager.OnSwipe -= OnSwipe;
        dirs.Clear();
    }
    private void Start()
    {
        OnPlayerChangePos(transform.position);
    }
    private void Undo(int nMoves)
    {
        movesToUndo = nMoves;
        UndoMoves();
        
    }
    private void UndoMoves()
    {
        if (movesToUndo-- > 0 && dirs.Count > 0)
        {
            Vector3 dir = dirs.Pop();
            Vector3 reverseDir = -dir;
            Vector3 target = transform.position + reverseDir;
            transform.DOMove(target, timeToMove).SetEase(movementEase).OnStart(() => UIManager.Instance.playerMoving = true).OnComplete(() =>
            {
                UIManager.Instance.playerMoving = false;
                transform.position = target;
                UndoMoves();
            });
        }
    }
    private void OnSwipe(Vector2 dir)
    {
        if (GameManager.Instance.state == GameManager.GameState.PlayerTurn && !GameManager.Instance.transition)
        {
            if (!DOTween.IsTweening(transform))
            {
                if (CanMove(dir))
                {
                    Move(dir);
                }
                else
                {
                    Punch(dir);
                }
            }
        }
    }
private void Move(Vector3 dir)
    {
        Vector3 target = transform.position + dir;
        if (!DOTween.IsTweening(transform.position))
        {
            transform.DOMove(target, timeToMove).SetEase(movementEase).OnStart(() => 
            {
                UIManager.Instance.playerMoving = true;
                GameManager.Instance.playerMoves++;
                dirs.Push(dir);
            }).OnComplete(() => 
            {
                
                transform.position = target;
                UIManager.Instance.playerMoving = false;
                EndMove();
            });
        }
    }
    private void EndMove()
    {
        OnPlayerChangePos(transform.position);
        if (!reachedExit)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.EnemyTurn);
        }
        else
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.Win);
        }
    }
    private void Punch(Vector3 dir)
    {
        transform.DOPunchPosition(dir * 0.07f, 0.15f, 1, 1, false);
    }
    private bool CanMove(Vector2 dir)
    {
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, dir, raycastLength, mazeLayer);
        if (raycast.collider != null)
        {
            return false;
        }
        return true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minotaur") && GameManager.Instance.state != GameManager.GameState.Retry && !GameManager.Instance.transition)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.Loose);
            GetComponent<SpriteRenderer>().enabled = false;
            broken.SetActive(true);
            broken.transform.position = transform.position;
            Rigidbody2D[] rbs = broken.GetComponentsInChildren<Rigidbody2D>();
            gameObject.GetComponent<SpriteRenderer>().DOColor(new Color32(227, 227, 227, 255), 0.05f).OnComplete(() => 
            {
                GetComponent<SpriteRenderer>().enabled = false;
            });
            foreach (Rigidbody2D rb in rbs)
            {
                GameManager.Instance.currentLevel.spriteRenderers.Add(rb.gameObject.GetComponent<SpriteRenderer>());
                rb.AddForce((rb.transform.position - broken.transform.position).normalized * explosionForce * UnityEngine.Random.Range(0, 2f), ForceMode2D.Impulse);
                Sequence s = DOTween.Sequence();
                s.Append(rb.gameObject.GetComponent<SpriteRenderer>().DOColor(new Color32(227, 227, 227, 255), 0.1f)).Append(rb.gameObject.GetComponent<SpriteRenderer>().DOColor(new Color32(97, 97, 97, 255), 0.1f));
            }
        }
        if (collision.gameObject.CompareTag("Exit"))
        {
            reachedExit = true;
        }
    }
}
