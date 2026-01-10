using UnityEngine;
using UnityEngine.Tilemaps;

public class ModulePlacer : MonoBehaviour {
    [SerializeField]
    public GameObject _module;

    [SerializeField]
    public GameObject _platform;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private Tilemap _tilemap;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    public void MakePlatform(int _borderSizeXlw, int _borderSizeYlw, int _borderSizeXhi, int _borderSizeYhi) {

        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin + _borderSizeXlw;
        int lowY = bounds.yMin + _borderSizeYlw;
        int hiX = bounds.xMax - _borderSizeXhi;
        int hiY = bounds.yMax - _borderSizeYhi;

        int pickX = Random.Range(lowX, hiX);
        int pickY = Random.Range(lowY, hiY);

        Debug.Log("Placing module at " + pickX + "," + pickY);

        GameObject platformInstance = Instantiate(_platform);
        platformInstance.transform.position = new Vector3(pickX, pickY, 0);

        Debug.Log("Platform placed");

    }

    public void MakeModules(int _borderSizeXlw, int _borderSizeYlw, int _borderSizeXhi, int _borderSizeYhi, int moduleCount) {
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin + _borderSizeXlw;
        int lowY = bounds.yMin + _borderSizeYlw;
        int hiX = bounds.xMax - _borderSizeXhi;
        int hiY = bounds.yMax - _borderSizeYhi;


        for (int i = 0; i < moduleCount; i++) {
            GameObject module = Instantiate(_module);
            module.transform.position = new Vector3(Random.Range(lowX, hiX), Random.Range(lowY, hiY));
        }
    }
}
