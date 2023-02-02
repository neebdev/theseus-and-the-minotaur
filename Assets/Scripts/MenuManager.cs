using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public float timeToRotate;
    public Transform minotaur;
    public GameObject pInvertCircle;
    public GameObject maskCircle;
    private List<GameObject> spawnedInvertCircles = new List<GameObject>();
    public Ease invertEase;
    public Ease inEase;
    public Ease outEase;
    private ObjectPool<GameObject> invertCirclePool;
    public TMP_Text fps;
    int framesPerSecond;
    public Camera cam;
    public GameObject mainMenu;
    public GameObject creditsMenu;
    private MenuState menuState;
    
    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(FPS());
        SetMenuState(MenuState.MainMenu);
        invertCirclePool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(pInvertCircle);
        }, circle => { 
            circle.SetActive(true); 
        }, circle => {
            circle.SetActive(false);
            circle.transform.localScale = new Vector3(0, 0, 0);
        },circle => { 
            Destroy(circle);
        }, false ,20, 20);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, ray.direction);
            if (menuState == MenuState.MainMenu)
            {
                Vector2 dir = (mousePos - new Vector2(minotaur.position.x, minotaur.position.y)).normalized;
                RotateMinotaur(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                if (hit && hit.collider.name == "Credits")
                {
                    maskCircle.transform.position = hit.transform.position;
                    TransitionTo(MenuState.CreditsMenu);
                }
                else if(hit && hit.collider.name == "Play")
                {
                    maskCircle.transform.position = hit.transform.position;
                    LoadGame();
                }
                else
                {
                    SpawnInvertCircle(mousePos);
                }
            }
            else if(menuState == MenuState.CreditsMenu)
            {
                if (hit && hit.collider.name == "Back")
                {
                    TransitionTo(MenuState.MainMenu);
                }
                else
                {
                    SpawnInvertCircle(mousePos);
                }
            }
            else if(menuState == MenuState.Transition)
            {
                SpawnInvertCircle(mousePos);
            }
        }
    }
    public void SetMenuState(MenuState state)
    {
        menuState = state;
        switch (state)
        {
            case MenuState.MainMenu:
                break;
            case MenuState.CreditsMenu:
                break;
        }
    }
    public void LoadGame()
    {
        creditsMenu.SetActive(false);
        maskCircle.SetActive(true);
        SetMenuState(MenuState.Transition);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Additive);
        loadOp.completed += LoadOp_completed;
    }

    private void LoadOp_completed(AsyncOperation obj)
    {
        //cam = FindObjectOfType<CameraManager>().GetComponent<Camera>();
        //maskCircle.transform.DOScale(GetCircleScale(maskCircle.transform.position), 0.7f).SetEase(inEase).OnComplete(() =>
        //{
            
        //    //AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //    //unloadOp.completed += UnloadOp_completed;
        //});
    }

    private void UnloadOp_completed(AsyncOperation obj)
    {
        print("Unloaded");
    }

    public void TransitionTo(MenuState state)
    {
        SetMenuState(MenuState.Transition);
        maskCircle.SetActive(true);
        switch (state)
        {
            case MenuState.MainMenu:
                maskCircle.transform.DOScale(0f, 0.7f).SetEase(inEase).OnComplete(() =>
                {
                    SetMenuState(MenuState.MainMenu);
                    maskCircle.SetActive(false);
                });
                break;
            case MenuState.CreditsMenu:
                maskCircle.transform.DOScale(GetCircleScale(maskCircle.transform.position), 0.7f).SetEase(inEase).OnComplete(() =>
                {
                    SetMenuState(MenuState.CreditsMenu);
                });
                break;
        }
    }
    private IEnumerator FPS()
    {
        for (; ; ) 
        {
            int lastFrameCount = Time.frameCount;
            float lastTie = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(0.1f);
            float timeSpan = Time.realtimeSinceStartup - lastTie;
            int frameCount = Time.frameCount - lastFrameCount;
            framesPerSecond = Mathf.RoundToInt(frameCount / timeSpan);
            fps.text = framesPerSecond.ToString();
        }
    }
    private void RotateMinotaur(float angle)
    {
        minotaur.DORotate(new Vector3(0f, 0f, angle - 90f), timeToRotate, RotateMode.Fast).SetEase(Ease.Linear);
    }
    private void SpawnInvertCircle(Vector2 pos)
    {
        GameObject spawnedCircle = invertCirclePool.Get();
        spawnedCircle.transform.position = pos;


        spawnedCircle.transform.DOScale(GetCircleScale(pos), 1.2f).SetEase(invertEase).OnComplete(() =>
        {
            spawnedInvertCircles.Add(spawnedCircle);
            DestroyCircle();
        });
    }
    private void DestroyCircle()
    {
        if (spawnedInvertCircles.Count > 2)
        {
            for (int i = 0; i < 2; i++)
            {
                invertCirclePool.Release(spawnedInvertCircles[i]);
                spawnedInvertCircles.RemoveAt(i);
            }
        }
    }
    float GetCircleScale(Vector2 pos)
    {
        Vector2 cameraExtends = new Vector2(cam.orthographicSize * Screen.width / Screen.height, cam.orthographicSize);
        Vector2 topRight = cameraExtends;
        Vector2 topLeft = new Vector2(-cameraExtends.x, cameraExtends.y);
        Vector2 bottomRight = new Vector2(cameraExtends.x, -cameraExtends.y);
        Vector2 bottomLeft = new Vector2(-cameraExtends.x, -cameraExtends.y);
        float distTopRight = Vector2.Distance(topRight, pos);
        float distTopLeft = Vector2.Distance(topLeft, pos);
        float distBottomRight = Vector2.Distance(bottomRight, pos);
        float distBottonLeft = Vector2.Distance(bottomLeft, pos);
        float maxDist = Mathf.Max(Mathf.Max(distTopLeft, distTopRight), Mathf.Max(distBottonLeft, distBottomRight));
        return maxDist / 2.5f;
    }

    public enum MenuState
    {
        MainMenu,
        CreditsMenu,
        Transition,
    }
}
