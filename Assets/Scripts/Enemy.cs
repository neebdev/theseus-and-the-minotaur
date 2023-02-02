using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public Ease movementEase;
    public float timeToMove;
    public float timeToRotate;
    public float raycastLength;
    public LayerMask mazeLayer;
    private int moveCounter;
    private Stack<Vector3> dirs = new Stack<Vector3>();
    private int movesToUndo;
    
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.Instance.OnEnemyUndo += Undo;
    }

    

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        dirs.Clear();
    }
    private void GameManager_OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.EnemyTurn && !GameManager.Instance.transition)
        {
            moveCounter = 0;
            MakeMove();
        }
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
            transform.DOMove(target, timeToMove).SetEase(movementEase).OnComplete(() =>
            {
                transform.position = target;
                UndoMoves();
            });
            Rotate(dir);
            if (dirs.Count > 0)
            {
                Rotate(dir);
            }
            else
            {
                Rotate(Vector2.zero);
            }
        }
    }

    public void MakeMove()
    {
        if (relPos.x > -0.5 && relPos.x < 0.5)
        {
            if (relPos.y > 0.5)
            {
                if (CanMove(Vector2.up))
                {
                    Move(Vector2.up);
                }
                else
                {
                    EndTurn();
                }
            }
            else if (relPos.y < -0.5)
            {
                if (CanMove(Vector2.down))
                {
                    Move(Vector2.down);
                }
                else
                {
                    EndTurn();
                }
            }
        }
        if (relPos.y > -0.5 && relPos.y < 0.5)
        {
            if (relPos.x > 0.5)
            {
                if (CanMove(Vector2.right))
                {
                    Move(Vector2.right);
                }
                else
                {
                    EndTurn();
                }
            }
            else if (relPos.x < -0.5)
            {
                if (CanMove(Vector2.left))
                {
                    Move(Vector2.left);
                }
                else
                {
                    EndTurn();
                }
            }
        }
        else
        {
            if (relPos.x > 0.5)
            {
                if (relPos.y > 0.5)
                {
                    if (CanMove(Vector2.right))
                    {
                        Move(Vector2.right);
                    }
                    else if (CanMove(Vector2.up))
                    {
                        Move(Vector2.up);
                    }
                    else
                    {
                        EndTurn();
                    }
                }
                else if (relPos.y < -0.5)
                {
                    if (CanMove(Vector2.right))
                    {
                        Move(Vector2.right);
                    }
                    else if (CanMove(Vector2.down))
                    {
                        Move(Vector2.down);
                    }
                    else
                    {
                        EndTurn();
                    }
                }
            }
            else if (relPos.x < -0.5)
            {
                if (relPos.y > 0.5)
                {
                    if (CanMove(Vector2.left))
                    {
                        Move(Vector2.left);
                    }
                    else if (CanMove(Vector2.up))
                    {
                        Move(Vector2.up);
                    }
                    else
                    {
                        EndTurn();
                    }
                }
                else if (relPos.y < -0.5)
                {
                    if (CanMove(Vector2.left))
                    {
                        Move(Vector2.left);
                    }
                    else if (CanMove(Vector2.down))
                    {
                        Move(Vector2.down);
                    }
                    else
                    {
                        EndTurn();
                    }
                }
            }
        }
    }
    private void Move(Vector3 dir)
    {
        Vector3 target = transform.position + dir;
        if (!DOTween.IsTweening(transform.position))
        {
            transform.DOMove(target, timeToMove).SetEase(movementEase).OnStart(() => {
                GameManager.Instance.enemyMoves++;
                dirs.Push(dir);
            }).OnComplete(() =>
            {
                
                transform.position = target;
                EndMove();
                
            });
            Rotate(dir);
        }
    }
    public void EndMove()
    {
        moveCounter++;
        if(moveCounter < 2)
        {
            MakeMove();
        }
        else
        {
            EndTurn();
        }
    }
    private void EndTurn()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurn);
        GameManager.Instance.OnCycleEnd();
    }
    private void Rotate(Vector2 dir)
    {
        float angle = dir.x * -90;
        if (dir.y < 0)
        {
            angle += 180;
        }
        transform.DORotate(new Vector3(0f, 0f, angle), timeToRotate, RotateMode.Fast).SetEase(Ease.Linear);
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
    
    private Vector2 relPos
    {
        get
        {
            Vector3 vector = GameManager.Instance.playerPos - transform.position;
            Vector2 resultant = Vector2.zero;
            resultant.x = Vector2.Dot(vector, Vector2.right);
            resultant.y = Vector2.Dot(vector, Vector2.up);
            return resultant;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            
        }
    }
}
