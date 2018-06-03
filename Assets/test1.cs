using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test1 : MonoBehaviour
{

float speed = 0.5f;
RaycastHit hit = new RaycastHit();
Vector3 velocity;
Vector3 startPos;
bool canMove = false;
public Camera cam;
// Use this for initialization
void Start()
{

}

// Update is called once per frame
void Update()
{

if (true)
{
Ray ray = Camera.main.ScreenPointToRay(cam.transform.position);

if (Physics.Raycast(ray, out hit, 100))
{
if (null != hit.transform)
{
//print("can move");
canMove = true;
startPos = transform.position;
velocity = hit.point - startPos;
}

}

}
if (canMove && null != hit.transform)
{
//print("ready for move");
print(transform.position.ToString() + hit.point.ToString());
if ((transform.position.x - hit.point.x > 0.01 || (transform.position.x - hit.point.x < -0.01)) ||
(transform.position.y - hit.point.y > 0.01 || (transform.position.y - hit.point.y < -0.01)))
{
//print("moving");
transform.Translate(velocity / velocity.magnitude * speed * Time.deltaTime);

}
else
{
//print("can not move");
canMove = false;
}

}
}
}
