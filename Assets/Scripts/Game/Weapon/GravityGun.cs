using System;
using UnityEngine;

namespace Game.Weapon
{
    public class GravityGun : BaseGun
    {
        [SerializeField] private float pushForce = 10f;
        [SerializeField] private float maxDistance = 10f;

  

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
                    Vector3 forceDirection = hit.point;
                    rb.AddForceAtPosition(ray.direction * pushForce, hit.point, ForceMode.Impulse);            }
            }
            else
            {
                print("none");
            }
        }
    }
}
