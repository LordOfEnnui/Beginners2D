using UnityEngine;
using TMPro;

public class ModuleButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private ModuleJson moduleData;
    private ModuleSelectionUI selectionUI;

    public void Setup(ModuleJson data, ModuleSelectionUI ui)
    {
        moduleData = data;
        selectionUI = ui;

        nameText.text = data.name;
        descriptionText.text = data.description;
    }

    public void OnClick()
    {
        selectionUI.SelectModule(moduleData);
    }
}
