using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;
    private Rigidbody rb;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity) {
        this.velocity = velocity;
    }

    public void Rotation(Vector3 rotation) {
        this.rotation = rotation;
    }

    public void CameraRotation(float cameraRotationX) {
        this.cameraRotationX = cameraRotationX;
    }

    public void ApplyThruster(Vector3 thrusterForce) {
        this.thrusterForce = thrusterForce;
    }

    void FixedUpdate() {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement() {
        if (this.velocity != Vector3.zero) {
            rb.MovePosition(rb.position + this.velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero) {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation() {
        if (this.rotation != Vector3.zero) {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }

        if (cam != null) {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
