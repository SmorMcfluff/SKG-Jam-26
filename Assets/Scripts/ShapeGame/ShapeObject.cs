using DG.Tweening;
using UnityEngine;

public class ShapeObject : MonoBehaviour
{
    public bool draggable = true;

    private Shape shape;
    public SpriteMask mask;
    public SpriteRenderer sr;
    public SpriteRenderer outline;

    private ShapeReceiver currentReceiver;

    private Vector3 defaultScale;

    private Vector3 dragStartPos;

    private Tween returnTween;

    private void Start()
    {
        defaultScale = transform.localScale;
    }

    public void Initialize(Shape shape, Sprite shapeSprite, Sprite texture)
    {
        this.shape = shape;
        mask.sprite = shapeSprite;
        sr.sprite = texture;
        outline.sprite = shapeSprite;
    }

    public void BeginDrag()
    {
        if (returnTween != null && returnTween.IsActive())
        {
            returnTween.Kill();
            returnTween = null;
            draggable = true;
        }

        if (!draggable)
            return;

        dragStartPos = transform.position;

        transform.DOScale(defaultScale * 1.5f, 0.3f)
            .SetEase(Ease.OutBounce);
    }

    public void Drag(Vector3 worldPosition)
    {
        if (!draggable) return;
        transform.position = worldPosition;
    }

    public void EndDrag()
    {

        if (currentReceiver != null)
        {
            if (currentReceiver.TryFitShape(shape))
            {
                ShapeGameManager.instance.OnShapeSwallowed();
                draggable = false;

                Sequence seq = DOTween.Sequence();

                seq.Append(
                    transform.DOMove(currentReceiver.transform.position, 0.1f));
                seq.Append(
                    transform.DOScale(0, 0.5f)
                        .SetEase(Ease.InQuad));

                seq.OnComplete(() =>
                {
                    Destroy(gameObject);
                });

                return;
            }

            ReturnToDragStart();
            return;
        }

        if (!IsInsidePlayArea())
        {
            ReturnToDragStart();
            return;
        }

        transform.DOScale(defaultScale, 0.3f)
            .SetEase(Ease.OutBounce);
    }

    private bool IsInsidePlayArea()
    {
        Bounds objectBounds = GetComponent<Collider2D>().bounds;
        Bounds playBounds = ShapeGameManager.instance.bounds;

        return playBounds.Contains(objectBounds.min) &&
               playBounds.Contains(objectBounds.max);
    }

    private void ReturnToDragStart()
    {
        draggable = false;

        float speed = 3f;
        float duration = Vector3.Distance(transform.position, dragStartPos) / speed;

        returnTween = transform.DOMove(dragStartPos, duration);

        returnTween.OnComplete(() =>
        {
            draggable = true;
            returnTween = null;
        });

        transform.DOScale(defaultScale, 0.3f)
            .SetEase(Ease.OutBounce);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ShapeReceiver receiver))
        {
            currentReceiver = receiver;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out ShapeReceiver receiver) &&
            receiver == currentReceiver)
        {
            currentReceiver = null;
        }
    }
}

public enum Shape
{
    Square
}