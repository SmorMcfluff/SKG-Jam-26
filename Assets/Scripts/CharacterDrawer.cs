using UnityEngine;

public class CharacterDrawer : MonoBehaviour
{
    public static CharacterDrawer instance;

    Camera mainCamera;

    LineRenderer currentLineRenderer;
    public LineRenderer brushPrefab;

    Vector2 lastPos;

    private Collider2D drawingArea;

    public Transform characterHolder;

    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
        drawingArea = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            characterHolder.SetParent(GameObject.Find("Character Holder").transform);
            characterHolder.localPosition = Vector3.zero;
            Destroy(gameObject);
            CameraController.instance.GoDirection(Direction.Down);
            GameManager.DisplayInstruction("KILL ALL DISTRACTIONS!!!", 2,
                () =>
                {
                    EnemySpawner.instance.StartWave(3,
                        () => CameraController.instance.GoDirection(Direction.Right, () =>
                        {
                            CameraController.instance.GoDirection(Direction.Up);
                            GameManager.DisplayInstruction("MAKE MUSIC FOR \nFOR YOUR GAME!!", 2);
                        }
                        ));
                    CameraController.instance.GoDirection(Direction.Left);
                }
            );
        }
        Drawing();
    }

    void Drawing()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (drawingArea.OverlapPoint(mousePos))
            {
                CreateBrush(mousePos);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (currentLineRenderer == null)
                return;

            if (!drawingArea.OverlapPoint(mousePos))
            {
                currentLineRenderer = null;
                return;
            }

            PointToMousePos(mousePos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            currentLineRenderer = null;
        }
    }

    void CreateBrush(Vector2 worldPos)
    {
        currentLineRenderer = Instantiate(brushPrefab, characterHolder);

        currentLineRenderer.positionCount = 1;

        Vector3 localPos = characterHolder.InverseTransformPoint(worldPos);
        currentLineRenderer.SetPosition(0, localPos);

        lastPos = worldPos;
    }

    void AddAPoint(Vector2 worldPos)
    {
        Vector3 localPos = characterHolder.InverseTransformPoint(worldPos);

        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, localPos);
    }

    void PointToMousePos(Vector2 mousePos)
    {
        if (Vector2.Distance(lastPos, mousePos) > 0.03f)
        {
            AddAPoint(mousePos);
            lastPos = mousePos;
        }
    }
}
