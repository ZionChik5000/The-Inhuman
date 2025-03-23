using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        // Ищем Animator в дочерних объектах Player'а
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator не найден в дочерних объектах!");
        }
    }

    void Update()
    {
        bool isWalking = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        
        if (isWalking)
        {
            anim.Play("Walking");
        }
        else
        {
            anim.Play("Idle");
        }
    }
}
