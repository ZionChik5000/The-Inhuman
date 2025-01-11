using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;
    public AudioSource musicSource;
    public AudioSource soundSource;

    public Slider sensitivitySlider;
    public float defaultSensitivity = 400f;

    public GameObject keyBindingPanel;
    public GameObject panelS;

    public Text moveForwardKeyText;
    public Text moveBackwardKeyText;
    public Text moveLeftKeyText;
    public Text moveRightKeyText;
    public Text jumpKeyText;
    public Text crouchKeyText;
    public Text slideKeyText;
    public Text dashKeyText;

    public Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    private string keyToRebind;

    private void Start()
    {
/*        InitializeKeyBindings();
        DebugKeyBindings();
        UpdateKeyBindingUI();*/

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundSlider.onValueChanged.AddListener(SetSoundVolume);

        sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

        SetMusicVolume(musicSlider.value);
        SetSoundVolume(soundSlider.value);
        SetMouseSensitivity(sensitivitySlider.value);
    }

/*    private void InitializeKeyBindings()
    {
        keyBindings = new Dictionary<string, KeyCode>
    {
        { "MoveForward", KeyCode.W },
        { "MoveBackward", KeyCode.S },
        { "MoveLeft", KeyCode.A },
        { "MoveRight", KeyCode.D },
        { "Jump", KeyCode.Space },
        { "Crouch", KeyCode.LeftControl },
        { "Slide", KeyCode.LeftShift },
        { "Dash", KeyCode.V }
    };

        foreach (var key in keyBindings.Keys.ToList())
        {
            if (PlayerPrefs.HasKey(key))
            {
                keyBindings[key] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
            }
        }
    }*/


    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSoundVolume(float value)
    {
        soundSource.volume = value;
        PlayerPrefs.SetFloat("SoundVolume", value);
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

/*    public void StartRebinding(string key)
    {
        if (!keyBindings.ContainsKey(key))
        {
            Debug.LogError($"Key '{key}' not found in keyBindings dictionary!");
            return;
        }

        keyToRebind = key;

        switch (key)
        {
            case "MoveForward": moveForwardKeyText.text = ""; break;
            case "MoveBackward": moveBackwardKeyText.text = ""; break;
            case "MoveLeft": moveLeftKeyText.text = ""; break;
            case "MoveRight": moveRightKeyText.text = ""; break;
            case "Jump": jumpKeyText.text = ""; break;
            case "Crouch": crouchKeyText.text = ""; break;
            case "Slide": slideKeyText.text = ""; break;
            case "Dash": dashKeyText.text = ""; break;
        }
    }

    private bool IsKeyAlreadyBound(KeyCode keyCode)
    {
        return keyBindings.Values.Contains(keyCode);
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(keyToRebind) && Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (IsKeyAlreadyBound(keyCode))
                    {
                        Debug.LogWarning($"Key '{keyCode}' is already assigned to another action.");
                        return;
                    }

                    keyBindings[keyToRebind] = keyCode;
                    PlayerPrefs.SetString(keyToRebind, keyCode.ToString());
                    keyToRebind = string.Empty;

                    // Обновляем UI после выбора клавиши
                    UpdateKeyBindingUI();
                    break;
                }
            }
        }
    }*/

    public void ResetToDefault()
    {
        PlayerPrefs.DeleteAll();

        musicSlider.value = 0.5f;
        soundSlider.value = 0.5f;

        sensitivitySlider.value = defaultSensitivity;

/*        InitializeKeyBindings();
        UpdateKeyBindingUI();*/
    }

/*    private void UpdateKeyBindingUI()
    {
        moveForwardKeyText.text = keyBindings["MoveForward"].ToString();
        moveBackwardKeyText.text = keyBindings["MoveBackward"].ToString();
        moveLeftKeyText.text = keyBindings["MoveLeft"].ToString();
        moveRightKeyText.text = keyBindings["MoveRight"].ToString();
        jumpKeyText.text = keyBindings["Jump"].ToString();
        crouchKeyText.text = keyBindings["Crouch"].ToString();
        slideKeyText.text = keyBindings["Slide"].ToString();
        dashKeyText.text = keyBindings["Dash"].ToString();
    }

    public void SwitchPanel()
    {
        if (keyBindingPanel != null) keyBindingPanel.SetActive(true);
        if (panelS != null) panelS.SetActive(false);
    }

    public void SwitchPanel2()
    {
        if (panelS != null) panelS.SetActive(true);
        if (keyBindingPanel != null) keyBindingPanel.SetActive(false);
    }*/

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

/*    private void DebugKeyBindings()
    {
        foreach (var pair in keyBindings)
        {
            Debug.Log($"Key: {pair.Key}, Value: {pair.Value}");
        }
    }*/
}
