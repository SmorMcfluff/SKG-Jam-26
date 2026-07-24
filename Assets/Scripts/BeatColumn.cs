using UnityEngine;

public class BeatColumn : MonoBehaviour
{
    [SerializeField] private BeatButton[] buttons;

    public MusicBeat Beat { get; } = new();

    private void Awake()
    {
        foreach (var button in buttons)
            button.column = this;
    }

    private void Start()
    {
        UpdateVisuals();
    }

    public void Toggle(BeatType instrument)
    {
        Beat.Toggle(instrument);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (var button in buttons)
        {
            button.indicator.enabled = Beat.Has(button.instrument);
        }
    }
}