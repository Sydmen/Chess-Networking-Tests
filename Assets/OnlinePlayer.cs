using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OnlinePlayer : NetworkBehaviour
{
    void Update()
    {
        TestMethod();
    }

    void TestMethod(){
        if(isLocalPlayer){
            Vector2 movementTestVector = InputManager.Player.Move.ReadValue<Vector2>();
            transform.position = transform.position + (Vector3)movementTestVector * Time.deltaTime;
        }
    }
}
