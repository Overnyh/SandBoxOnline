using System;
using FishNet.Object;
using UnityEngine;

namespace Game.Weapon
{
    public class PhysicsGunNet: NetworkBehaviour
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
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Fire();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                StopFire();
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Fire2();
            }
       
        }

        private void FixedUpdate()
        {
            _playerController.canModedCamera = !Input.GetKey(KeyCode.E);
           
            if (_trackedObject != null)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    
                }
                else
                {
                    float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                    _distance = Mathf.Clamp(_distance + scrollInput * 10f, 2f, maxDistance);
                
                    Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                    Vector3 targetPosition = ray.GetPoint(_distance);
                    // targetPosition = Vector3.Lerp(_trackedObject.position, targetPosition, Time.fixedDeltaTime * 1f);
                    MoveObjectServer(_trackedObject.gameObject, targetPosition);
                }
            }
        }

        [ServerRpc]
        private void MoveObjectServer(GameObject obj, Vector3 position)
        {
            MoveObject(obj, position);
        }
        
        [ObserversRpc]
        private void MoveObject(GameObject obj, Vector3 position)
        {
            obj.transform.transform.position = position;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Fire()
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
                    FriseObjectServer(_trackedObject.gameObject, true);
                }
            }
            else
            {
                print("none");
            }
        }
        private void Fire2()
        {
            if (_trackedObject != null)
            {
                _trackedObject = null;
            }
        }
        
        private void StopFire()
        {
            if (_trackedObject != null)
            {
                FriseObjectServer(_trackedObject.gameObject, false);

                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                Vector3 targetPosition = ray.GetPoint(_distance);

                // if(targetPosition != _trackedObject.position)
                //     _trackedObject.AddForceAtPosition(_trackedObject.position- targetPosition, targetPosition);
                
                _trackedObject = null;
            }
        }

        [ServerRpc]
        private void FriseObjectServer(GameObject obj, bool frees)
        {
            FriseObject(obj, frees);
        }
        
        [ObserversRpc]
        private void FriseObject(GameObject obj,  bool frees)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = frees;
            rb.useGravity = !frees;
        }
    }
}