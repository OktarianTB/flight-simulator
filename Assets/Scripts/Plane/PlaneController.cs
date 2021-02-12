using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlaneController : MonoBehaviour
{

    public float speed = 15f;
    float vertical, horizontal;

    [SerializeField]
    TextMeshProUGUI displayText = null;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
        float xMovement = - transform.rotation.z * Time.deltaTime * speed * 3f;
        transform.position += new Vector3(xMovement, 0, 0);

        speed -= transform.forward.y * Time.deltaTime * 13.0f;
        
        vertical = Mathf.Lerp(vertical, Input.GetAxis("Vertical"), Time.deltaTime * 2f);
        horizontal = Mathf.Lerp(horizontal, Input.GetAxis("Horizontal"), Time.deltaTime * 2f);

        transform.Rotate(vertical, horizontal, -horizontal);

        displayText.text = "Altitude: " + (int)transform.position.y;
    }

}
