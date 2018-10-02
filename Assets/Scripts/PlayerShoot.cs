using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(PlayerSetup))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";
        
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;
    private IEnumerator reloading;
    private PlayerUI playerUI;
    private bool isUIOpened;

    public void Start() {
        if (playerCamera == null) {
            Debug.Log("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
        currentWeapon = weaponManager.getCurrentWeapon();
        reloading = null;
        playerUI = GetComponent<PlayerSetup>().playerUIInstance.GetComponent<PlayerUI>();
        isUIOpened = false;
    }

    public void Update() {
        currentWeapon = weaponManager.getCurrentWeapon();

        if (isUIOpened) {
            CancelInvoke("Shoot");

            return;
        }

        if (reloading != null)
            return;

        if (currentWeapon.weaponFirerate == 0f) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        } else {
            if (Input.GetButtonDown("Fire1")) {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.weaponFirerate);
            } else if (Input.GetButtonUp("Fire1")) {
                CancelInvoke("Shoot");
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            CancelInvoke("Shoot");
            CmdReload();
        }
        
    }  
    
    public void setUIOpended(bool value) {
        isUIOpened = value;
    }  

    [Command]
    void CmdReload() {
        RpcReload();
    }

    [ClientRpc]
    void RpcReload() {
        reloading = Reload();

        if (weaponManager.getCurrentWeaponReload() != null)
            weaponManager.getCurrentWeaponReload().Play();

        StartCoroutine(reloading);
    }

    public IEnumerator Reload() {
        if (currentWeapon.weaponClipSize > currentWeapon.weaponCurrentAmmo) {            
            yield return new WaitForSeconds(currentWeapon.weaponReloadTime);

            currentWeapon.weaponCurrentAmmo = currentWeapon.weaponClipSize;
        }

        reloading = null;
    }

    [Command]
    void CmdOnShoot() {
        RpcShowMuzzleFlashEffect();
        RpcPlayWeaponSound();
    }

    [ClientRpc]
    void RpcShowMuzzleFlashEffect() {
        weaponManager.getCurrentGraphics().muzzleFlash.Play();
    }
    
    [ClientRpc]
    void RpcPlayWeaponSound() {
        weaponManager.getCurrentWeaponSound().Play();
    }

    [Command]
    void CmdOnHit(Vector3 position, Vector3 normal) {
        RpcHitEffect(position, normal);
    }

    [ClientRpc]
    void RpcHitEffect(Vector3 position, Vector3 normal) {
        GameObject hitEffect = (GameObject)Instantiate(weaponManager.getCurrentGraphics().hitEffectPrefab, position, Quaternion.LookRotation(normal));        
        Destroy(hitEffect, 1);
    }

    [Client]
    void Shoot() {       
        if (!isLocalPlayer)
            return;

        if (currentWeapon.weaponCurrentAmmo <= 0) {
            weaponManager.getCurrentWeaponClipOut().Play();

            return;
        }

        currentWeapon.weaponCurrentAmmo -= 1;

        CmdOnShoot();                           

        RaycastHit shootHit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out shootHit, currentWeapon.weaponRange, mask)) {
            if (shootHit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(shootHit.collider.name, currentWeapon.weaponDamage);
                playerUI.showDamageCrossHair();
            }

            CmdOnHit(shootHit.point, shootHit.normal);
        }
    }

    [Command]
    void CmdPlayerShot(string playerID, int weaponDamage) {
        Player shotedPlayer = GameManager.getPlayer(playerID);
        shotedPlayer.RpcTakeDamage(weaponDamage);
    }
}
