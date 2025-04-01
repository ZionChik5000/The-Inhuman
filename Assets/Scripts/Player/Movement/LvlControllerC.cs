using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlControllerC : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("BunkerScene");
        }
    }
}
