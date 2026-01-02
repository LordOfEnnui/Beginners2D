using UnityEngine;
using System.Collections.Generic;

public class ModuleSelectionUI : MonoBehaviour
{
    public GameObject panel;
    public ModuleButton[] moduleButtons;

    public void Show(List<ModuleJson> allModules)
    {
        panel.SetActive(true);
        Time.timeScale = 0f; // pause game

        List<ModuleJson> choices = GetRandomModules(allModules, 3);

        for (int i = 0; i < moduleButtons.Length; i++)
        {
            moduleButtons[i].Setup(choices[i], this);
        }
    }

    public void SelectModule(ModuleJson selected)
    {
        //WHEN WE CAN ACTUALLY ADD MODULES, for now just closes, modules selection is not saved

        //FindObjectOfType<PlayerModuleController>()
          //  .ApplyModule(selected);

        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    List<ModuleJson> GetRandomModules(List<ModuleJson> modules, int count)
    {
        List<ModuleJson> pool = new List<ModuleJson>(modules);
        List<ModuleJson> result = new List<ModuleJson>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }
}
