using UnityEngine;

public class MainMenuState : State {
    private IAudioService _audioService;
    public MainMenuState(IStateMachine stateMachine, IAudioService audioService) : base(stateMachine) {
        _audioService = audioService;
    }
    public override void Enter() {
        _audioService.StartMusicPlaylist(MusicPlaylist.MainMenu);
    }
    public override void Exit() {
    }
}