using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerControl : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float thrusterForce = 1000f;    
    [SerializeField]
    private float thrusterFuelBurnSpeed = 0.01f;
    [SerializeField]
    private float thrusterFuelRegen = 0.3f;
    private float thrusterFuelTank = 1f;
    private float thrusterFuelAmount = 1f;
    [SerializeField]
    private LayerMask enviromentLayerMask;
    [Header("Spring Settings")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;
    [HideInInspector]
    public UIOptions uiOptions;

    //components
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;
    private bool isUIOpened;    

    
    public float getThrusterFuelAmount() {
        return thrusterFuelAmount / thrusterFuelTank;
    }

    void Start() {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        isUIOpened = false;

        SetJointSettings(jointSpring);
    }    

    void Update() {
        //physics to ground
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, enviromentLayerMask)) {
            joint.targetPosition = new Vector3(0, -hit.point.y , 0);
        }
        else {
            joint.targetPosition = new Vector3(0, 0, 0);
        }

        //Menu
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                uiOptions.openOptions();
                Input.ResetInputAxes();
                motor.Rotation(Vector3.zero);
                motor.Move(Vector3.zero);
                motor.CameraRotation(0f);
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                uiOptions.closeOptions();
                Input.ResetInputAxes();
            }
        }
        //
        //
        if (isUIOpened)
            return;
        //
        //calculate movement velocity
        float xAxys = Input.GetAxisRaw("Horizontal");
        float zAxys = Input.GetAxis("Vertical");

        Vector3 xMov = transform.right * xAxys;
        Vector3 zMov = transform.forward * zAxys;
        Vector3 velocity = (xMov + zMov).normalized * speed;

        motor.Move(velocity);
        animator.SetFloat("ForwardVelocity", zAxys);

        //
        //rotation player & movement
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0f, yRot, 0f) * uiOptions.getMouseSensitivity();
        motor.Rotation(rotation);            
        //
        //rotation camera
        float xRot = Input.GetAxisRaw("Mouse Y");
        float cameraRotation = xRot * uiOptions.getMouseSensitivity();

        motor.CameraRotation(cameraRotation);

        //
        //thruster
        Vector3 thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0) {

            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01) {
                thrusterForce = Vector3.up * this.thrusterForce;
                SetJointSettings(0f);
            }
        } 
        else {
            thrusterFuelAmount += thrusterFuelRegen * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0, thrusterFuelTank);

        motor.ApplyThruster(thrusterForce);             
    }

    public void setUIOpended(bool value) {
        isUIOpened = value;
    }

    private void SetJointSettings(float jointSpring) {
        if (joint != null) {
            joint.yDrive = new JointDrive {
                positionSpring = jointSpring,
                maximumForce = jointMaxForce
            };
        }
    }
}
