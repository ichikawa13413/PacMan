using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;



public class ClearAndOverManager : MonoBehaviour
{
    private Player _player;
    private PlayTimer _playTimer;
    private MapManager _mapManager;
    private Camera _camera;
    private Canvas _canvas;
    private ISaveData _saveFile;
    private ScoreManager _scoreManager;
    private HighScore _highScore;

    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Vector3 textSetPosition;

    [SerializeField] private Button _button;
    [SerializeField] private Vector3 buttonPosition;
    [SerializeField] private string retryText;
    [SerializeField] private string gameFinText;
    public Button ReTryButton {  get; private set; }
    public Button gameFinButton {  get; private set; }

    private bool hasExcuted;

    private static string key = "FinishScore";

    [Inject]
    public void Construct(Player player, PlayTimer playTimer, MapManager mapManager,Camera camera, Canvas canvas,
        ISaveData saveFile,ScoreManager scoreManager,HighScore highScore)
    {
        _player = player;
        _playTimer = playTimer;
        _mapManager = mapManager;
        _camera = camera;
        _canvas = canvas;
        _saveFile = saveFile;
        _scoreManager = scoreManager;
        _highScore = highScore;
    }

    private void Start()
    {
        CreateClearText();
        CreateGameOverText();
        CreateRetryButton();
        CreateGameFinButton();
        hasExcuted = false;
        _highScore.WritingFinishScores(Load());
    }

    private void Update()
    {
        if (_player.GetPlayerLife() == 0 && !hasExcuted)//ゲームオーバー時の処理
        {
            Save(_scoreManager.score);
            gameOverText.gameObject.SetActive(true);
            ReTryButton.gameObject.SetActive(true);
            gameFinButton.gameObject.SetActive(true);
            _playTimer.StopTimer();
            SoundManager.instance.SoundStop();
            hasExcuted = true;
        }
        else if (_mapManager.cookieCounter <= 0 && !hasExcuted)//ゲームクリア時の処理
        {
            Save(_scoreManager.score);
            clearText.gameObject.SetActive(true);
            ReTryButton.gameObject.SetActive(true);
            gameFinButton.gameObject.SetActive(true);
            _playTimer.StopTimer();
            SoundManager.instance.SoundStop();
            hasExcuted = true;
        }
    }
    
    private void CreateClearText()
    {
        clearText = Instantiate(clearText, transform.position, Quaternion.identity, _canvas.transform);
        RectTransform rect = clearText.GetComponent<RectTransform>();
        rect.position = GetScreenPosition(textSetPosition);
        clearText.gameObject.SetActive(false);
    }

    private void CreateRetryButton()
    {
        ReTryButton = Instantiate(_button, transform.position, Quaternion.identity, _canvas.transform);
        ReTryButton.onClick.AddListener(RetryFunction);

        TextMeshProUGUI ReStartButtonText = ReTryButton.GetComponentInChildren<TextMeshProUGUI>();
        ReStartButtonText.text = retryText.ToString();

        RectTransform rect = ReTryButton.GetComponent<RectTransform>();
        rect.position = GetScreenPosition(buttonPosition);
        ReTryButton.gameObject.SetActive(false);
    }

    private void CreateGameOverText()
    {
        gameOverText = Instantiate(gameOverText, transform.position, Quaternion .identity, _canvas.transform);
        RectTransform rect = gameOverText.GetComponent<RectTransform>();
        rect.position = GetScreenPosition(textSetPosition);
        gameOverText.gameObject.SetActive(false);
    }

    private void CreateGameFinButton()
    {
        gameFinButton = Instantiate(_button, transform.position, Quaternion.identity, _canvas.transform);
        gameFinButton.onClick.AddListener(ExitFunction);

        TextMeshProUGUI gameFinButtonText = gameFinButton.GetComponentInChildren<TextMeshProUGUI>();
        gameFinButtonText.text = gameFinText.ToString();

        RectTransform rect = gameFinButton.GetComponent<RectTransform>();
        Vector3 newButtonPos = new Vector3(buttonPosition.x * -1, buttonPosition.y, buttonPosition.z);//retryTextの反対側に作る
        rect.position = GetScreenPosition(newButtonPos);
        gameFinButton.gameObject.SetActive(false);
    }

    private Vector3 GetScreenPosition(Vector3 pos)
    {
        Vector3 position = RectTransformUtility.WorldToScreenPoint(_camera, pos);
        return position;
    }

    private void RetryFunction()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentScene);
    }

    private void ExitFunction()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif
    }

    private List<int> Load()
    {
        var jsonData = _saveFile.Load(key);
        var finishScores = JsonUtility.FromJson<ScoreData>(jsonData);
        if (finishScores == null) finishScores = new ScoreData();
        return finishScores.Score;
    }

    private void Save(int score)
    {
        var finishScores = new ScoreData();

        var data = Load();
        data.Add(score);

        finishScores.Score = data.OrderByDescending(score => score).Take(3).ToList();

        var jsonData = JsonUtility.ToJson(finishScores);
        _saveFile.Save(key, jsonData);
    }
}
