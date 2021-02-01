using Mirror;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace CubeFarm.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        public bool CanMove = true;

        [Header("Camera controls")]
        public Transform lookObject;

        public GameObject cam;

        private float xRot; // The rotation of the camera itself

        [Space]
        public Vector2 cameraSensitivity = Vector2.one;

        [Space]
        public float minView = -75f;

        public float maxView = 90f;

        [Header("Movement")]
        private Rigidbody rigidBody;

        public Vector2 moveSpeed = new Vector2(3, 5);

        public float jumpForce = 2;

        public float shellOffset = .1f;
        public float groundCheckDistance = .1f;

        internal CapsuleCollider m_collider;
        private bool isGrounded = true;

        public Vector3[] center = new Vector3[] { Vector3.one, Vector3.one };
        public Vector2[] size = new Vector2[] { Vector2.one, Vector2.one };

        private bool _jump;
        internal bool crouched;
        private Vector3 move;

        private void OnValidate()
        {
            m_collider = GetComponent<CapsuleCollider>();

            rigidBody = GetComponent<Rigidbody>();
            rigidBody.freezeRotation = true;
        }

        private void Start()
        {
            if (m_collider == null) m_collider = GetComponent<CapsuleCollider>();

            if (rigidBody == null)
            {
                rigidBody = GetComponent<Rigidbody>();
                rigidBody.freezeRotation = true;
            }

            if (!hasAuthority)
            {
                cam.gameObject.SetActive(false);

                CanMove = false;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (CanMove == false || hasAuthority == false)
                return;

            // Apply camera movement
            RotateCamera();

            if (Input.GetButton("Jump")) _jump = true;
            if (Input.GetButtonDown("Fire3"))
            {
                CmdSetCollider(1);
                crouched = true;
            }
            if (Input.GetButtonUp("Fire3"))
            {
                CmdSetCollider(0);
                crouched = false;
            }

            // Apply forward momentum
            var moveX = Input.GetAxis("Horizontal") * moveSpeed.x;
            var moveY = Input.GetAxis("Vertical") * moveSpeed.y;

            move = Vector3.right * moveX + Vector3.forward * moveY;
        }

        private void FixedUpdate()
        {
            if (CanMove == false || hasAuthority == false)
                return;

            GroundCheck();

            rigidBody.MovePosition(transform.position + transform.TransformVector(move) * Time.deltaTime);

            if (_jump)
            {
                if (isGrounded) rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _jump = false;
            }
        }

        [Command]
        public void CmdSetCollider(int value)
        {
            RpcSetCollider(value);
        }

        [ClientRpc]
        public void RpcSetCollider(int value)
        {
            m_collider.center = center[value];
            m_collider.radius = size[value].x;
            m_collider.height = size[value].y;

            lookObject.position = new Vector3(lookObject.position.x, size[value].y + 0.1f, lookObject.position.z); ;
        }

        private void RotateCamera()
        {
            var mouseX = Input.GetAxis("Mouse X") * cameraSensitivity.x;
            var mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity.y;

            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, minView, maxView);

            transform.Rotate(Vector3.up, mouseX);
            lookObject.localRotation = Quaternion.Euler(xRot, 0, 0);
        }

        private void GroundCheck()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_collider.radius * (1.0f - shellOffset),
                Vector3.down, out hitInfo, m_collider.height + groundCheckDistance,
                Physics.AllLayers, QueryTriggerInteraction.Collide))
            {
                isGrounded = (hitInfo.distance < 1);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.yellow : Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.down, 1.0f - shellOffset);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerMovement))]
    internal class PlayerMovementEditor : Editor
    {
        private int current = -1;
        private string[] options = { "Is Standing", "Is Crouching" };

        private int GetValue(PlayerMovement player)
        {
            int index = 0;
            float distance = 10;

            for (int i = 0; i < options.Length; i++)
            {
                float m_dist = Vector3.Distance(player.m_collider.center, player.center[i]) +
                (player.m_collider.radius - player.size[i].x) +
                (player.m_collider.height - player.size[i].y);

                if (m_dist <= distance)
                    index = i;
            }

            return index;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var player = (PlayerMovement)target;

            if (current >= options.Length || current < 0)
            {
                EditorUtility.SetDirty(player);
                current = GetValue(player);
            }

            int newValue = GUILayout.Toolbar(current, options);

            if (newValue != current)
            {
                current = GetValue(player);

                current = newValue;

                player.m_collider.center = player.center[current];
                player.m_collider.radius = player.size[current].x;
                player.m_collider.height = player.size[current].y;

                Debug.Log($"{player.center[current]} , {player.size[current].x }, {player.size[current].y }");
            }

            if (GUILayout.Button("Set collider value."))
            {
                player.center[current] = player.m_collider.center;
                player.size[current].x = player.m_collider.radius;
                player.size[current].y = player.m_collider.height;

                Debug.Log($"{player.center[current]} , {player.size[current].x }, {player.size[current].y }");
            }

            if (GUILayout.Button("Reset to standing."))
            {
                player.center[current] = player.center[0];
                player.size[current].x = player.size[0].x;
                player.size[current].y = player.size[0].y;

                player.m_collider.center = player.center[current];
                player.m_collider.radius = player.size[current].x;
                player.m_collider.height = player.size[current].y;

                Debug.Log($"{player.center[current]} , {player.size[current].x }, {player.size[current].y }");
            }
        }
    }

#endif
}