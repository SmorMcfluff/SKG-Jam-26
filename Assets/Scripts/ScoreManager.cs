using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score;
    public TextMeshProUGUI scoreText;

    public void Awake()
    {
        instance = this;
    }

    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = $"Score \n {score}";
    }
}
