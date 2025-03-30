using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlControllerB : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("BossFight");
        }
    }
}
