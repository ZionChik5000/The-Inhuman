using UnityEngine;

public class DoorController2 : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        CheckEnemies();
    }

    void Update()
    {
        CheckEnemies();
    }

    void CheckEnemies()
    {
        bool enemiesExist = AreEnemiesPresent();

        if (enemiesExist)
        {
            animator.Play("Closed2");
        }
        else
        {
            animator.Play("OpenDoor2");
        }
    }

    bool AreEnemiesPresent()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == enemyLayer)
            {
                return true;
            }
        }
        return false;
    }
}
