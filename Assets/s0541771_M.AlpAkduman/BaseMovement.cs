using UnityEngine;

namespace Assets.s0541771_M.AlpAkduman
{
    public class BaseMovement : MonoBehaviour {
        public GameObject HorzPedal;
        public GameObject VertPedal;
        public float HorzRot, VertRot;
        public float PedalFactor = -10;
        public float RotFactor = 30f;
        private GameObject _root;


        void Start () {

            //Get References to the game pedals for applying a rotation in Update()
            //initialize all values for use
            HorzPedal = GameObject.Find("HorzPedal");
            VertPedal = GameObject.Find("VertPedal");
            _root = GameObject.Find("MovablePlayfield");

        }

        // Update is called once per frame
        void Update()
        {
            //Get Input from Input.GetAxis
            var moveHorizontal = Input.GetAxis("Horizontal") * RotFactor;
            var moveVertical = Input.GetAxis("Vertical") * RotFactor;


            //Stretch it from 0-1 to 0 - maxRotFactor

            //Apply a part of the rotation to this (and children) to rotate the playfield
            //Use Quaternion.Slerp und Quaternion.Euler for doing it
            //REM: Maybe you have to invert one axis to get things right visualy
            _root.transform.rotation = Quaternion.Slerp(_root.transform.rotation, Quaternion.Euler(-moveVertical, 0, moveHorizontal), Time.deltaTime * 2.0f);


            //Apply an exaggerated ammount of rotation to the pedals to visualize the players input
            //Make the rotation look right
            VertPedal.transform.rotation = Quaternion.Slerp(VertPedal.transform.rotation, Quaternion.Euler(-moveVertical, 0, 90), Time.deltaTime * 2.0f);
            HorzPedal.transform.rotation = Quaternion.Slerp(HorzPedal.transform.rotation, Quaternion.Euler(-moveHorizontal, 90, 90), Time.deltaTime * 2.0f);


        }
    }
}
