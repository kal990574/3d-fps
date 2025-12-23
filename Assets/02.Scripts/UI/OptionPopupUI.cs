using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionPopupUI : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;
    private void Start()
    {
        _continueButton.onClick.AddListener(GameContinue);
        _restartButton.onClick.AddListener(GameRestart);
        _quitButton.onClick.AddListener(GameQuit);
        Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void GameContinue()
    {
        GameManager.Instance.Continue();
        Hide();
    }

    private void GameRestart()
    {
        GameManager.Instance.Restart();
        Hide();
    }

    private void GameQuit()
    {
        GameManager.Instance.Quit();
    }
}
