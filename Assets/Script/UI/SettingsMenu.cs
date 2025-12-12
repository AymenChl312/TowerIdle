using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public GameObject confirmPanel;

    void Start()
    {
        CloseAllMenus();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnResetButtonClicked()
    {
        if (confirmPanel != null) confirmPanel.SetActive(true);
    }

    public void OnCancelReset()
    {
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    public void OnConfirmReset()
    {
        CloseAllMenus();

        if (SaveManager.instance != null)
        {
            SaveManager.instance.DeleteSave();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void CloseAllMenus()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }
}