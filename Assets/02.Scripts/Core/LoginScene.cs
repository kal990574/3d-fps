using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

public class LoginScene : MonoBehaviour
{
    // 로그인씬 (로그인/회원가입) -> 게임씬

    // 상수 정의
    private const string LAST_LOGIN_ID_KEY = "LastLoginID";
    private const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private const string HMAC_KEY = "3DFPSGame_SecretKey_2024";

    private enum SceneMode
    {
        Login,
        Register
    }
    
    private SceneMode _mode = SceneMode.Login;
    
    // 비밀번호 확인 오브젝트
    [SerializeField] private GameObject _passwordCofirmObject;
    [SerializeField] private Button _gotoRegisterButton;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _gotoLoginButton;
    [SerializeField] private Button _registerButton;

    [SerializeField] private TextMeshProUGUI _messageTextUI;
    
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _passwordConfirmInputField;
    
    private void Start()
    {
        AddButtonEvents();
        LoadLastLoginID();
        Refresh();
    }

    private void AddButtonEvents()
    {
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _loginButton.onClick.AddListener(Login);
        _gotoLoginButton.onClick.AddListener(GotoLogin);
        _registerButton.onClick.AddListener(Register);
    }

    private void Refresh()
    {
        // 2차 비밀번호 오브젝트는 회원가입 모드일때만 노출
        _passwordCofirmObject.SetActive(_mode == SceneMode.Register);
        _gotoRegisterButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        _gotoLoginButton.gameObject.SetActive(_mode == SceneMode.Register);
        _registerButton.gameObject.SetActive(_mode == SceneMode.Register);
    }

    private void Login()
    {
        // 로그인
        // 1. 아이디 입력을 확인한다.
        string id = _idInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        // 2. 비밀번호 입력을 확인한다.
        string password = _passwordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "패스워드를 입력해주세요.";
            return;
        }

        // 3. 실제 저장된 아이디-비밀번호 계정이 있는지 확인한다.
        // 3-1. 아이디가 있는지 확인한다.
        if (!PlayerPrefs.HasKey(id))
        {
            _messageTextUI.text = "아이디를 확인해주세요.";
            return;
        }

        // 3-2. 해시된 비밀번호 검증
        string storedHash = PlayerPrefs.GetString(id);
        if (!VerifyPassword(password, storedHash))
        {
            _messageTextUI.text = "비밀번호를 확인해주세요.";
            return;
        }

        // 4. 마지막 로그인 ID 저장 후 씬 이동
        PlayerPrefs.SetString(LAST_LOGIN_ID_KEY, id);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("LoadingScene");
    }

    private void Register()
    {
        // 회원가입
        // 1. 아이디 입력을 확인한다.
        string id = _idInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        // 1-1. 이메일 형식 검사
        if (!IsValidEmail(id))
        {
            _messageTextUI.text = "올바른 이메일 형식이 아닙니다.\n예: example@email.com";
            return;
        }

        // 2. 비밀번호 입력을 확인한다.
        string password = _passwordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "패스워드를 입력해주세요.";
            return;
        }

        // 2-1. 비밀번호 유효성 검사
        if (!IsValidPassword(password))
        {
            _messageTextUI.text = GetPasswordRequirements();
            return;
        }

        // 3. 비밀번호 확인 입력을 확인한다. (버그 수정: _passwordConfirmInputField 사용)
        string password2 = _passwordConfirmInputField.text;
        if (string.IsNullOrEmpty(password2) || password != password2)
        {
            _messageTextUI.text = "패스워드가 일치하지 않습니다.";
            return;
        }

        // 4. 중복 계정 확인
        if (PlayerPrefs.HasKey(id))
        {
            _messageTextUI.text = "중복된 아이디입니다.";
            return;
        }

        // 5. 비밀번호 해싱 후 저장
        string hashedPassword = HashPassword(password);
        PlayerPrefs.SetString(id, hashedPassword);
        PlayerPrefs.Save();

        _messageTextUI.text = "회원가입이 완료되었습니다.";
        GotoLogin();
    }

    private void GotoLogin()
    {
        _mode = SceneMode.Login;
        Refresh();
    }

    private void GotoRegister()
    {
        _mode = SceneMode.Register;
        Refresh();
    }

    // 마지막 로그인 ID 불러오기
    private void LoadLastLoginID()
    {
        if (PlayerPrefs.HasKey(LAST_LOGIN_ID_KEY))
        {
            _idInputField.text = PlayerPrefs.GetString(LAST_LOGIN_ID_KEY);
        }
    }

    // 이메일 형식 검사
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return Regex.IsMatch(email, EMAIL_PATTERN);
    }

    // 비밀번호 유효성 검사
    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // 길이 체크 (7~20자)
        if (password.Length < 7 || password.Length > 20)
            return false;

        // 영어/숫자/특수문자만 허용
        if (!Regex.IsMatch(password, @"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+$"))
            return false;

        // 대문자 최소 1개
        if (!Regex.IsMatch(password, @"[A-Z]"))
            return false;

        // 소문자 최소 1개
        if (!Regex.IsMatch(password, @"[a-z]"))
            return false;

        // 특수문자 최소 1개
        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            return false;

        return true;
    }

    // 비밀번호 요구사항 메시지
    private string GetPasswordRequirements()
    {
        return "비밀번호 요구사항:\n" +
               "- 7~20자\n" +
               "- 대문자, 소문자 각 1개 이상\n" +
               "- 특수문자 1개 이상\n" +
               "- 영어/숫자/특수문자만 가능";
    }

    // HMACSHA256 해싱
    private string HashPassword(string password)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(HMAC_KEY);

        using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = hmac.ComputeHash(passwordBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

    // 비밀번호 검증
    private bool VerifyPassword(string inputPassword, string storedHash)
    {
        string inputHash = HashPassword(inputPassword);
        return inputHash == storedHash;
    }

}