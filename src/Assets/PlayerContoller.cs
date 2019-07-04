using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    public float speed = 3.0f;
    GameObject sword;

    // Start is called before the first frame update
    void Start()
    {
        sword = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // 前後左右
        if (Input.GetKey("up"))
        {
            transform.position -= transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("down"))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("right"))
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey("left"))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        Swipe();
    }

    void Swipe()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 target_pos = ray.GetPoint(5.0f);

            sword.transform.LookAt(target_pos);
        }
    }
}
