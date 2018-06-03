using System.Collections;
using System.Collections.Generic;
using Finch;
using UnityEngine;

public class test3 : MonoBehaviour {
LineRenderer nowline;
public GameObject linerwt;
public List<GameObject> alline;
public bool drawnow;
public int flag = 0;
public FinchChirality Chirality;
// Use this for initialization
void Start () {

}
// Update is called once per frame
void Update () {
FinchController controller = FinchVR.GetFinchController (Chirality);
if (flag == 0){
setdrawnow (1);
}
flag++;
if(flag == 65536) {flag = 1;}
if (Input.GetKeyDown (KeyCode.B) || controller.GetPress (FinchControllerElement.ButtonThumb)) {
setdrawnow (2);
}
if (drawnow) {
setline ();
}
}
public void setdrawnow (int id) {
if (id == 1) {
okstartdraw ();
}
if (id == 2) {
enddraw ();
}
}
public void okstartdraw () {
GameObject g = (GameObject) Instantiate (linerwt);
nowline = g.GetComponent<LineRenderer> ();
nowline.SetPosition (0, transform.position);
drawnow = true;
alline.Add (g);
//bitoumr.material.color = Color.green;
}
public void enddraw () {
//bitoumr.material.color = Color.white;
drawnow = false;
}
public void setline () {
if (!nowline) { return; }
int inum = nowline.positionCount;
if (nowline.GetPosition (inum - 1) != transform.position) {
inum++;
nowline.positionCount = inum;
nowline.SetPosition (inum - 1, transform.position);
}
//setfxt += Time.deltaTime;
//if (setfxt >= 0.1f)
//{
//    setfxnow();
//    setfxt = 0;
//}
}
}

