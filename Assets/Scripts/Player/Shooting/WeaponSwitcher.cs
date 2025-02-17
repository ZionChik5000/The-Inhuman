using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    public Transform weaponHolder;
    public ParticleSystem switchEffect;

    private int currentWeaponIndex;
    private GameObject currentWeapon;

    void Start()
    {
        currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeaponIndex", 0);
        EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
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
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

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