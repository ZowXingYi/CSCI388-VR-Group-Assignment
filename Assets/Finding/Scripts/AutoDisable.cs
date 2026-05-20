using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    // This function will be called by the Animation Event
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
