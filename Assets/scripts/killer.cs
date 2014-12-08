using UnityEngine;
using System.Collections;

public class killer : MonoBehaviour {

   void OnTriggerEnter2D(Collider2D co) {

      // loop ground and sky
      if(co.tag == "ground" || co.tag == "background") {
         co.gameObject.transform.Translate(gameLogic.elemWidth * gameLogic.elemCount, 0, 0);
      }
      // destroy pipes
      else if(co.tag == "pipe") {
         Destroy(co.gameObject);
      }

   }

}