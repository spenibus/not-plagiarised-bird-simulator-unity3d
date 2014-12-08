using UnityEngine;
using System.Collections;

public class bird : MonoBehaviour {


   // the bird hits something
   void OnCollisionEnter2D(Collision2D co) {

      // we touch the ground or an obstacle
      if(co.collider.tag == "ground" || (co.transform.parent && co.transform.parent.tag == "pipe") ) {
         gameLogic.fatalHit();
      }
   }




   // the bird keeps hitting something
   void OnCollisionStay2D(Collision2D co) {

      // bird touches the ceiling
      if(co.collider.tag == "ceiling") {
         gameLogic.birdCeilingTouch();
      }
   }




   // the bird stops hitting something
   void OnTriggerExit2D(Collider2D co) {

      // the bird exits a score zone, score point
      if(co.tag == "scoreZone") {
         gameLogic.addPoint();
      }
   }
}