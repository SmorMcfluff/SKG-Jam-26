using UnityEngine;

public class BeatButton : MonoBehaviour
{
    public BeatColumn column;
    public BeatType instrument;

    public SpriteRenderer indicator;

    private void OnMouseDown()
    {
        column.Toggle(instrument);
    }
}

