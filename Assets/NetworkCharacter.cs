using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {
    public float Speed = 10f;
    public float JumpSpeed = 6f;

    [HideInInspector]
    public Vector3 Direction = Vector3.zero;
    [HideInInspector]
    public bool IsJumping = false;
    [HideInInspector]
    public float AimAngle = 0f;

    // Use this for initialization
    void Start() {
        CacheComponents();
    }

    // Update is called once per frame
    void Update() {

    }

    void CacheComponents() {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (charController == null)
            charController = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        if (photonView.isMine) {
            DoLocalMovement();
        } else {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            animator.SetFloat("AimAngle", Mathf.Lerp(animator.GetFloat("AimAngle"), realAimAngle, 0.1f));
        }
    }

    void DoLocalMovement() {
        Vector3 distance = Direction * Speed * Time.deltaTime;

        if (IsJumping) {
            IsJumping = false;
            if (charController.isGrounded) {
                verticalVelocity = JumpSpeed;
            }
        }

        if (charController.isGrounded && verticalVelocity < 0) {
            animator.SetBool("Jumping", false);
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        } else {
            if (Mathf.Abs(verticalVelocity) > JumpSpeed * 0.75f) {
                animator.SetBool("Jumping", true);
            }

            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        distance.y = verticalVelocity * Time.deltaTime;

        animator.SetFloat("AimAngle", AimAngle);
        animator.SetFloat("Speed", Direction.magnitude);

        charController.Move(distance);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // OnPhotonSerializeView is called before Start sometimes, giving an NRE on animator.
        // This call prevents that. It's unfortunate though because this is called unnecessarily several times per second for the rest of the game
        // when it is not needed.
        CacheComponents();

        if (stream.isWriting) {
            // Our player. Need to send our actual position to network.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));    // Getting NRE here when joining with 2nd player
            stream.SendNext(animator.GetBool("Jumping"));
            stream.SendNext(animator.GetFloat("AimAngle"));
        } else {
            // Someone else's player. Need to receive their position (as of a few milliseconds ago) and update our version of them.
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("Jumping", (bool)stream.ReceiveNext());
            realAimAngle = (float)stream.ReceiveNext();

            if (!gotFirstUpdate) {
                gotFirstUpdate = true;
                transform.position = realPosition;
                transform.rotation = realRotation;
                animator.SetFloat("AimAngle", realAimAngle);
            }
        }
    }

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    float realAimAngle = 0f;
    Animator animator;
    bool gotFirstUpdate = false;
    CharacterController charController;
    float verticalVelocity = 0f;
}
