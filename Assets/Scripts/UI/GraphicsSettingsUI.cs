using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsUI : MonoBehaviour
{
    public Dropdown graphicsDropdown;

    void Start()
    {
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        graphicsDropdown.RefreshShownValue();

        graphicsDropdown.onValueChanged.AddListener(SetGraphicsQuality);
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Графика установлена на: " + QualitySettings.names[index]);
    }
}
