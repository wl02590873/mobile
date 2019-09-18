using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace FPSControllerLPFP
{

    /// Manages a first person character
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class FpsControllerLPFP : MonoBehaviourPun, IPunObservable
    {
        #region 玩家設定
        [Header("玩家名稱")]
        public Text textName;
        [Header("玩家血量")]
        public float playerHp;

        [Header("生命數量")]
        public float life;
        float lifePC;
        [Header("死亡UI")]
        public GameObject PlayerDeadUI;

        private float playerMaxHp = 100;//最大HP
        [Header("玩家血量文字")]
        public Text textHP;
        [Header("玩家生命文字")]
        public Text textLife;
        #endregion

        #region 玩家控制開關
        [Header("Photon元件")]
        public PhotonView pv;
        [Header("玩家UI")]
        public GameObject playerUI;

        [Header("攝影機")]
        public GameObject cam;
        #endregion
        #region 同步座標資訊
        [Header("同步座標資訊")]
        public Vector3 positionNext;
        public Vector3 RotateNext;
        [Header("同步平滑速度"), Range(0.1f, 50.0f)]
        public float smoothSpeed = 30.0f;
        #endregion

        #region 數值設定
#pragma warning disable 649
        [Header("Arms")]
        [Tooltip("The transform component that holds the gun camera."), SerializeField]
        private Transform arms;

        [Tooltip("The position of the arms and gun camera relative to the fps controller GameObject."), SerializeField]
        private Vector3 armPosition;

        [Header("Audio Clips")]
        [Tooltip("The audio clip that is played while walking."), SerializeField]
        private AudioClip walkingSound;

        [Tooltip("The audio clip that is played while running."), SerializeField]
        private AudioClip runningSound;

        [Header("Movement Settings")]
        [Tooltip("How fast the player moves while walking and strafing."), SerializeField]
        private float walkingSpeed = 5f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float runningSpeed = 9f;

        [Tooltip("Approximately the amount of time it will take for the player to reach maximum running or walking speed."), SerializeField]
        private float movementSmoothness = 0.125f;

        [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
        private float jumpForce = 35f;

        [Header("Look Settings")]
        [Tooltip("Rotation speed of the fps controller."), SerializeField]
        private float mouseSensitivity = 7f;

        [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
        private float rotationSmoothness = 0.05f;

        [Tooltip("Minimum rotation of the arms and camera on the x axis."),
         SerializeField]
        private float minVerticalAngle = -90f;

        [Tooltip("Maximum rotation of the arms and camera on the axis."),
         SerializeField]
        private float maxVerticalAngle = 90f;

        [Tooltip("The names of the axes and buttons for Unity's Input Manager."), SerializeField]
        private FpsInput input;
#pragma warning restore 649
        #endregion
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private AudioSource _audioSource;
        private SmoothRotation _rotationX;
        private SmoothRotation _rotationY;
        private SmoothVelocity _velocityX;
        private SmoothVelocity _velocityZ;
        private bool _isGrounded;

        private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
        private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];

        /// Initializes the FpsController on start.
        private void Start()
        {
            lifePC = life;
            //如果不是自己的物件
            if (!pv.IsMine)
            {
                playerUI.SetActive(false);//玩家UI
                cam.SetActive(false);//玩家攝影機
                GetComponent<PlayerAnimator>().enabled = false;
                textName.text = pv.Owner.NickName;//玩家名稱.文字=元件擁有者.暱稱
            }
            textName.text = PhotonNetwork.NickName;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _collider = GetComponent<CapsuleCollider>();
            _audioSource = GetComponent<AudioSource>();
            arms = AssignCharactersCamera();
            _audioSource.clip = walkingSound;
            _audioSource.loop = true;
            _rotationX = new SmoothRotation(RotationXRaw);
            _rotationY = new SmoothRotation(RotationYRaw);
            _velocityX = new SmoothVelocity();
            _velocityZ = new SmoothVelocity();
            //鎖定滑鼠
            Cursor.lockState = CursorLockMode.Locked;
            ValidateRotationRestriction();
            //顯示目前生命
            textLife.text = "剩餘生命" + "\n" + life;
        }

        private Transform AssignCharactersCamera()
        {
            var t = transform;
            arms.SetPositionAndRotation(t.position, t.rotation);
            return arms;
        }

        /// Clamps <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> to valid values and
        /// ensures that <see cref="minVerticalAngle"/> is less than <see cref="maxVerticalAngle"/>.
        private void ValidateRotationRestriction()
        {
            minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
            maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);
            if (maxVerticalAngle >= minVerticalAngle) return;
            Debug.LogWarning("maxVerticalAngle should be greater than minVerticalAngle.");
            var min = minVerticalAngle;
            minVerticalAngle = maxVerticalAngle;
            maxVerticalAngle = min;
        }

        #region 玩家扣血
        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "子彈")
            {
                playerHp -= UnityEngine.Random.Range(1, 20);
                textHP.text = "" + playerHp;
                if (playerHp <= 0)
                {
                    Dead0();
                }
            }
        }
        #endregion

        private static float ClampRotationRestriction(float rotationRestriction, float min, float max)
        {
            if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
            var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
            Debug.LogWarning(message);
            return Mathf.Clamp(rotationRestriction, min, max);
        }

        /// Checks if the character is on the ground.
        private void OnCollisionStay()
        {
            var bounds = _collider.bounds;
            var extents = bounds.extents;
            var radius = extents.x - 0.01f;
            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
            if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
            for (var i = 0; i < _groundCastResults.Length; i++)
            {
                _groundCastResults[i] = new RaycastHit();
            }

            _isGrounded = true;
        }

        /// Processes the character movement and the camera rotation every fixed framerate frame.
        private void FixedUpdate()
        {
            // FixedUpdate is used instead of Update because this code is dealing with physics and smoothing.
            //如果是自己物件執行控制玩家
            if (pv.IsMine)
            {
                RotateCameraAndCharacter();
                PlayFootstepSounds();
                MoveCharacter();
                Jump();
            }
            //否則同步座標
            else
            {
                SmoothMove();
                SmoothRotateWeapon();
            }
            _isGrounded = false;
        }
        [PunRPC]
        /// <summary>
        /// 平滑移動
        /// </summary>
        private void SmoothMove()
        {
            //其他玩家的座標=(原本座標，同步座標資訊，百分比-同步平滑速度*時間差)
            transform.position = Vector3.Lerp(transform.position, positionNext, smoothSpeed * Time.deltaTime);
        }

        private void SmoothRotateWeapon()
        {
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, RotateNext, smoothSpeed * Time.deltaTime);
        }

        /// Moves the camera to the character, processes jumping and plays sounds every frame.
        private void Update()
        {
            arms.position = transform.position + transform.TransformVector(armPosition);
            Jump();
            PlayFootstepSounds();
        }

        #region 原本視角控制跳躍
        private void RotateCameraAndCharacter()
        {
            var rotationX = _rotationX.Update(RotationXRaw, rotationSmoothness);
            var rotationY = _rotationY.Update(RotationYRaw, rotationSmoothness);
            var clampedY = RestrictVerticalRotation(rotationY);
            _rotationY.Current = clampedY;
            var worldUp = arms.InverseTransformDirection(Vector3.up);
            var rotation = arms.rotation *
                           Quaternion.AngleAxis(rotationX, worldUp) *
                           Quaternion.AngleAxis(clampedY, Vector3.left);
            transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
            arms.rotation = rotation;
        }

        /// Returns the target rotation of the camera around the y axis with no smoothing.
        private float RotationXRaw
        {
            get { return input.RotateX * mouseSensitivity; }
        }

        /// Returns the target rotation of the camera around the x axis with no smoothing.
        private float RotationYRaw
        {
            get { return input.RotateY * mouseSensitivity; }
        }

        /// Clamps the rotation of the camera around the x axis
        /// between the <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> values.
        private float RestrictVerticalRotation(float mouseY)
        {
            var currentAngle = NormalizeAngle(arms.eulerAngles.x);
            var minY = minVerticalAngle + currentAngle;
            var maxY = maxVerticalAngle + currentAngle;
            return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
        }

        /// Normalize an angle between -180 and 180 degrees.
        /// <param name="angleDegrees">angle to normalize</param>
        /// <returns>normalized angle</returns>
        private static float NormalizeAngle(float angleDegrees)
        {
            while (angleDegrees > 180f)
            {
                angleDegrees -= 360f;
            }

            while (angleDegrees <= -180f)
            {
                angleDegrees += 360f;
            }

            return angleDegrees;
        }

        private void MoveCharacter()
        {
            var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
            var worldDirection = transform.TransformDirection(direction);
            var velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed);
            //Checks for collisions so that the character does not stuck when jumping against walls.
            var intersectsWall = CheckCollisionsWithWalls(velocity);
            if (intersectsWall)
            {
                _velocityX.Current = _velocityZ.Current = 0f;
                return;
            }

            var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
            var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
            var rigidbodyVelocity = _rigidbody.velocity;
            var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        private bool CheckCollisionsWithWalls(Vector3 velocity)
        {
            if (_isGrounded) return false;
            var bounds = _collider.bounds;
            var radius = _collider.radius;
            var halfHeight = _collider.height * 0.5f - radius * 1.0f;
            var point1 = bounds.center;
            point1.y += halfHeight;
            var point2 = bounds.center;
            point2.y -= halfHeight;
            Physics.CapsuleCastNonAlloc(point1, point2, radius, velocity.normalized, _wallCastResults,
                radius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
            var collides = _wallCastResults.Any(hit => hit.collider != null && hit.collider != _collider);
            if (!collides) return false;
            for (var i = 0; i < _wallCastResults.Length; i++)
            {
                _wallCastResults[i] = new RaycastHit();
            }

            return true;
        }

        private void Jump()
        {
            if (!_isGrounded || !input.Jump) return;
            _isGrounded = false;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void PlayFootstepSounds()
        {
            if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f)
            {
                _audioSource.clip = input.Run ? runningSound : walkingSound;
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Pause();
                }
            }
        }
        #endregion

        #region 同步內容
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //如果 正在寫入資料
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);//傳遞資料座標
                stream.SendNext(transform.rotation.eulerAngles);//傳遞旋轉資料

            }
            //如果正在讀取資料
            else if (stream.IsReading)
            {
                positionNext = (Vector3)stream.ReceiveNext();//同步座標資訊
                RotateNext = (Vector3)stream.ReceiveNext();//同步旋轉資訊
            }
        }
        #endregion

        /// <summary>
        /// 死亡方法
        /// </summary>
        private void Dead()
        {
            if (playerHp <= 0)
            {
                life -= 1.0f;
                textLife.text = "剩餘生命" + "\n" + life;
                playerHp += 100;
            }
        }
        private void Dead0()
        {
            PlayerDeadUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

        }

        /// A helper for assistance with smoothing the camera rotation.
        private class SmoothRotation
        {
            private float _current;
            private float _currentVelocity;

            public SmoothRotation(float startAngle)
            {
                _current = startAngle;
            }

            /// Returns the smoothed rotation.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }

        /// A helper for assistance with smoothing the movement.
        private class SmoothVelocity
        {
            private float _current;
            private float _currentVelocity;

            /// Returns the smoothed velocity.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }

        /// Input mappings
        [Serializable]
        private class FpsInput
        {
            [Tooltip("The name of the virtual axis mapped to rotate the camera around the y axis."),
             SerializeField]
            private string rotateX = "Mouse X";

            [Tooltip("The name of the virtual axis mapped to rotate the camera around the x axis."),
             SerializeField]
            private string rotateY = "Mouse Y";

            [Tooltip("The name of the virtual axis mapped to move the character back and forth."),
             SerializeField]
            private string move = "Horizontal";

            [Tooltip("The name of the virtual axis mapped to move the character left and right."),
             SerializeField]
            private string strafe = "Vertical";

            [Tooltip("The name of the virtual button mapped to run."),
             SerializeField]
            private string run = "Fire3";

            [Tooltip("The name of the virtual button mapped to jump."),
             SerializeField]
            private string jump = "Jump";

            /// Returns the value of the virtual axis mapped to rotate the camera around the y axis.
            public float RotateX
            {
                get { return Input.GetAxisRaw(rotateX); }
            }

            /// Returns the value of the virtual axis mapped to rotate the camera around the x axis.        
            public float RotateY
            {
                get { return Input.GetAxisRaw(rotateY); }
            }

            /// Returns the value of the virtual axis mapped to move the character back and forth.        
            public float Move
            {
                get { return Input.GetAxisRaw(move); }
            }

            /// Returns the value of the virtual axis mapped to move the character left and right.         
            public float Strafe
            {
                get { return Input.GetAxisRaw(strafe); }
            }

            /// Returns true while the virtual button mapped to run is held down.          
            public bool Run
            {
                get { return Input.GetButton(run); }
            }

            /// Returns true during the frame the user pressed down the virtual button mapped to jump.          
            public bool Jump
            {
                get { return Input.GetButtonDown(jump); }
            }
        }
    }
}