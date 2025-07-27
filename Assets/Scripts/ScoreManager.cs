using TMPro;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    [SerializeField] private int cookiePoint;
    public int enemyPoint { get; private set; }

    public int score { get; private set; }

    private void Start()
    {
        enemyPoint = 200;
        scoreText = GetComponent<TextMeshProUGUI>();
        score = 0;
        scoreText.text = "Score : " + score;
    }

    private void Update()
    {
        scoreText.text = "Score : " + score;
    }

    public void CookieScore()
    {
        score += cookiePoint;
    }

    public void EnemyScore()
    {
        score += enemyPoint;
    }

    public int GetCookiePoint()
    {
        return cookiePoint;
    }
}
