using UnityEngine;

// The file name must be "Spinny.cs" to match this, sir.
public class Spinny : MonoBehaviour
{
    // You can set the speed right here in the Inspector, just like before.
    [Tooltip("The speed of rotation on each axis (X, Y, Z).")]
    public Vector3 spinSpeed = new Vector3(0, 50, 0);

    // This part keeps it spinnin' 'round.
    void Update()
    {
        // It'll spin real smooth, sir. I promise.
        transform.Rotate(spinSpeed * Time.deltaTime);
    }
}