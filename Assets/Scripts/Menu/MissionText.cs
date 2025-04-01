using UnityEngine;
using TMPro;
using System.Collections;

public class MissionTextTMP : MonoBehaviour
{
    public TextMeshProUGUI missionText; // Ссылка на TextMeshPro-текст
    public float displayTime = 3f; // Время показа текста
    public float fadeDuration = 1f; // Время исчезновения

    void Start()
    {
        if (missionText != null)
        {
            missionText.text = "Objective: Find the bunker"; // Устанавливаем текст
            missionText.alpha = 1f; // Полная видимость
            StartCoroutine(FadeOutText());
        }
    }

    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(displayTime); // Ждём 3 секунды

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            missionText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        missionText.gameObject.SetActive(false); // Отключаем текст после исчезновения
    }
}
