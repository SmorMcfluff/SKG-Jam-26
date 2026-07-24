using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreFill : MonoBehaviour
{
    TextMeshProUGUI scoreText;
    public Image fillImage;

    public bool followFill;

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    void LateUpdate()
    {
        if (followFill)
        {
            fillImage.fillAmount = Random.value;
            float score = fillImage.fillAmount * 5;
            scoreText.text = score.ToString("0.00");
        }
        else
        {
            fillImage.fillAmount = 0;
            scoreText.text = "-1/5";
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Start Scene");
    }
}
