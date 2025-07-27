using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class HighScore : MonoBehaviour
{
    private TextMeshProUGUI rankingText;

    private void Start()
    {
        rankingText = GetComponent<TextMeshProUGUI>();
    }

    public void WritingFinishScores(List<int> scores)
    {
        if (scores.Count <= 0)
        {
            rankingText.text = $"1st -- 2nd -- 3rd --";
        }
        else
        {
            string firstScore = (scores.Count > 0) ? scores[0].ToString() : "--";
            string secondScore = (scores.Count >1) ? scores[1].ToString() : "--";
            string thirdScore = (scores.Count > 2)? scores[2].ToString() : "--";

            rankingText.text = $"1st {firstScore} 2nd {secondScore} 3rd{thirdScore}";
        }
    }
}
