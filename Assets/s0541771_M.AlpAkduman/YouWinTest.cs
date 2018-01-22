using UnityEngine;
using UnityEngine.UI;

namespace Assets.s0541771_M.AlpAkduman
{
    public class YouWinTest : MonoBehaviour {
        CreateLevel gameCtrl;

        // Use this for initialization
        void Start()
        {
            //Get a reference
        }

        void OnTriggerEnter(Collider other)
        {
            //call a method of gameCtrl indicating a possible win situation
                gameCtrl.WinTrigger(other.gameObject);
        }
    }
}
