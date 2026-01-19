using UnityEngine;

namespace Code.MainSystem.Rhythm.Test
{
    public class ConcertPopUpDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject concertPopUp;


        public void DisplayConcertPopUp()
        { 
            concertPopUp.SetActive(true);
        }

        public void HidePopUp()
        {
            concertPopUp.SetActive(false);
        }
    }
}