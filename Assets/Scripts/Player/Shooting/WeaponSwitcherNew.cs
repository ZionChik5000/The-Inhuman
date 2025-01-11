using UnityEngine;

public class WeaponSwitcherNew : MonoBehaviour
{
    public GameObject[] weaponPrefabs; // Префабы оружия
    public Transform weaponHolder;    // Точка крепления оружия
    public ParticleSystem switchEffect; // Эффект смены оружия

    private int currentWeaponIndex;
    private GameObject currentWeapon;
    private WeaponBase currentWeaponScript; // Ссылка на скрипт текущего оружия
    private float switchCooldown = 0.5f; // Задержка между переключениями
    private float lastSwitchTime;

    void Start()
    {
        currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeaponIndex", 0);
        EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // Проверяем задержку переключения
        if (Time.time - lastSwitchTime < switchCooldown)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
            lastSwitchTime = Time.time;
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
            lastSwitchTime = Time.time;
            PlaySwitchEffect();
            EquipWeapon(currentWeaponIndex);
        }
    }

    void EquipWeapon(int index)
    {
        // Удаляем текущее оружие
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Создаем новое оружие
        currentWeapon = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // Получаем ссылку на скрипт оружия
        currentWeaponScript = currentWeapon.GetComponent<WeaponBase>();

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
