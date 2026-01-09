using UnityEngine;
using Zenject;

public class GameManager2D : MonoBehaviour
{
    [SerializeField]
    Trigger2D levelExitTrigger;

    [SerializeField]
    PlayerState pState;

    [Inject] private IGameManager gm;
    private void Awake() {
        Application.targetFrameRate = -1;
        Debug.Log($"Injected gm : {gm}");
    }

    void Start()
    {
        if (levelExitTrigger != null) levelExitTrigger.triggerEvent.AddListener(OnLevelComplete);
        pState = LDirectory2D.Instance.pState;
        pState.onDeath.AddListener(OnDeath);
        pState.onSufficientOil.AddListener(SetLevelTriggerState);
    }

    private void SetLevelTriggerState() {
        if (levelExitTrigger != null) levelExitTrigger.SetActive(true);
    }

    public void OnLevelComplete(GameObject player, bool entered) {
        if (entered) Debug.Log("Pog");
    }

    public void OnDeath() {
        Debug.Log("Ded");
    }
}
