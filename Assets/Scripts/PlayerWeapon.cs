using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    public string weaponName = "Sci-Fi Automatic";
    public int weaponDamage = 10;
    public float weaponRange = 100f;
    public float weaponFirerate = 0f;
    public float weaponClipSize = 20f;
    public float weaponCurrentAmmo = 20f;
    public float weaponReloadTime = 1.561f;
    public GameObject weaponGraphic;
    public AudioSource weaponSound;
    public AudioSource weaponReload;
    public AudioSource weaponBoltPull;
    public AudioSource weaponClipOut;
    public AudioSource weaponClipIn;

    public void Setup() {
        this.weaponCurrentAmmo = this.weaponClipSize;
    }    
}
