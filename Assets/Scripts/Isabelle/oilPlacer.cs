using UnityEngine;
using UnityEngine.Tilemaps;

public class OilPlacer : MonoBehaviour
{
    [SerializeField]
    public GameObject _oil;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private Tilemap _tilemap;

    // [SerializeField]
    // private int oilCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void MakeOil(int _borderSizeXlw, int _borderSizeYlw,int _borderSizeXhi, int _borderSizeYhi,int oilCount){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin+_borderSizeXlw;
        int lowY = bounds.yMin+_borderSizeYlw;
        int hiX = bounds.xMax-_borderSizeXhi;
        int hiY = bounds.yMax-_borderSizeYhi;

        for (int i = 0; i < oilCount; i++) {
            GameObject oil = Instantiate(_oil);
            oil.transform.position = new Vector3(Random.Range(lowX, hiX), Random.Range(lowY, hiY));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
