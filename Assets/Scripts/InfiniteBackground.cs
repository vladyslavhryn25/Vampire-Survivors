using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Sprite tileSprite;

    [Header("Generated Tile")]
    [SerializeField] private int tileResolution = 64;
    [SerializeField] private float pixelsPerUnit = 16f;
    [SerializeField] private int patchSize = 8;
    [SerializeField] private Color darkColor = new Color(0.14f, 0.20f, 0.16f);
    [SerializeField] private Color lightColor = new Color(0.22f, 0.30f, 0.24f);
    [SerializeField] private Color detailColor = new Color(0.32f, 0.44f, 0.29f);
    [SerializeField, Range(0f, 0.05f)] private float detailDensity = 0.012f;
    [SerializeField, Range(0f, 0.2f)] private float pixelNoise = 0.05f;
    [SerializeField] private int seed = 12345;

    [Header("Rendering")]
    [SerializeField] private int sortingOrder = -100;

    private SpriteRenderer spriteRenderer;
    private Vector2 tileSize;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (tileSprite == null) tileSprite = BuildTile();

        spriteRenderer.sprite = tileSprite;
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.sortingOrder = sortingOrder;

        tileSize = tileSprite.bounds.size;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        int countX = Mathf.CeilToInt(halfW * 2f / tileSize.x) + 4;
        int countY = Mathf.CeilToInt(halfH * 2f / tileSize.y) + 4;
        spriteRenderer.size = new Vector2(countX * tileSize.x, countY * tileSize.y);

        Vector3 c = cam.transform.position;
        transform.position = new Vector3(
            Mathf.Round(c.x / tileSize.x) * tileSize.x,
            Mathf.Round(c.y / tileSize.y) * tileSize.y,
            0f);
    }

    private Sprite BuildTile()
    {
        int res = tileResolution;
        int cells = Mathf.Max(1, res / patchSize);
        System.Random rng = new System.Random(seed);

        float[] patches = new float[cells * cells];
        for (int i = 0; i < patches.Length; i++)
            patches[i] = (float)rng.NextDouble();

        Color[] pixels = new Color[res * res];
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float patch = patches[(x / patchSize) % cells + ((y / patchSize) % cells) * cells];
                float noise = ((float)rng.NextDouble() - 0.5f) * pixelNoise;
                Color c = Color.Lerp(darkColor, lightColor, patch);
                pixels[x + y * res] = new Color(c.r + noise, c.g + noise, c.b + noise, 1f);
            }
        }

        int details = Mathf.RoundToInt(res * res * detailDensity);
        for (int i = 0; i < details; i++)
        {
            int x = rng.Next(res);
            int y = rng.Next(res);
            pixels[x + y * res] = detailColor;
            pixels[x + ((y + 1) % res) * res] = detailColor;
            if (rng.NextDouble() > 0.5)
                pixels[((x + 1) % res) + y * res] = detailColor;
        }

        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), pixelsPerUnit, 0, SpriteMeshType.FullRect);
    }
}
