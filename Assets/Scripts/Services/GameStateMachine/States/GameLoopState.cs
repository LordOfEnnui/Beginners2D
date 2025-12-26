using UnityEngine;

public class GameLoopState : State {
    private IAudioService _audioService;

    public GameLoopState(IStateMachine stateMachine, IAudioService audioService) : base(stateMachine) {
        _audioService = audioService;
    }

    public override void Enter() {
        _audioService.StartMusicPlaylist(MusicPlaylist.GameLoop);
    }


    public override void Exit() {
    }
}


