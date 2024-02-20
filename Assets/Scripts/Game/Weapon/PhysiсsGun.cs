using FishNet.Broadcast;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Weapon
{
    public class PhysicsGun : BaseGun
    {
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private float rotationSpeed = 5f; 

        private Rigidbody _trackedObject;
        private float _distance = 10;

        private PlayerController _playerController;
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }
        
        private void FixedUpdate()
        {
            _playerController.canModedCamera = !Input.GetKey(KeyCode.E);
           
            if (_trackedObject != null)
            {
                if (Input.GetKey(KeyCode.E))
                {
                
                    float rotateInputX = Input.GetAxis("Mouse X") * rotationSpeed;
                    float rotateInputY = Input.GetAxis("Mouse Y") * rotationSpeed;

                    // Вращение по осям X и Y
                    _trackedObject.transform.Rotate(Vector3.up, -rotateInputX, Space.World);
                    _trackedObject.transform.Rotate(Vector3.right, rotateInputY, Space.World);
                    // _trackedObject.transform.Rotate(rotateInputY,-rotateInputX,  0);
                
                }
                else
                {
                    float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                    _distance = Mathf.Clamp(_distance + scrollInput * 10f, 2f, maxDistance);
                
                    Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                    Vector3 targetPosition = ray.GetPoint(_distance);

                    _trackedObject.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(_trackedObject.position, targetPosition, Time.fixedDeltaTime * 6f));
                }
               
            }
        }
        
        protected override void Fire()
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                GameObject hoveredObject = hit.collider.gameObject;
                Debug.Log("Hovered object: " + hoveredObject.name);

                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    _distance = hit.distance;
                    _trackedObject = rb;
                    _trackedObject.isKinematic = true;
                    _trackedObject.useGravity = false;
                }
            }
            else
            {
                print("none");
            }
        }

        protected override void Fire2()
        {
            if (_trackedObject != null)
            {
                _trackedObject = null;
            }
        }

        protected override void StopFire()
        {
            if (_trackedObject != null)
            {
                _trackedObject.isKinematic = false;
                _trackedObject.useGravity = true;

                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                Vector3 targetPosition = ray.GetPoint(_distance);

                if(targetPosition != _trackedObject.position)
                    _trackedObject.AddForceAtPosition(_trackedObject.position- targetPosition, targetPosition);


                _trackedObject = null;
            }
        }
    }
    
}
