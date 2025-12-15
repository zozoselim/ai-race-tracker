using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countdownText;

    [Header("Disable these scripts until GO")]
    public MonoBehaviour[] scriptsToDisable;

    [Header("Timing")]
    public float stepDuration = 1f;
    public float goDuration = 0.7f;

    private void Start()
    {
        Debug.Log("COUNTDOWN START");

        // Başta hepsini kapat
        SetScriptsEnabled(false, "START-DISABLE");

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
        }
        yield return new WaitForSeconds(stepDuration);

        if (countdownText != null) countdownText.text = "2";
        yield return new WaitForSeconds(stepDuration);

        if (countdownText != null) countdownText.text = "1";
        yield return new WaitForSeconds(stepDuration);

        // GO
        if (countdownText != null) countdownText.text = "GO!";
        Debug.Log("GO! enabling scripts");

        SetScriptsEnabled(true, "GO-ENABLE");

        yield return new WaitForSeconds(goDuration);

        if (countdownText != null) countdownText.gameObject.SetActive(false);

        Debug.Log("COUNTDOWN END");
    }

    private void SetScriptsEnabled(bool enabled, string phase)
    {
        if (scriptsToDisable == null)
        {
            Debug.LogWarning($"{phase}: scriptsToDisable is NULL");
            return;
        }

        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            var s = scriptsToDisable[i];
            if (s == null)
            {
                Debug.LogWarning($"{phase}: Element {i} is NULL");
                continue;
            }

            s.enabled = enabled;
            Debug.Log($"{phase}: {(enabled ? "ENABLED" : "DISABLED")} -> {s.GetType().Name} on {s.gameObject.name} (now enabled={s.enabled})");
        }
    }
}
