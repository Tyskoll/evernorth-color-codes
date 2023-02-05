using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class CharLoop
    {
        public string characters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$%^&*()-_=+[{]}|\\;:'\",./<>?";

        public void InsertLoop()
        {
            foreach(char c in characters)
            {
                Debug.Log(c);
            }
        }
    }

}
