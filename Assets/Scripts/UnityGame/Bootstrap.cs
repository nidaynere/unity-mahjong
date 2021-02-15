using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Awake ()
    {
        UserData.Load(); // Loads user data.
    }
}
