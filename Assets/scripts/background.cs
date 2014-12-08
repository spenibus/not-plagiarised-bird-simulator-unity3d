using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {

   void Update() {

      if(gameLogic.gameState == 1) {
         transform.Translate(-gameLogic.speedBackground * Time.deltaTime, 0, 0);
      }
   } // end Update()

}