using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeGameHUD : MonoBehaviour
{
    public Image comboSlider;
    public Image totalSlider;

    public TextMeshProUGUI multiplier;

    public void UpdateSlider(Image slider, float fillAmount)
    {
        slider.fillAmount = fillAmount;
    }

    public void UpdateMultiplier(int mult)
    {
        multiplier.text = $"x {mult}";
    }
}
