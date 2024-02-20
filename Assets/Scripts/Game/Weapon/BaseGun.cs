using System;
using FishNet.Object;
using UnityEngine;

namespace Game.Weapon
{
    public class BaseGun : NetworkBehaviour
    {
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

        protected virtual void Fire()
        {
            print("pew");
        }
        protected virtual void Fire2()
        {
            print("pew2");
        }
        
        protected virtual void StopFire()
        {
            print("stop pew");
        }
    }
}
