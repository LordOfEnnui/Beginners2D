using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Zenject;

public class LoadingMapState : State<GameManager> {
    private readonly ISceneLoader _sceneLoader;
    private readonly ISceneDataService _sceneData;

    public LoadingMapState(
        ISceneLoader sceneLoader,
        ISceneDataService sceneData) {
        _sceneLoader = sceneLoader;
        _sceneData = sceneData;
    }

    public override void Enter() {
        LoadScene().Forget();
    }

    private async UniTaskVoid LoadScene() {
        await _sceneLoader.LoadAsync(_sceneData.GetMapSceneName());
        Context.StateMachine.ChangeState<GameLoopState>();
    }

    public override void Exit() {
    }
}