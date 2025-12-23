using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _stateTextUI;
    [SerializeField] private OptionPopupUI _optionPopupUI;

    [Header("Core References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;

    private List<MonoBehaviour> _pausableComponents = new List<MonoBehaviour>();

    // Player GameObject reference.
    public GameObject Player => _player;

    // Main camera reference.
    public Camera MainCamera => _mainCamera;

    // Player Transform reference.
    public Transform PlayerTransform => _player?.transform;

    // Manager accessors.
    public InputManager Input => InputManager.Instance;
    public GameplayPoolManager GameplayPool => GameplayPoolManager.Instance;
    public EffectPoolManager EffectPool => EffectPoolManager.Instance;

    private void Awake()
    {
        _instance = this;
        GameEvents.OnPlayerDeath += HandlePlayerDeath;
    }

    private void Start()
    {
        ValidateReferences();
        CachePausableComponents();

        _stateTextUI.gameObject.SetActive(true);
        _state = EGameState.Ready;
        _stateTextUI.text = "준비중";

        StartCoroutine(StartToPlay_Coroutine());
    }

    private void Update()
    {
        HandleCursorLock();
        if (InputManager.Instance.EscapePressed)
        {
            Time.timeScale = 0f;
            _optionPopupUI.Show();
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandleCursorLock()
    {
        if (_state != EGameState.Playing)
        {
            return;
        }

        if (!InputManager.Instance.EscapePressed)
        {
            return;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void ValidateReferences()
    {
        if (_player == null)
        {
            Debug.LogError("[GameManager] Player reference is missing! Please assign in Inspector.");
        }

        if (_mainCamera == null)
        {
            Debug.LogError("[GameManager] MainCamera reference is missing! Please assign in Inspector.");
        }
    }

    private void CachePausableComponents()
    {
        if (_player != null)
        {
            _pausableComponents.Add(_player.GetComponent<PlayerMove>());
            _pausableComponents.Add(_player.GetComponent<PlayerRotate>());
            _pausableComponents.Add(_player.GetComponent<PlayerGunFire>());
            _pausableComponents.Add(_player.GetComponent<PlayerBombFire>());
        }

        if (_mainCamera != null)
        {
            _pausableComponents.Add(_mainCamera.GetComponent<CameraRotate>());
        }

        _pausableComponents.RemoveAll(c => c == null);
    }

    private void SetPausableComponentsEnabled(bool enabled)
    {
        foreach (var component in _pausableComponents)
        {
            if (component != null)
            {
                component.enabled = enabled;
            }
        }

        SetMonstersEnabled(enabled);
    }

    private void SetMonstersEnabled(bool isEnabled)
    {
        MonsterStateMachine[] monsters = FindObjectsByType<MonsterStateMachine>(FindObjectsSortMode.None);
        foreach (var monster in monsters)
        {
            if (monster != null)
            {
                monster.enabled = isEnabled;
            }
        }

        EliteMonsterStateMachine[] eliteMonsters = FindObjectsByType<EliteMonsterStateMachine>(FindObjectsSortMode.None);
        foreach (var eliteMonster in eliteMonsters)
        {
            if (eliteMonster != null)
            {
                eliteMonster.enabled = isEnabled;
            }
        }
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        SetPausableComponentsEnabled(false);

        yield return new WaitForSeconds(1.5f);

        _stateTextUI.text = "시작!";

        yield return new WaitForSeconds(0.5f);

        _state = EGameState.Playing;
        _stateTextUI.gameObject.SetActive(false);

        SetPausableComponentsEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandlePlayerDeath()
    {
        _state = EGameState.GameOver;
        Time.timeScale = 0f;
        SetPausableComponentsEnabled(false);

        _stateTextUI.gameObject.SetActive(true);
        _stateTextUI.text = "GAME OVER";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
