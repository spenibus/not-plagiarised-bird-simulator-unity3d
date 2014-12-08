using UnityEngine;
using System.Collections;

public class gameLogic : MonoBehaviour {

   //Application.LoadLevel(Application.loadedLevel);


   // prefabs
   public GameObject prefabKiller;     // collider used to destroy scrolling elements
   public GameObject prefabCeiling;    // ceiling collider
   public GameObject prefabGround;     // ground element
   public GameObject prefabBackground; // background element
   public GameObject prefabPipe;       // pipe element
   public GameObject prefabBird;       // bird element

   public static Animator birdAnimator;

   // gui
   public static GUIText gui_middleText;
   public static GUIText gui_title;
   public static GUIText gui_credits;
   public static GUIText gui_score;
   public static GUIText gui_scoreValue;

   // score
   public static int score     = 0;
   public static int highScore = 0;

   /* game state
      0 - waiting to start
      1 - playing
      2 - game over
   */
   public static int gameState = 0;

   public static float speedForeground =   4f; // used for ground and pipes
   public static float speedBackground = 0.4f; // used for background

   public static float speedGravity    = 20f; // gravity acceleration per sec
   public static float speedGravityMax = 10f; // gravity speed max

   public static float speedFlapping    = 20f; // flapping acceleration per sec
   public static float speedFlappingMax = 10f; // flapping speed max

   public static float camEdgeLeft;
   public static float camEdgeRight;
   public static float cameraWidth;

   public static float elemWidth = 3; // width of elements (excluding overlapping part)
   public static int   elemCount = 0;
   public static int   elemStart = 0;
   public static int   elemEnd   = 0;

   public static float spawnRate = 1f; // seconds between pipe spawns
   public static float spawnWait = 0f; // how long until next pipe spawn

   public static float birdVerticalSpeed = 0f;

   public static bool isFlapping = false;
   public static bool bypassIntro = false;




   /***************************************************************************/
   public static void addPoint() {
      score++;
      if(score > highScore) {
         highScore = score;
      }
   }




   /***************************************************************************/
   void scoreUpdate() {
      gui_scoreValue.text = score.ToString()+" / "+highScore.ToString();
   }




   /***************************************************************************/
   void checkFlapping() {

      // flapping status
      isFlapping = Input.GetMouseButton(0) ? true : false;

      // update animator
      birdAnimator.SetBool("isFlapping", isFlapping);
   }




   /***************************************************************************/
   void birdMove() {

      // user is flapping
      if(isFlapping) {
         birdVerticalSpeed = Mathf.Clamp(
            birdVerticalSpeed + speedFlapping * Time.deltaTime,
            0,
            speedFlappingMax
         );
      }
      // apply gravity
      else {
         birdVerticalSpeed = Mathf.Clamp(
            birdVerticalSpeed - speedGravity * Time.deltaTime,
            -speedGravityMax,
            speedFlappingMax
         );
      }

      // apply vertical speed
      prefabBird.rigidbody2D.velocity = new Vector2(0, birdVerticalSpeed);
   }




   /***************************************************************************/
   public static void birdCeilingTouch() {
      birdVerticalSpeed = 0;
   }




   /************************************* bird has touched a forbidden object */
   public static void fatalHit() {

      // change game state
      gameState = 2;

      // update and show text
      gui_middleText.text = "GAME OVER\nCLICK TO RETRY";
      gui_middleText.enabled = true;

      // update animator
      birdAnimator.SetBool("isDead", true);
   }




   /***************************************************************************/
   void pipeGenerator() {

      // update spawn wait
      spawnWait -= Time.deltaTime;

      // spawn pipes
      if(spawnWait <= 0f) {

         // random passage point
         float xPos = elemEnd * elemWidth;
         float yPos = Random.Range(-2f, 3f);

         // spawn the top pipe
         Instantiate(
            prefabPipe,
            prefabPipe.transform.position + new Vector3(xPos, yPos, 0),
            Quaternion.identity
         );

         // reset spawn wait
         spawnWait = spawnRate;
      }

   }




   /*************************************** scale gui text size to resolution */
   void guiScale() {

      int fontSize;

      fontSize = Screen.height/15;
      gui_middleText.fontSize = fontSize;

      fontSize = Screen.height/31;
      gui_title.fontSize      = fontSize;
      gui_credits.fontSize    = fontSize;
      gui_score.fontSize      = fontSize;
      gui_scoreValue.fontSize = fontSize;
   }




   /************************************************************** init stuff */
   void init() {

      // get gui texts

      gui_middleText = GameObject.Find("gui_middleText").GetComponent<GUIText>();
      gui_title      = GameObject.Find("gui_title").GetComponent<GUIText>();
      gui_credits    = GameObject.Find("gui_credits").GetComponent<GUIText>();
      gui_score      = GameObject.Find("gui_score").GetComponent<GUIText>();
      gui_scoreValue = GameObject.Find("gui_scoreValue").GetComponent<GUIText>();


      // intro text
      gui_middleText.text = "CLICK TO FLIP FLOP ?";

      // get bird animator
      birdAnimator = prefabBird.GetComponent<Animator>();

      // get camera edges
      camEdgeLeft  = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -10)).x;
      camEdgeRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, -10)).x;

      // camera width
      cameraWidth = Mathf.Abs(camEdgeRight-camEdgeLeft);


      // populate fore/background
      elemCount = Mathf.CeilToInt(cameraWidth / elemWidth) + 2;
      elemStart = -elemCount / 2;
      elemEnd   = -elemStart;


      for(float i=elemStart; i<=elemEnd; i++) {

         float xPos = i * elemWidth;

         // background
         Instantiate(
            prefabBackground,
            prefabBackground.transform.position + new Vector3(xPos, 0, 0),
            Quaternion.identity
         );

         // ground
         Instantiate(
            prefabGround,
            prefabGround.transform.position + new Vector3(xPos, 0, 0),
            Quaternion.identity
         );
      }


      // set killer horizontal position
      prefabKiller.transform.position = new Vector2(
         (elemStart - 1) * elemWidth,
         prefabKiller.transform.position.y
      );
   }




   /***************************************************************************/
   void Start() {

      // init
      init();

      // scale gui
      guiScale();

   } // end Start()




   /***************************************************************************/
   void Update() {

      // waiting to start game
      if(gameState == 0) {

         // reset score
         score = 0;

         //reset spawnWait
         spawnWait = 0;

         // start the game on click or on bypass
         if(Input.GetMouseButton(0) || bypassIntro) {

            // change game state
            gameState = 1;

            // hide intro
            gui_middleText.enabled = false;
         }
      }
      // playing
      else if(gameState == 1) {

         // update score on screen
         scoreUpdate();

         // generate pipes
         pipeGenerator();

         // check if player is flapping
         checkFlapping();
      }
      // game over
      else if(gameState == 2) {

         // restart the game on click
         if(Input.GetMouseButton(0)) {
            gameState = 0;
            bypassIntro = true;
            Application.LoadLevel(Application.loadedLevel);
         }

      }

   } // end Update()




   /***************************************************************************/
   void FixedUpdate() {

      // waiting to start game
      if(gameState == 0) {

      }
      // playing
      else if(gameState == 1) {

         // move the bird
         birdMove();
      }
      // game over
      else if(gameState == 2) {

      }

   } // end FixedUpdate()

}