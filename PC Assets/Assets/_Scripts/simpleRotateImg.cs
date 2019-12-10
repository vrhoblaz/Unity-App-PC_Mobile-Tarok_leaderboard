using System.Collections;
using UnityEngine;

public class simpleRotateImg : MonoBehaviour {
    /// <summary>
    /// script on the loading image
    /// </summary>

    // start angle
    int angle = 0;

    // Use this for initialization
    void Start () {
        // start coroutione as soon, as the script loads
        StartCoroutine("RotateImage");
	}

    // rotate image coroutine
    IEnumerator RotateImage () {
        // every 0.1 second rotate image for 40 degres
        while (true)
        {
            angle += 40;
            transform.rotation = Quaternion.Euler(0, 180, angle);
            yield return new WaitForSeconds(.1f);
        }
        
	}

    // on image enable stop all coroutines just in case
    // and start one instance of rotate image coroutine
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine("RotateImage");
    }
}
