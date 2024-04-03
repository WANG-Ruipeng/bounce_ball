using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class SpinnerController : MonoBehaviour
    {
        [SerializeField] private string spinnerID = "Spinner_0";

        public string SpinnerID => spinnerID;
    }
}