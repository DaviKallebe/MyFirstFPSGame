using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private PlayerWeapon primaryWeapon;
    private PlayerWeapon currentWeapon;
    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;

    private WeaponGraphics currentGraphics;
    private AudioSource currentWeaponSound;
    private AudioSource currentWeaponReload;
    private AudioSource currentWeaponClipOut;

    private void Start() {
        EquipWeapon(primaryWeapon);
    }

    public PlayerWeapon getCurrentWeapon() {
        return currentWeapon;
    }

    public WeaponGraphics getCurrentGraphics() {
        return currentGraphics;
    }

    public AudioSource getCurrentWeaponSound() {
        return currentWeaponSound;
    }

    public AudioSource getCurrentWeaponReload() {
        return currentWeaponReload;
    }

    public AudioSource getCurrentWeaponClipOut() {
        return currentWeaponClipOut;
    }

    void EquipWeapon(PlayerWeapon playerWeapon) {
        currentWeapon = playerWeapon;
        GameObject weaponInstance = (GameObject)Instantiate(currentWeapon.weaponGraphic, weaponHolder.position, weaponHolder.rotation);
        weaponInstance.transform.SetParent(weaponHolder);

        currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();
        currentWeaponSound = (AudioSource)Instantiate(currentWeapon.weaponSound, weaponHolder.position, Quaternion.identity);
        currentWeaponReload = (AudioSource)Instantiate(currentWeapon.weaponReload, weaponHolder.position, Quaternion.identity);
        currentWeaponClipOut = (AudioSource)Instantiate(currentWeapon.weaponClipOut, weaponHolder.position, Quaternion.identity);
        //currentWeaponSound = weaponInstance.GetComponent<AudioSource>();   
    
        currentWeaponSound.transform.SetParent(weaponHolder);
        currentWeaponReload.transform.SetParent(weaponHolder);
        currentWeaponClipOut.transform.SetParent(weaponHolder);

        if (currentGraphics == null)
            Debug.Log("No WeaponGraphics component was assigned.");

        if (isLocalPlayer) {
            currentWeaponSound.volume = 0.3f;
            currentWeaponReload.volume = 0.3f;
            currentWeaponClipOut.volume = 0.3f;
            Utilites.setLayerRecursively(weaponInstance, LayerMask.NameToLayer(weaponLayerName));

            PlayerUI playerUI = GetComponent<PlayerSetup>().playerUIInstance.GetComponent<PlayerUI>();
            playerUI.setCurrentWeapon(currentWeapon);
        } else {
            currentWeaponSound.volume = 0.5f;
            currentWeaponSound.spatialBlend = 1;
            currentWeaponReload.volume = 1f;
            currentWeaponReload.spatialBlend = 1;
            currentWeaponClipOut.volume = 1f;
            currentWeaponClipOut.spatialBlend = 1;
        }
    }
}
