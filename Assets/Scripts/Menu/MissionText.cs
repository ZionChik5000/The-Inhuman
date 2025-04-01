using UnityEngine;
using TMPro;
using System.Collections;

public class MissionTextTMP : MonoBehaviour
{
    public TextMeshProUGUI missionText; // ������ �� TextMeshPro-�����
    public float displayTime = 3f; // ����� ������ ������
    public float fadeDuration = 1f; // ����� ������������

    void Start()
    {
        if (missionText != null)
        {
            missionText.text = "Objective: Find the bunker"; // ������������� �����
            missionText.alpha = 1f; // ������ ���������
            StartCoroutine(FadeOutText());
        }
    }

    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(displayTime); // ��� 3 �������

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            missionText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        missionText.gameObject.SetActive(false); // ��������� ����� ����� ������������
    }
}
