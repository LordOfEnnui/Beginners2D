using System;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour, IUiService {

    [SerializeField] UIPanel pauseUI;

    [SerializeField] SettingsMenuUI settingsPanel;
    [SerializeField] LoadingScreenUI loading;

    public LoadingScreenUI LoadingUI => loading;
    public SettingsMenuUI SettingsMenu => settingsPanel;
    public UIPanel PauseUI => pauseUI;

    public void HidePanel(UIPanel panel) {
        panel.Hide();
    }

    public void ShowPanel(UIPanel panel) {
        panel.Show();
    }

    public void TogglePanel(UIPanel panel) {
        if (panel.IsOpen) {
            panel.Hide();
        } else {
            panel.Show();
        }
    }
}
