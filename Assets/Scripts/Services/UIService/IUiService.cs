public interface IUiService {
    LoadingScreenUI LoadingUI { get; }
    SettingsMenuUI SettingsMenu { get; }
    UIPanel PauseUI { get; }

    void HidePanel(UIPanel pauseUI);
    void ShowPanel(UIPanel pauseUI);

    void TogglePanel(UIPanel panel);
}
