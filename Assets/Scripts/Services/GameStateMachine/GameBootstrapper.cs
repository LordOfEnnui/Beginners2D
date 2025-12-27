using UnityEngine;
using Zenject;

public class GameBootstrapper : MonoBehaviour {
    [Inject] private IStateMachine stateMachine;

    private void Start() {
        stateMachine.ChangeState<BootstrapState>();
    }

    private void OnApplicationQuit() {
        if (stateMachine != null)
        stateMachine.ChangeState<ExitState>();
    }
}