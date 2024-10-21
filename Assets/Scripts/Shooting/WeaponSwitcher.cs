using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weaponPrefabs;  // Array of weapon prefabs
    public Transform weaponHolder;  // Point of attachment of weapon to camera
    public ParticleSystem switchEffect;  // Weapon change effect

    private int currentWeaponIndex;
    private GameObject currentWeapon;

    void Start()
    {
        // Load the current weapon from the save
        currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeaponIndex", 0);
        EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // Checking the mouse wheel scrolling
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
            PlaySwitchEffect();
            EquipWeapon(currentWeaponIndex);
        }
        else if (scroll < 0f)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weaponPrefabs.Length - 1;
            }
            PlaySwitchEffect();
            EquipWeapon(currentWeaponIndex);
        }
    }

    void EquipWeapon(int index)
    {
        // Remove current weapon
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Create and bind new weapons
        currentWeapon = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // Save the current weapon index
        PlayerPrefs.SetInt("CurrentWeaponIndex", index);
        PlayerPrefs.Save();
    }

    void PlaySwitchEffect()
    {
        if (switchEffect != null)
        {
            switchEffect.transform.position = weaponHolder.position;
            switchEffect.Play();
        }
    }
}
