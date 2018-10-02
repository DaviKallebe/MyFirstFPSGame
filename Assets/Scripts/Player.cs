using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool isDead = false;
    public bool IsDead {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 200;
    [SyncVar]
    private int currentHeath;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject RespawnEffect;

    [ClientRpc]
    public void RpcTakeDamage(int weaponDamage) {
        if (isDead)
            return;

        currentHeath -= weaponDamage;

        Debug.Log(transform.name + " took " + weaponDamage + " damage, " + currentHeath + "/" + maxHealth + " health.");

        if (currentHeath <= 0)
            Died();
    }

    public float getCurrentHealth() {
        return currentHeath;
    }

    public float getMaxHealth() {
        return maxHealth;
    }

    //private void Update() {
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K)) {
    //        RpcTakeDamage(99999);
    //    }
    //}

    private void Died() {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableGameObjectOnDeath.Length; i++) {
            disableGameObjectOnDeath[i].SetActive(false);
        }

        Collider playerCollider = GetComponent<Collider>();

        if (playerCollider != null)
            playerCollider.enabled = false;

        //Death effect
        GameObject deathEffectInstance = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffectInstance, 3f);

        if (isLocalPlayer) {
            GameManager.instance.setSceneCameraState(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " rest in pepperoni.");

        //respaw 
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn () {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        
        Transform startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;

        setDefaults();

        Debug.Log(transform.name + " has respawned.");
    }

    public void Setup() {
        wasEnabled = new bool[disableOnDeath.Length];

        for (int i = 0; i < wasEnabled.Length; i++) {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        setDefaults();
    }

    public void setDefaults() {
        isDead = false;
        currentHeath = maxHealth;

        for (int i = 0; i < disableGameObjectOnDeath.Length; i++) {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        for (int i = 0; i < disableOnDeath.Length; i++) {
            if (disableOnDeath[i] != null)
                disableOnDeath[i].enabled = wasEnabled[i];
        }        

        Collider playerCollider = GetComponent<Collider>();

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (isLocalPlayer) {
            GameManager.instance.setSceneCameraState(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.GetComponent<PlayerUI>().resetCrosshair();
        }

        //Death effect
        GameObject RespawnEffectInstance = (GameObject)Instantiate(RespawnEffect, transform.position, Quaternion.identity);
        Destroy(RespawnEffectInstance, 3f);
    }
}
