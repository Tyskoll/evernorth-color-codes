using System;
using System.Collections;
using System.Collections.Generic;
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

        public string decompString;

        public int dataLengthTotal;

        Vector3Int colV = new Vector3Int(0, 0, 0);
        char c = '\u0000';

        public int dataPos1 = 0;
        public int dataPos2 = 0;
        public bool isDecrypting;
        public bool isDecrypted;
        public bool isString;

        public string outString;

        public Vector3Int[] cArray;

        public Decrypt(IOServer ioServer, Dictionary<Vector3Int, char> colorToCharKey, string charactersS)//Dictionary<char, Vector3Int> charToColorKey)
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
            if(isString)
            {
                dataLengthTotal = eStringData.Length + (eStringData.Length / 3);

                decompString = DecodeFromChar(eStringData);

                sColArray = StringToVector(decompString);

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

            for (int i = 0; i < newCArray.Length; i++)
            {
                char c = CharLookup(newCArray[i]);
                s += c;
                dataPos2 += 1;

                if (dataPos2 >= newCArray.Length)
                {
                    isDecrypting = false;
                    isDecrypted = true;
                }
            }

            outputText = s;
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

        public string DecodeFromChar(string s)
        {
            string tempString = "";

            for(int i = 0; i < s.Length;i++)
            {
                int i1 = charactersS.IndexOf(s[i]);

                tempString = tempString + $"{i1.ToString("000")}";
            }

            return tempString;
        }

        public Vector3Int[] StringToVector(string s)
        {
            Vector3Int[] tempV3 = new Vector3Int[s.Length / 9];

            int vPos = 0;

            for (int i = 0; i < s.Length; i += 9)
            {
                string c1 = s[i].ToString();
                string c2 = s[i+1].ToString();
                string c3 = s[i+2].ToString();
                string c4 = s[i+3].ToString();
                string c5 = s[i+4].ToString();
                string c6 = s[i+5].ToString();
                string c7 = s[i+6].ToString();
                string c8 = s[i+7].ToString();
                string c9 = s[i+8].ToString();

                string iS1 = $"{c1}{c2}{c3}";
                string iS2 = $"{c4}{c5}{c6}";
                string iS3 = $"{c7}{c8}{c9}";

                int i1 = int.Parse(iS1.ToString());
                int i2 = int.Parse(iS2.ToString());
                int i3 = int.Parse(iS3.ToString());

                tempV3[vPos] = new Vector3Int(i1,i2,i3);

                vPos++;
            }

            string tmp = "";

            foreach(Vector3Int v3 in tempV3)
            {
                tmp = tmp + $"{v3.x}{v3.y}{v3.z}";
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
