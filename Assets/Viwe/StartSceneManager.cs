using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    private Button startButton;
    private Button exitButton;

    private void Awake()
    {
        GameObject sButton = gameObject.transform.GetChild(1).gameObject;
        GameObject eButton = gameObject.transform.GetChild(2).gameObject;

        startButton = sButton.GetComponent<Button>();
        exitButton = eButton.GetComponent<Button>();
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartFunction);
        exitButton.onClick.AddListener(ExitFunction);
        SoundManager.instance.StartSceneSound();
    }

    private void StartFunction()
    {
        SceneManager.LoadScene(1);
    }

    private void ExitFunction()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
