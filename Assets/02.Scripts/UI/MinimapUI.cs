using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MinimapCamera _minimapCamera;

    [Header("Buttons")]
    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;

    private void Start()
    {
        if (_zoomInButton != null)
        {
            _zoomInButton.onClick.AddListener(OnZoomInClicked);
        }

        if (_zoomOutButton != null)
        {
            _zoomOutButton.onClick.AddListener(OnZoomOutClicked);
        }
    }

    private void OnDestroy()
    {
        if (_zoomInButton != null)
        {
            _zoomInButton.onClick.RemoveListener(OnZoomInClicked);
        }

        if (_zoomOutButton != null)
        {
            _zoomOutButton.onClick.RemoveListener(OnZoomOutClicked);
        }
    }

    private void OnZoomInClicked()
    {
        if (_minimapCamera != null)
        {
            _minimapCamera.ZoomIn();
        }
    }

    private void OnZoomOutClicked()
    {
        if (_minimapCamera != null)
        {
            _minimapCamera.ZoomOut();
        }
    }
}