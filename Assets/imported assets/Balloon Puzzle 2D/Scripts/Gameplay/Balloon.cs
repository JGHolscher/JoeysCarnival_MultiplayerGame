using UnityEngine;
namespace BalloonPuzzle2D
{
    public class Balloon : MonoBehaviour
    {
        public GameObject m_ConnectionEffectPrefab;
        private HingeJoint2D m_ActiveJoint;

        private bool m_IsDragging = false;
        private bool m_IsAttached = false;
        private Vector2 m_Offset;
        public BoxCollider2D m_ThreadCollider;
        public float m_LiftForce = 20f;
        bool m_IsDead = false;
        public GameObject m_KillEffectPrefab;
        float arc = 0;
        float Radius = 10;
        float speed = 0.2f;

        //public Vector3 initPos;
        void Start()
        {
            speed = 1;
            arc = Random.Range(0, 360);
            m_IsDead = false;
        }

        void Update()
        {
            //mobile control
            if (Application.isMobilePlatform)
            {
                HandleTouchInput();
            }
            // else
            // {
            //     HandleMouseInput();
            // }

            if (m_IsDragging)
            {
                Vector2 mousePosition = GameControl.Current.m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = mousePosition + m_Offset;

                CheckForBarCollision();
            }
            else if (m_IsAttached)
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * m_LiftForce, ForceMode2D.Force);
            }
            else
            {
                arc += speed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, Radius * Mathf.Sin(2 * arc));
            }
        }
        void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mouseWorldPos = GameControl.Current.m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (IsTouchingThis(mouseWorldPos))
                {
                    if (m_ActiveJoint != null)
                        Destroy(m_ActiveJoint);

                    m_IsAttached = false;
                    m_IsDragging = true;
                    m_Offset = (Vector2)transform.position - mouseWorldPos;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (m_IsDragging)
                {
                    Vector2 mouseWorldPos = GameControl.Current.m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                    transform.position = mouseWorldPos + m_Offset;
                    CheckForBarCollision();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_IsDragging = false;
            }
        }


        void CheckForBarCollision()
        {
            Vector2 checkPos = m_ThreadCollider.bounds.center;
            float radius = 0.5f;

            Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, radius);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Bar"))
                    {
                        AttachToBar(hit.gameObject);

                        GameObject obj = Instantiate(m_ConnectionEffectPrefab);
                        obj.transform.position = transform.position + new Vector3(0, -15, 0);
                        Destroy(obj, 1);

                        break;
                    }
                }
            }
        }

        void OnMouseDown()
        {
            if (m_ActiveJoint != null)
            {
                Destroy(m_ActiveJoint);
            }

            m_IsAttached = false;
            m_IsDragging = true;

            Vector2 mouseWorldPos = GameControl.Current.m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            m_Offset = (Vector2)transform.position - mouseWorldPos;
        }

        void OnMouseUp()
        {
            if (m_IsDragging)
            {
                m_IsDragging = false;
            }
        }

        void AttachToBar(GameObject bar)
        {
            m_IsAttached = true;

            HingeJoint2D joint = GetComponent<HingeJoint2D>();

            joint.enabled = true;
            joint.connectedBody = bar.GetComponent<Rigidbody2D>();

            Vector2 liftedPointWorld = new Vector2(m_ThreadCollider.bounds.center.x, m_ThreadCollider.bounds.min.y + 0.9f);
            Vector2 anchorLocal = transform.InverseTransformPoint(liftedPointWorld);
            joint.anchor = anchorLocal;

            Vector2 contactPoint = bar.GetComponent<Collider2D>().ClosestPoint(liftedPointWorld);
            Vector2 connectedAnchor = bar.transform.InverseTransformPoint(contactPoint);
            joint.connectedAnchor = connectedAnchor;

            transform.position += new Vector3(0f, 0.1f, 0f);

        }
        public void Kill()
        {
            if (!m_IsDead)
            {
                m_IsDead = true;
                GameObject obj = Instantiate(m_KillEffectPrefab);
                obj.transform.position = transform.position + new Vector3(0, 0, -1);
                Destroy(obj, 2);
                Destroy(gameObject);
            }
        }
        void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = GameControl.Current.m_MainCamera.ScreenToWorldPoint(touch.position);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (IsTouchingThis(touchPos))
                        {
                            if (m_ActiveJoint != null)
                                Destroy(m_ActiveJoint);

                            m_IsAttached = false;
                            m_IsDragging = true;
                            m_Offset = (Vector2)transform.position - touchPos;
                        }
                        break;

                    case TouchPhase.Moved:
                        if (m_IsDragging)
                        {
                            transform.position = touchPos + m_Offset;
                            CheckForBarCollision();
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (m_IsDragging)
                            m_IsDragging = false;
                        break;
                }
            }
        }
        bool IsTouchingThis(Vector2 touchPos)
        {
            Collider2D hit = Physics2D.OverlapPoint(touchPos);
            return hit != null && hit.gameObject == gameObject;
        }

    }
}