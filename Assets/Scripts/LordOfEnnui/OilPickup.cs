using UnityEngine;

public class OilPickup : MonoBehaviour
{
    public float oilAmount = 20;
    public float lifetime = 20;

    private void Awake() {
        Destroy(gameObject, lifetime);
        gameObject.layer = Layers.Pickup;
    }
}
