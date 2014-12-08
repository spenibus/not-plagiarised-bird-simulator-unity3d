using UnityEngine;
using System.Collections;

public class ground : MonoBehaviour {

   void Update() {

      if(gameLogic.gameState == 1) {
         transform.Translate(-gameLogic.speedForeground * Time.deltaTime, 0, 0);
      }
   } // end Update()

}