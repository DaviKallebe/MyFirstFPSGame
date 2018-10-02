using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform fuelAmount;
    [SerializeField]
    private Text health;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Text ammo;
    [SerializeField]
    private Image ammoBar;
    [SerializeField]
    private Image damageCrosshair;

    private PlayerControl playerControl;
    private Player player;
    private PlayerWeapon currentWeapon;

    public void setPlayerController(PlayerControl playerControl) {
        this.playerControl = playerControl;
    }

    public void setPlayer(Player player) {
        this.player = player;
    }

    public void setCurrentWeapon(PlayerWeapon playerWeapon) {
        this.currentWeapon = playerWeapon;
    }

    void setFuelAmount(float fuelAmount) {
        this.fuelAmount.localScale = new Vector3(1f, fuelAmount, 1f);
    }

    void setHealth(float currentHealth, float maxHealth) {
        this.health.text = currentHealth.ToString("F0") + "/" + maxHealth.ToString("F0");
        this.healthBar.fillAmount = currentHealth / maxHealth;
    }

    void setAmmo(float currentAmmo, float maxAmmo) {
        this.ammo.text = currentAmmo.ToString("F0") + "/" + maxAmmo.ToString("F0");
        this.ammoBar.fillAmount = currentAmmo / maxAmmo;
    }

    public void showDamageCrossHair() {        
        damageCrosshair.CrossFadeAlpha(1, 0f, false);
        damageCrosshair.CrossFadeAlpha(0, 0.2f, false);
    }

    public void resetCrosshair() {
        Color c = damageCrosshair.color;
        c.a = 1;

        damageCrosshair.color = c;
        damageCrosshair.CrossFadeAlpha(0, 0f, false);
    }

    private void Update() {
        setFuelAmount(playerControl.getThrusterFuelAmount());
        setHealth(player.getCurrentHealth(), player.getMaxHealth());
        setAmmo(currentWeapon.weaponCurrentAmmo, currentWeapon.weaponClipSize);
    }
}
