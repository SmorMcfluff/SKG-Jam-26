using UnityEngine;

public class ShapeReceiver : MonoBehaviour
{
    public SpriteRenderer sr;

    public void Initialize(Sprite texture)
    {
        sr.sprite = texture;
    }

    public bool TryFitShape(Shape otherShape)
    {
        return true;
    }
}
