using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

namespace Evernorth.ColourCodes
{
    public class Decrypt
    {
        public IOServer ioServer;

        public Dictionary<Vector3Int, char> ColorToCharKey;
        public Dictionary<char, Vector3Int> CharToColorKey;
        public string charactersS;

        public int[] iSeed;

        public Vector3Int[] colArray;
        public string eStringData;
        public Vector3Int[] sColArray;
        public string outputText;

        Vector3Int colV = new Vector3Int(0, 0, 0);
        char c = '\u0000';

        public int dataLengthTotal;
        public int dataPos1 = 0;
        public int dataPos2 = 0;
        public bool isDecrypting;
        public bool isDecrypted;
        public bool isString;

        public string outString;

        public Vector3Int[] cArray;

        public Decrypt(IOServer ioServer, Dictionary<Vector3Int, char> colorToCharKey, string charactersS)
        {
            this.ioServer = ioServer;
            this.ColorToCharKey = colorToCharKey;
            this.charactersS = charactersS;
        }

        // Receive Vector3Int array and assign values to dataStream[]
        // This is just one way of acquiring the data and is sufficient 
        // for proof of concept.
        public void ReceiveData(Vector3Int[] dStream)
        {
            colArray = dStream;
        }

        public void ReceiveSData(string sDStream)
        {
            eStringData = sDStream;
        }

        public void DecryptStart()
        {
            if (isString)
            {
                dataLengthTotal = eStringData.Length + (eStringData.Length / 3);

                sColArray = StringToVector(eStringData);

                ColorToString(sColArray);

            }
            else if(!isString)
            {
                dataLengthTotal = colArray.Length;
                ColorToString(colArray);
            }
        }

        public Vector3Int[] StringToColor(string eSData)
        {
            int eSL = eSData.Length;

            isDecrypting = true;

            Vector3Int[] cArray = new Vector3Int[eSData.Length];

            for (int i = 0; i < eSData.Length; i++)
            {
                Vector3Int v = ColorLookup(eSData[i]);
                cArray[i] += v;
                dataPos1 += 1;
            }

            return cArray;
        }

        // Retrieve color from KeyPair dictionary based on char Key provided
        Vector3Int ColorLookup(char c)
        {
            try
            {
                CharToColorKey.TryGetValue(c, out colV);
                return colV;
            }
            catch (System.Exception e)
            {
                Debug.Log($"colour not found for {c}");
            }

            return colV;
        }


        public void ColorToString(Vector3Int[] cArray)
        {
            Vector3Int[] newCArray = Shift(cArray);

            string s = "";

            isDecrypting = true;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < newCArray.Length; i++)
            {
                char c = CharLookup(newCArray[i]);
                sb.Append(c);
                dataPos2 += 1;

                if (dataPos2 >= newCArray.Length)
                {
                    isDecrypting = false;
                    isDecrypted = true;
                }
            }

            outputText = sb.ToString();
        }

        // Retrieve char from KeyPair dictionary based on colour Key provided
        char CharLookup(Vector3Int col)
        {
            colV.x = col.x;
            colV.y = col.y;
            colV.z = col.z;

            try
            {
                ColorToCharKey.TryGetValue(colV, out c);
                return c;
            }
            catch (System.Exception e)
            {
                Debug.Log($"character not found for {colV.ToString()}");
            }

            return c;
        }

        public Vector3Int[] Shift(Vector3Int[] cArray)
        {
            //Debug.Log("Decrypt Shift started");
            int seedPos = 0;

            for (int i = 0; i < cArray.Length; i++)
            {
                if (seedPos >= 255)
                    seedPos = 0;

                if (cArray[i].x != 0)
                {
                    cArray[i].x -= iSeed[seedPos];
                    seedPos++;
                    cArray[i].y += iSeed[seedPos];
                    seedPos++;
                    cArray[i].z -= iSeed[seedPos];
                    seedPos++;
                    seedPos++;
                }
            }

            return cArray;
        }

        public Vector3Int[] StringToVector(string s)
        {
            Debug.Log("StringToVector");
            isDecrypting = true;

            Vector3Int[] tempV3 = new Vector3Int[s.Length / 3];

            Debug.Log($"s L: {s.Length}\n" +
                $"tempV3 L {tempV3.Length}");

            int vPos = 0;

            for (int i = 0; i < s.Length; i += 3)
            {
                int i1 = int.Parse(charactersS.IndexOf(s[i]).ToString("000"));
                int i2 = int.Parse(charactersS.IndexOf(s[i+1]).ToString("000"));
                int i3 = int.Parse(charactersS.IndexOf(s[i+2]).ToString("000"));

                tempV3[vPos] = new Vector3Int(i1,i2,i3);

                vPos++;
                dataPos1 += 1;
            }

            return tempV3;
        }

        // To-do:
        // Read pixel colour instead of values from array.
        public void GetPixel()
        {

        }
    }

}
