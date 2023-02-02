using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public Camera cam;
    public static CameraManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        cam = Camera.main;
    }
    private (Vector3 center, float size) CalculateOrthoSize(Tilemap tilemap, float buffer)
    {
        var bound = new Bounds();
        tilemap.CompressBounds();
        var tilemapBounds = tilemap.localBounds;
        bound.Encapsulate(tilemapBounds);
        bound.Expand(buffer);
        var vertical = bound.size.y;
        var horizontal = bound.size.x * cam.pixelHeight / cam.pixelWidth;
        var size = Mathf.Max(vertical, horizontal) * 0.5f;
        var center3d = bound.center + tilemap.transform.position;
        var center2d = new Vector3(center3d.x, center3d.y, -10);
        return (center2d, size);
    }
    public void SetCamera(Tilemap tilemap, float buffer)
    {
        var (center, size) = CalculateOrthoSize(tilemap, buffer);
        if (!GameManager.Instance.transition)
        {
            cam.transform.position = center;
            cam.orthographicSize = size;
        }
        else
        {
            cam.transform.DOMove(center, 0.4f);
            cam.DOOrthoSize(size, 0.5f);
        }
    }
}
