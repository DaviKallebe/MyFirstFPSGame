using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIOptions : MonoBehaviour {
    [SerializeField]
    private Slider sliderMouseSensitivity;
    [SerializeField]
    private InputField InputMouseSensitivity;
    [SerializeField]
    private float mouseSensitivity;
    [SerializeField]
    private NetworkManagerHUD networkManagerHUD;

    private PlayerShoot playerShoot;
    private PlayerControl playerControl;
    private Canvas canvas;


    private void Start() {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;        

        if (networkManagerHUD == null) {
            networkManagerHUD = FindObjectOfType<NetworkManagerHUD>();
        }
    }
    private void sliderOnValueChanged() {
        InputMouseSensitivity.text = sliderMouseSensitivity.value.ToString("F2");
        mouseSensitivity = sliderMouseSensitivity.value;
    }

    private void inputOnValueChanged() {
        float result;

        if (float.TryParse(InputMouseSensitivity.text, out result)) {
            sliderMouseSensitivity.value = Mathf.Clamp(result,
                sliderMouseSensitivity.minValue,
                sliderMouseSensitivity.maxValue);

            mouseSensitivity = sliderMouseSensitivity.value;
        }
    }

    public void openOptions() {
        playerShoot.setUIOpended(true);
        playerControl.setUIOpended(true);

        sliderMouseSensitivity.value = mouseSensitivity;
        sliderMouseSensitivity.onValueChanged.AddListener(delegate { sliderOnValueChanged(); });

        InputMouseSensitivity.text = sliderMouseSensitivity.value.ToString("F2");
        InputMouseSensitivity.onValueChanged.AddListener(delegate { inputOnValueChanged(); });

        canvas.enabled = true;
        networkManagerHUD.showGUI = false;
    }

    public void closeOptions() {
        playerShoot.setUIOpended(false);
        playerControl.setUIOpended(false);

        canvas.enabled = false;
        networkManagerHUD.showGUI = true;
    }

    public void setPlayerShoot(PlayerShoot playerShoot) {
        this.playerShoot = playerShoot;
    }

    public void setPlayerControl(PlayerControl playerControl) {
        this.playerControl = playerControl;
    }

    public float getMouseSensitivity() {
        return this.mouseSensitivity;
    }
}
