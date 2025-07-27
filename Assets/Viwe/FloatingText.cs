using TMPro;
using UnityEngine;
using VContainer;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private ScoreManager _scoreManager;

    [Inject]
    public void Construct(ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
    }

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        scoreText.text = _scoreManager.enemyPoint.ToString();
    }
}
