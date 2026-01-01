using UnityEngine;

public class UIManager2D : MonoBehaviour
{
    [SerializeField]
    UIBar healthBar, oilBar;

    [SerializeField]
    PlayerState pState;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
    }

    private void Update() {
        healthBar.UpdateComponent(pState.currentHealth, pState.maxHealth);
        oilBar.UpdateComponent(pState.currentOil, pState.maxOil);
    }
}
