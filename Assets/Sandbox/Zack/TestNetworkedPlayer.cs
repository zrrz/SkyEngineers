using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BeardedManStudios.Forge.Networking.Generated;

public class TestNetworkedPlayer : PlayerBehavior {

    public float speed = 5.0f;

    private void Update()
    {
        // If we are not the owner of this network object then we should
        // move this cube to the position/rotation dictated by the owner
        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
            return;
        }

        // Let the owner move the cube around with the arrow keys
        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f) * speed * Time.deltaTime;

        // If we are the owner of the object we should send the new position
        // and rotation across the network for receivers to move to in the above code
        networkObject.position = transform.position;
        networkObject.rotation = transform.rotation;

        // Note: Forge Networking takes care of only sending the delta, so there
        // is no need for you to do that manually
    }
}
