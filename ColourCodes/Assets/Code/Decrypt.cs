using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Evernorth.ColourCodes
{
    public class Decrypt
    {
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public Vector3Int[] colArray;
        public string outputText;

        public Decrypt(Dictionary<Vector3Int, char> colorToCharKey)
        {
            this.ColorToCharKey = colorToCharKey;
            
        }

        char CharLookup(Vector3Int col)
        {
            char c;
            Vector3Int colV = new Vector3Int(col.x, col.y, col.z);
            ColorToCharKey.TryGetValue(colV, out c);

            return c;
        }

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

        public string ReceiveData(Vector3Int[] dStream)
        {
            Debug.Log("Decrypt.ReceiveData->");
            colArray = new Vector3Int[dStream.Length];
            System.Array.Copy(dStream, colArray, dStream.Length);

            return ColorToString(colArray);
        }
    }

}
