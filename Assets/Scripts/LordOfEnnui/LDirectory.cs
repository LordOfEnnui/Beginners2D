using UnityEngine;

public class LDirectory : MonoBehaviour {
    public static LDirectory Instance;

    public GameObject player;
    public PlayerController playerController;
    public GameObject pCamera;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(Instance);
        }
    }

}
