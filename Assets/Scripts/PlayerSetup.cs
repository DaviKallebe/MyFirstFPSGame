using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(PlayerShoot))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    private string remoteLayerName = "RemotePlayer";
    [SerializeField]
    private string IgnoreDrawLayerName = "IgnoreDraw";
    [SerializeField]
    private GameObject playerGraphics;
    [SerializeField]
    private GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;
    [SerializeField]
    private GameObject uiOptionsPrefab;
    [HideInInspector]
    public GameObject uiOptionInstance;

    void Start() {
        if (!isLocalPlayer) {
            DisableComponents();
            AssignRemoteLayer();            
        }   
        else {
            //Disable local graphics
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(IgnoreDrawLayerName));

            //UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            //Options
            uiOptionInstance = Instantiate(uiOptionsPrefab);
            uiOptionInstance.name = playerUIPrefab.name;

            PlayerUI playerUI = playerUIInstance.GetComponent<PlayerUI>();
            UIOptions uiOptions = uiOptionInstance.GetComponent<UIOptions>();
            Player player = GetComponent<Player>();            

            if (playerUI == null)
                Debug.Log("No PlayerUI component in UI.");
            else {
                PlayerControl pc = GetComponent<PlayerControl>();
                playerUI.setPlayerController(pc);                
                playerUI.setPlayer(player);
                uiOptions.setPlayerControl(pc);
                uiOptions.setPlayerShoot(GetComponent<PlayerShoot>());

                GetComponent<PlayerControl>().uiOptions = uiOptions;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;            
        }

        GetComponent<Player>().Setup();
    }

    public override void OnStartClient() {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netID, player);
    }
    
    void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents() {
        for (int i = 0; i < componentsToDisable.Length; ++i) {
            componentsToDisable[i].enabled = false;
        }
    }

    void SetLayerRecursively(GameObject gameObject, int layer) {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    void OnDisable() {
        Destroy(playerUIInstance);

        if (isLocalPlayer)
            GameManager.instance.setSceneCameraState(true);

        GameManager.UnRegisterPlayer(transform.name);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
