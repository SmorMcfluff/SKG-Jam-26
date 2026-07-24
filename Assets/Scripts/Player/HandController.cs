using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    public bool isDead;

    public static HandController instance;
    public HandState[] handStates;

    public Image handImg;

    private HandState currentHandState;
    private ShapeObject draggedShape;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;

        handStates[(int)HandInt.shape].OnPress += BeginDrag;
        handStates[(int)HandInt.shape].OnRelease += EndDrag;

        handStates[(int)HandInt.shoot].OnPress += Shoot;        

        currentHandState = handStates[(int)HandInt.point];
        SetHandSprite(currentHandState.openSprite);

        GameManager.SetCursor(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;

        if (!isDead)
        {
            HandleInput();
        }


        if (GameManager.instance.pcGameInProgress)
        {

            if (draggedShape != null && GameManager.instance.state is ShapeHole)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                mouseWorld.z = 0;

                draggedShape.Drag(mouseWorld);
            }
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetHandSprite(currentHandState.holdSprite);
            currentHandState.OnPress?.Invoke();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetHandSprite(currentHandState.openSprite);
            currentHandState.OnRelease?.Invoke();
        }
    }

    public void ChangeHandState(HandInt newState)
    {
        currentHandState = handStates[(int)newState];

        Sprite startSprite = (Input.GetMouseButton(0))
            ? currentHandState.holdSprite
            : currentHandState.openSprite;

        SetHandSprite(startSprite);
    }

    private void SetHandSprite(Sprite sprite)
    {
        handImg.sprite = sprite;

        RectTransform rt = handImg.rectTransform;

        Vector2 normalizedPivot = new(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
        rt.pivot = normalizedPivot;
    }

    private void BeginDrag()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 point = new(mouseWorld.x, mouseWorld.y);

        Collider2D hit = Physics2D.OverlapPoint(point);

        if (hit != null && hit.TryGetComponent(out draggedShape))
        {
            draggedShape.BeginDrag();

            handImg.DOKill();
            handImg.DOFade(0.75f, 0.15f);
        }
    }

    private void EndDrag()
    {
        if (draggedShape == null) return;

        draggedShape.EndDrag();
        draggedShape = null;

        handImg.DOKill();
        handImg.DOFade(1f, 0.15f);
    }

    private void Shoot()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new(mouseWorld.x, mouseWorld.y);

        int enemyMask = LayerMask.GetMask("Enemy");
        Collider2D hit = Physics2D.OverlapPoint(point, enemyMask);

        if (hit != null && hit.TryGetComponent(out Distraction enemy))
        {
            enemy.TakeDamage(1);
        }
    }
}

public enum HandInt
{
    shoot, shape, point
}
