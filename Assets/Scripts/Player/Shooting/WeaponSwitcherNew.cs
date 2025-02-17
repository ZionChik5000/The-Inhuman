using UnityEngine;

public class WeaponSwitcherNew : MonoBehaviour
{
    [SerializeField] protected Camera fpsCam;
    [SerializeField] private PlayerMovementAdvanced playerMovement;
    [SerializeField] protected LineRenderer lineRenderer;

    public WeaponBase[] weaponPrefabs;
    public Transform weaponHolder;
    public ParticleSystem switchEffect;

    private int currentWeaponIndex;
    private GameObject currentWeapon;
    private WeaponBase currentWeaponScript;
    private float switchCooldown = 0.5f;
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
        currentWeaponScript = Instantiate(weaponPrefabs[index], weaponHolder);
        currentWeaponScript.Initialize(fpsCam, lineRenderer, playerMovement);
        currentWeapon = currentWeaponScript.gameObject;
        currentWeapon.gameObject.SetActive(true);
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
