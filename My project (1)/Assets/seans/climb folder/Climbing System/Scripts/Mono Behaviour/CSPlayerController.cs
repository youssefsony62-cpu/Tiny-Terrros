using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using DiasGames.Components;

namespace DiasGames.Controller
{
    public class CSPlayerController : MonoBehaviour
    {
        // Components
        private AbilityScheduler _scheduler = null;
        private Health _health = null;
        private IMover _mover;
        private ICapsule _capsule;

        private const float _threshold = 0.01f;

        [SerializeField] private bool hideCursor = true;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        [Tooltip("Speed of camera turn")]
        public Vector2 CameraTurnSpeed = new Vector2(300.0f, 200.0f);
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // for shooter ui
        public float CurrentRecoil { get; private set; } = 0f;
        private float recoilReturnVel = 0;

        [Header("Lighter")]
        [SerializeField] private GameObject lighterPrefab;  // Assign your lighter prefab in inspector
        [SerializeField] private Transform handPosition;    // Empty object in hand where lighter spawns
        [SerializeField] private AudioClip flameSound;      // Sound when turning on
        [SerializeField] private float lightIntensity = 2f;
        [SerializeField] private float lightRange = 3f;
        [SerializeField] private float flickerAmount = 0.3f;

        private GameObject currentLighter;
        private Light lighterLight;
        private AudioSource audioSource;
        private bool isLighterOut = false;

        private void Awake()
        {
            _scheduler = GetComponent<AbilityScheduler>();
            _health = GetComponent<Health>();
            _mover = GetComponent<IMover>();
            _capsule = GetComponent<ICapsule>();

            if (hideCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // set right angle on start for camera
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.eulerAngles.y;

            // Prepare audio source for lighter sound
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            _scheduler.OnUpdatedAbilities += ResetActions;
#endif
            if (_health != null)
                _health.OnDead += Die;
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            _scheduler.OnUpdatedAbilities -= ResetActions;
#endif
            if (_health != null)
                _health.OnDead -= Die;
        }

        private void Update()
        {
            UpdateCharacterActions();

            if (CurrentRecoil > 0)
                CurrentRecoil = Mathf.SmoothDamp(CurrentRecoil, 0, ref recoilReturnVel, 0.2f);

#if ENABLE_LEGACY_INPUT_MANAGER
            LegacyInput();
#endif

            // --- LIGHTER FEATURE ---
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isLighterOut)
                    SpawnLighter();
                else
                    HideLighter();
            }

            if (isLighterOut && lighterLight != null)
            {
                float randomFlicker = Mathf.PerlinNoise(Time.time * 10f, 0f);
                lighterLight.intensity = lightIntensity + (randomFlicker - 0.5f) * flickerAmount;
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void Die()
        {
            _scheduler.StopScheduler();

            _mover.DisableGravity();
            _mover.StopMovement();

            _capsule.DisableCollision();

            _mover.ApplyRootMotion(Vector3.one);
        }

        private void CameraRotation()
        {
            if (Look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                _cinemachineTargetYaw += Look.x * CameraTurnSpeed.x * Time.deltaTime;
                _cinemachineTargetPitch += Look.y * CameraTurnSpeed.y * Time.deltaTime;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void UpdateCharacterActions()
        {
            _scheduler.characterActions.move = Move;
            _scheduler.characterActions.jump = Jump;
            _scheduler.characterActions.walk = Walk;
            _scheduler.characterActions.crouch = Crouch;
            _scheduler.characterActions.interact = Interact;
            _scheduler.characterActions.drop = Drop;
            _scheduler.characterActions.zoom = Zoom;
        }

        #region Input receiver

        [Header("Input")]
        public Vector2 Move = Vector2.zero;
        public Vector2 Look = Vector2.zero;
        public bool Jump = false;
        public bool Walk = false;
        public bool Crouch = false;
        public bool Interact = false;
        public bool Zoom = false;
        public bool Drop = false;

        public void ResetActions()
        {
            Jump = false;
            Interact = false;
            Drop = false;
        }

        public void LegacyInput()
        {
            Move.x = Input.GetAxis("Horizontal");
            Move.y = Input.GetAxis("Vertical");

            Look.x = Input.GetAxis("Mouse X");
            Look.y = Input.GetAxis("Mouse Y");

            Walk = Input.GetButton("Walk");
            Jump = Input.GetButtonDown("Jump");
            Crouch = Input.GetButton("Crouch");
            Zoom = Input.GetButtonDown("Zoom");
            Interact = Input.GetButtonDown("Interact");
            Drop = Input.GetButtonDown("Drop");
        }

        public void OnMove(Vector2 value) => Move = value;
        public void OnLook(Vector2 value) => Look = value;
        public void OnJump(bool value) => Jump = value;
        public void OnWalk(bool value) => Walk = value;
        public void OnCrouch(bool value) => Crouch = value;
        public void OnZoom(bool value) => Zoom = value;
        public void OnInteract(bool value) => Interact = value;
        public void OnDrop(bool value) => Drop = value;

#if ENABLE_INPUT_SYSTEM
        private void OnMove(InputValue value) => OnMove(value.Get<Vector2>());
        private void OnLook(InputValue value) => OnLook(value.Get<Vector2>());
        private void OnJump(InputValue value) => OnJump(value.isPressed);
        private void OnWalk(InputValue value) => OnWalk(value.isPressed);
        private void OnCrouch(InputValue value) => OnCrouch(value.isPressed);
        private void OnZoom(InputValue value) => OnZoom(value.isPressed);
        private void OnInteract(InputValue value) => OnInteract(value.isPressed);
        private void OnDrop(InputValue value) => OnDrop(value.isPressed);
#endif

        #endregion

        // --- LIGHTER FUNCTIONS ---
        private void SpawnLighter()
        {
            currentLighter = Instantiate(lighterPrefab, handPosition.position, handPosition.rotation, handPosition);

            lighterLight = currentLighter.AddComponent<Light>();
            lighterLight.type = LightType.Point;
            lighterLight.range = lightRange;
            lighterLight.intensity = lightIntensity;
            lighterLight.color = new Color(1f, 0.85f, 0.6f);

            if (flameSound != null)
                audioSource.PlayOneShot(flameSound);

            isLighterOut = true;
        }

        private void HideLighter()
        {
            if (currentLighter != null)
                Destroy(currentLighter);

            isLighterOut = false;
        }
    }
}
