using UnityEngine;

public class ChangeLayerForDefault : MonoBehaviour
{
    public string layerName = "NewLayer"; // Название нового слоя
    public string excludeTag = "Player"; // Исключаем объекты с этим тегом

    void Start()
    {
        int newLayer = LayerMask.NameToLayer(layerName);
        int defaultLayer = LayerMask.NameToLayer("Default");

        if (newLayer == -1)
        {
            Debug.LogError("Слой не найден! Проверь название слоя.");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int changedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            // Меняем слой только если объект на Default и не имеет исключённый тег
            if (obj.layer == defaultLayer && obj.tag != excludeTag)
            {
                obj.layer = newLayer;
                changedCount++;
            }
        }

        Debug.Log($"Слой {layerName} установлен для {changedCount} объектов.");
    }
}
