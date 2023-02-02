using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;

public class Level : MonoBehaviour
{
    public Tilemap tilemap;
    public float expand;
    public List<TilemapRenderer> tilemapRenderers = new List<TilemapRenderer>();
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    [HideInInspector] public MaskTextMeshProUGUI levelNumber;
    void Awake()
    {
        CameraManager.Instance.SetCamera(tilemap, expand);
        GameManager.Instance.SetLevel(this);
    }
    private void OnDestroy()
    {
        if (levelNumber != null) Destroy(levelNumber.gameObject);
    }
    public void SetLevelInteractionNone()
    {
        levelNumber.gameObject.GetComponent<MaskInteraction>().SetInteraction(MaskInteraction.MaskInter.None);
    }
    public void SetLevelNumberText(int level)
    {
        levelNumber = UIManager.Instance.SpawnLevelNumber(level);
    }
    public void SetVisibleOutside()
    {
        foreach(TilemapRenderer tr in tilemapRenderers)
        {
            tr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        levelNumber.gameObject.GetComponent<MaskInteraction>().SetInteraction(MaskInteraction.MaskInter.VisibleOutsideMask);
    }
    public void SetVisibleInside()
    {
        foreach (TilemapRenderer tr in tilemapRenderers)
        {
            tr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        levelNumber.gameObject.GetComponent<MaskInteraction>().SetInteraction(MaskInteraction.MaskInter.VisibleInsideMask);
    }
    public void SetNone()
    {
        foreach (TilemapRenderer tr in tilemapRenderers)
        {
            tr.maskInteraction = SpriteMaskInteraction.None;
        }
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.maskInteraction = SpriteMaskInteraction.None;
        }
        if (levelNumber != null) SetLevelInteractionNone();
    }

}
