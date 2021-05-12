using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    private const float BG_OFFSET = 1f;

    public GameObject bgTile;
    public Transform holder;

    private Vector3 leftBottom;
    private Vector3 leftTop;
    private Vector3 rightTop;
    private Vector3 rightBottom;

    private SpriteRenderer spriteRenderer;

    private float halfTileHeight;
    private float halfTileWidth;

    private int columnCount;
    private int rowCount;

    private Camera cam;

    void Awake()
    {

        SetCameraBoundaries();
    }
    private void Start()
    {


        InitValues();
        CreateBackground();
    }

    private void InitValues()
    {
        spriteRenderer = bgTile.transform.GetComponent<SpriteRenderer>();
        halfTileHeight = spriteRenderer.bounds.extents.y;
        halfTileWidth = spriteRenderer.bounds.extents.x;

        columnCount = Mathf.CeilToInt((rightTop - leftBottom).x / (halfTileWidth * 2));
        rowCount = Mathf.CeilToInt((rightTop - leftBottom).y / (halfTileHeight * 2));

    }

    private void CreateBackground()
    {
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                Vector3 pos = leftBottom + new Vector3(halfTileWidth + j * halfTileWidth * 2, halfTileHeight + i * halfTileHeight * 2, 0);
                Instantiate(bgTile, pos, Quaternion.identity, holder);
            }
        }
    }

    private void SetCameraBoundaries()
    {
        cam = Camera.main;


        float z = -cam.transform.position.z + BG_OFFSET;
        leftBottom = cam.ScreenToWorldPoint(new Vector3(0, 0, z));
        leftTop = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, z));
        rightTop = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, z));
        rightBottom = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, z));


    }
}
