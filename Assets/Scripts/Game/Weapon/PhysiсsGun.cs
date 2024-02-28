using FishNet.Broadcast;
using FishNet.Component.Transforming;
using FishNet.Connection;
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
        private NetworkConnection _conn;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            if (IsOwner)
            {
                Debug.LogError(OwnerId);
            }
            
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
                    _trackedObject.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(_trackedObject.transform.position, targetPosition, Time.fixedDeltaTime * 6f)); 
                    MoveObjectServer(_trackedObject.gameObject, targetPosition);
                }
               
            }
        }
        
        [ServerRpc(RunLocally = false)]
        private void MoveObjectServer(GameObject obj, Vector3 targetPosition, NetworkConnection conn = null)
        {
            //obj.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(obj.transform.position, targetPosition, Time.fixedDeltaTime * 6f));
            obj.transform.position = targetPosition;
            // Debug.LogError( obj.transform.position);
            // MoveObject(obj, position);
            MoveObject(obj, targetPosition, conn.ClientId);
        }
        [ObserversRpc]
        private void MoveObject(GameObject obj, Vector3 targetPosition, int conn)
        {
           
            if (conn == Owner.ClientId)
            {
                if (IsOwner)
                {
                    Debug.LogError($"Its me {conn} {OwnerId}");
                    return;
                }
                
            }
            obj.transform.position = targetPosition;
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
                    FriseObject(_trackedObject.gameObject, true);
                    
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
                // _trackedObject.isKinematic = false;
                // _trackedObject.useGravity = true;
                FriseObject(_trackedObject.gameObject, false);
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                Vector3 targetPosition = ray.GetPoint(_distance);

                if (targetPosition != _trackedObject.position)
                {
                    _trackedObject.AddForceAtPosition(_trackedObject.position- targetPosition, targetPosition);
                    Impuls(_trackedObject.gameObject, targetPosition);
                }
                  


                _trackedObject = null;
                // _trackedObject.GetComponent<NetworkTransform>().SetSynchronizePosition(true);
                // _trackedObject.GetComponent<NetworkTransform>().SetSynchronizeRotation(true);
            }
        }
        
        [ServerRpc(RunLocally = false)]
        private void Impuls(GameObject obj,  Vector3 targetPosition)
        {
            obj.GetComponent<Rigidbody>().AddForceAtPosition(obj.transform.position- targetPosition, targetPosition);
            ImpulsR(obj, targetPosition);
        }
        
        [ObserversRpc]
        private void ImpulsR(GameObject obj,  Vector3 targetPosition)
        {
            obj.GetComponent<Rigidbody>().AddForceAtPosition(obj.transform.position- targetPosition, targetPosition);
        }
        
        [ServerRpc(RunLocally = true)]
        private void FriseObject(GameObject obj,  bool frees)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = frees;
            rb.useGravity = !frees;
            FriseObjectA(obj, frees);
            // obj.GetComponent<NetworkTransform>().SetSynchronizePosition(!frees);
        }
        
        [ObserversRpc]
        private void FriseObjectA(GameObject obj,  bool frees)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = frees;
            rb.useGravity = !frees;
            // obj.GetComponent<NetworkTransform>().SetSynchronizePosition(!frees);
        }
    }
    
}
