using UnityEngine;

public class WrenchPickup : MonoBehaviour
{
    public ModuleSelectionUI moduleUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        moduleUI.Show(ModuleDatabaseLoader.LoadedModules);
        Destroy(gameObject);
    }
}
