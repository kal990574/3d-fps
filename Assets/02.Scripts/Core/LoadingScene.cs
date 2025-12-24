using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText;

    private void Start()
    {
        StartCoroutine(LoadScene_Coroutine());
    }

    private IEnumerator LoadScene_Coroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("GameScene");
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            _progressSlider.value = ao.progress;
            _progressText.text = $"{ao.progress*100}%";
            Debug.Log(ao.progress);
            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
