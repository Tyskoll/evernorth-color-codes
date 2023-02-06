using System.Collections.Generic;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class Decrypt
    {
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public Vector3Int[] colArray;
        public string outputText;

        Vector3Int colV = new Vector3Int(0, 0, 0);
        char c = '\u0000';

        public Decrypt(Dictionary<Vector3Int, char> colorToCharKey)
        {
            this.ColorToCharKey = colorToCharKey;      
        }

        // Retrieve char from KeyPair dictionary based on colour Key provided
        char CharLookup(Vector3Int col)
        {
            //Vector3Int colV = new Vector3Int(col.x, col.y, col.z);
            //colV = new Vector3Int(col.x, col.y, col.z);

            colV.x = col.x;
            colV.y = col.y;
            colV.z = col.z;

            try
            {
                ColorToCharKey.TryGetValue(colV, out c);
                return c;
            }
            catch(System.Exception e)
            {
                Debug.Log($"character not found for {colV.ToString()}");
            }

            return c;
        }

        // Build our string ready for output
        public string ColorToString(Vector3Int[] cArray)
        {
            string s = "";

            for (int i = 0; i < cArray.Length; i++)
            {
                char c = CharLookup(cArray[i]);
                s += c;
            }

            return s;
        }

        // Receive Vector3Int array and assign values to dataStream[]
        // This is just one way of acquiring the data and is sufficient 
        // for proof of concept.
        public string ReceiveData(Vector3Int[] dStream)
        {
            Debug.Log("Decrypt.ReceiveData->");
            
            colArray = dStream;

            return ColorToString(colArray);
        }

        // To-do:
        // Read pixel colour instead of values from array.
        public void GetPixel()
        {

        }
    }

}
