using System;
using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityStandardAssets.Characters.FirstPerson;
using BeardedManStudios.Forge.Networking.Generated;

[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (AudioSource))]
public class FirstPersonControllerCustom : PlayerBehavior
{
    [SerializeField] private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
	[SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private float m_StepInterval;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;
    private AudioSource m_AudioSource;

    PlayerData playerData;

    float speed;
    public bool inputLocked = false;

    // Use this for initialization
    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = GetComponentInChildren<Camera>();
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle/2f;
        m_Jumping = false;
        m_AudioSource = GetComponent<AudioSource>();
        m_MouseLook.Init(transform , m_Camera.transform);
        playerData = GetComponent<PlayerData>();
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();

        networkObject.UpdateInterval = 100;

        if (networkObject.IsOwner)
        {
            transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = GetComponentInChildren<Camera>();
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform , m_Camera.transform);
            playerData = GetComponent<PlayerData>();
        }
        // TODO:  Your initialization code that relies on network setup for this object goes here
    }

//    private void Update()
//    {
//        // If we are not the owner of this network object then we should
//        // move this cube to the position/rotation dictated by the owner
//        if (!networkObject.IsOwner)
//        {
//            transform.position = networkObject.position;
//            transform.rotation = networkObject.rotation;
//            return;
//        }
//
//        // Let the owner move the cube around with the arrow keys
//        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f) * speed * Time.deltaTime;
//
//        // If we are the owner of the object we should send the new position
//        // and rotation across the network for receivers to move to in the above code
//        networkObject.position = transform.position;
//        networkObject.rotation = transform.rotation;
//
//        // Note: Forge Networking takes care of only sending the delta, so there
//        // is no need for you to do that manually
//    }

    // Update is called once per frame
    private void Update()
    {
		if(networkObject != null) {
			if(!networkObject.IsOwner) {
				transform.position = networkObject.position;
				transform.rotation = networkObject.rotation;
				return;
			}

//            if (!inputLocked)
//            {
//
//                RotateView();
//                // the jump state needs to read here to make sure it is not missed
//                if (!m_Jump && m_CharacterController.isGrounded)
//                {
//                    m_Jump = CrossPlatformInputManager.GetButton("Jump");
//                }
//                GetInput();
//            }
//            else
//            {
//                speed = 0;
//            }
		}
//        else
//        {
//            return;
//        }

        if (!inputLocked)
        {

            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump && m_CharacterController.isGrounded)
            {
                m_Jump = PlayerInputManager.input.Jump.IsPressed;// CrossPlatformInputManager.GetButton("Jump");
            }
            GetInput();
        }
        else
        {
            speed = 0;
        }

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;

		if(networkObject != null) {
			networkObject.position = transform.position;
			networkObject.rotation = transform.rotation;
		}
    }


    private void PlayLandingSound()
    {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }


    private void FixedUpdate()
    {
        //        if (networkObject != null)
        //        {
        //            if (!networkObject.IsOwner)
        //            {
        //                transform.position = networkObject.position;
        //                transform.rotation = networkObject.rotation;
        //                return;
        //            }
        //        }
        //        else
        //        {
        ////            return;
        //        }

        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                           m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x*speed;
        m_MoveDir.z = desiredMove.z*speed;


        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;
            }
        }
        else
        {
            UnityEngine.Profiling.Profiler.BeginSample("Not grounded");
            m_MoveDir.x += Physics.gravity.x * m_GravityMultiplier * Time.fixedDeltaTime;
            m_MoveDir.y += Physics.gravity.y * m_GravityMultiplier * Time.fixedDeltaTime;
            m_MoveDir.z += Physics.gravity.z * m_GravityMultiplier * Time.fixedDeltaTime;
            UnityEngine.Profiling.Profiler.EndSample();
        }

        UnityEngine.Profiling.Profiler.BeginSample("Actual Move");
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.deltaTime);
        UnityEngine.Profiling.Profiler.EndSample();

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);

        //            m_MouseLook.UpdateCursorLock();
    }


    private void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }


    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }


    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }

	/// <summary>
	/// Resets the velocity. Usually used for respawning to zero out gravity effects
	/// </summary>
	public void ResetVelocity() {
		m_MoveDir = Vector3.zero;
	}

    private void GetInput()
    {
        // Read input
        float horizontal = PlayerInputManager.input.Move.X;//CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = PlayerInputManager.input.Move.Y;//CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        m_IsWalking = !PlayerInputManager.input.Sprint.IsPressed;//Input.GetKey(KeyCode.LeftShift);
        m_Input = new Vector2(horizontal, vertical);

        if(!m_IsWalking && m_Input.sqrMagnitude > 0) {
            if(playerData.currentStamina > 50f * Time.deltaTime) {
                playerData.currentStamina -= 40f * Time.deltaTime;
                playerData.usingStam = true;
            } else {
                m_IsWalking = true;
//                  playerData.usingStam = false;
            }
        } else {
            playerData.usingStam = false;
        }
#endif
        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation (transform, m_Camera.transform);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
    }
}
