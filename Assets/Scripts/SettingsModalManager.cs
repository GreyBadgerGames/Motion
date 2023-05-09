using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsModalManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _windowModeDropdown;
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _exitToDesktopBtn;
    [SerializeField] public GameObject _settingsModal;

    private List<Resolution> _resolutions = new List<Resolution>();
    private List<WindowMode> _windowModes = new List<WindowMode>();

    public void Start()
    {
        _backBtn.onClick.AddListener(backButtonButtonClicked);
        _exitToDesktopBtn.onClick.AddListener(exitToDesktopButtonClicked);
        _resolutionDropdown.onValueChanged.AddListener(delegate {
            resolutionDropdownChange();
        });
        _windowModeDropdown.onValueChanged.AddListener(delegate {
            windowModeDropdownChange();
        });

        initResolutionDropdown();
        initWindowModeDropdown();        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _settingsModal.activeInHierarchy)
        {
            _settingsModal.SetActive(false);   
        }
    }

    private void backButtonButtonClicked()
    {
        this.gameObject.SetActive(false);
    }

    private void exitToDesktopButtonClicked()
    {
        Application.Quit();
    }

    private void initResolutionDropdown()
    {
        _resolutions.Add(new Resolution() {Name = "3840 x 2160", Width = 3840, Height = 2160});
        _resolutions.Add(new Resolution() {Name = "2560 x 1440", Width = 2560, Height = 1440});
        _resolutions.Add(new Resolution() {Name = "1920 x 1080", Width = 1920, Height = 1080});
        _resolutions.Add(new Resolution() {Name = "1280 x 720", Width = 1280, Height = 720});

        _resolutionDropdown.options.Clear();

        int i = 0;
        foreach (Resolution resolution in _resolutions)
        {
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData() {text = resolution.Name});
            
            if (Screen.height == resolution.Height && Screen.width == resolution.Width)
            {
                // Stupid hack because you can't set placeholder text in dropdown, and without this
                // the application resets its resolution/window mode, which is quite jarring
                _resolutionDropdown.onValueChanged.RemoveAllListeners();
                _resolutionDropdown.value = i;
                _resolutionDropdown.onValueChanged.AddListener(delegate {
                    resolutionDropdownChange();
                });
            }
            i++;
        }
    }

    private void resolutionDropdownChange()
    {
        Resolution res = _resolutions[_resolutionDropdown.value];
        Debug.Log($"Setting resolution to {res.Name}");
        Screen.SetResolution(res.Width, res.Height, Screen.fullScreen);
    }

    // Supports windows only
    private void initWindowModeDropdown()
    {
        _windowModes.Add(new WindowMode() {Name = "Full Screen", Mode=FullScreenMode.ExclusiveFullScreen});
        _windowModes.Add(new WindowMode() {Name = "Borderless", Mode=FullScreenMode.FullScreenWindow});
        _windowModes.Add(new WindowMode() {Name = "Windowed", Mode=FullScreenMode.Windowed});

        _windowModeDropdown.options.Clear();

        int i = 0;
        foreach (WindowMode windowMode in _windowModes)
        {
            _windowModeDropdown.options.Add(new TMP_Dropdown.OptionData() {text = windowMode.Name});

            if (Screen.fullScreenMode == windowMode.Mode)
            {
                _windowModeDropdown.onValueChanged.RemoveAllListeners();
                _windowModeDropdown.captionText.text = windowMode.Name;
                _windowModeDropdown.onValueChanged.AddListener(delegate {
                    windowModeDropdownChange();
                });
            }
            i++;
        }
    }

    private void windowModeDropdownChange()
    {
        WindowMode mode = _windowModes[_windowModeDropdown.value];
        Debug.Log($"Setting window mode to {mode.Name}");
        Screen.fullScreenMode = mode.Mode;
    }
}

public class Resolution
{
    public string Name;
    public int Width;
    public int Height;
}

public class WindowMode
{
    public string Name;
    public FullScreenMode Mode;
}
