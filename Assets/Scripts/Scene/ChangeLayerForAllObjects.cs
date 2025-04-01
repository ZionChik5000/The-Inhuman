using UnityEngine;

public class ChangeLayerForDefault : MonoBehaviour
{
    public string layerName = "NewLayer"; // �������� ������ ����
    public string excludeTag = "Player"; // ��������� ������� � ���� �����

    void Start()
    {
        int newLayer = LayerMask.NameToLayer(layerName);
        int defaultLayer = LayerMask.NameToLayer("Default");

        if (newLayer == -1)
        {
            Debug.LogError("���� �� ������! ������� �������� ����.");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int changedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            // ������ ���� ������ ���� ������ �� Default � �� ����� ����������� ���
            if (obj.layer == defaultLayer && obj.tag != excludeTag)
            {
                obj.layer = newLayer;
                changedCount++;
            }
        }

        Debug.Log($"���� {layerName} ���������� ��� {changedCount} ��������.");
    }
}
