using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class Decrypt
    {
        public IOServer sendReceive;

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

        public Decrypt(IOServer sendReceive, Dictionary<Vector3Int, char> colorToCharKey, string charactersE)//Dictionary<char, Vector3Int> charToColorKey)
        {
            this.sendReceive = sendReceive;
            this.ColorToCharKey = colorToCharKey;
            //this.CharToColorKey = charToColorKey;
            this.charactersS = charactersE;
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
                dataLengthTotal = eStringData.Length * 2;

                decompString = Decompress(eStringData);


                //Vector3Int[] rebuiltColArray = new Vector3Int[colArray.Length];

                //sColArray = StringToColor(eStringData);    <-- old

                sColArray = Vector3Builder(decompString);

                ColorToString(sColArray);

            }
            else if(!isString)
            {
                dataLengthTotal = colArray.Length;
                ColorToString(colArray);
            }
            
            /*for(int i = 0;i < newColArray.Length; i++)
            {
                Debug.Log(
                    $"col: {newColArray[i]}");
            }*/

            
        }

        public Vector3Int[] StringToColor(string eSData)
        {
            Debug.Log("called StringToColor");
            int eSL = eSData.Length;
            Debug.Log($"eSData Length: {eSL}");
            string s = "";

            isDecrypting = true;

            Vector3Int[] cArray = new Vector3Int[eSData.Length];

            for (int i = 0; i < eSData.Length; i++)
            {
                Vector3Int v = ColorLookup(eSData[i]);
                cArray[i] += v;
                dataPos1 += 1;

                if (dataPos1 >= eSData.Length)
                {
                    Debug.Log($"End StringToColor");
                }
            }

            return cArray;
            //outputText = s;
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

            Debug.Log("called ColorToString");

            //RESULT PRINTING
            string cAString = "";

            for (int j = 0; j < cArray.Length; j++)
            {
                cAString = cAString + $"{cArray[j].x.ToString()}";
                cAString = cAString + $"{cArray[j].y.ToString()}";
                cAString = cAString + $"{cArray[j].z.ToString()}";
            }

            Debug.Log(
                $"PADDED: \n" +
                $"{cAString}");
            
            //END

            string s = "";

            isDecrypting = true;

            for (int i = 0; i < newCArray.Length; i++)
            {
                //Definitely 
                char c = CharLookup(newCArray[i]);
                s += c;
                dataPos2 += 1;

                /*
                Debug.Log(
                    $"dataPos: {dataPos2}\n" +
                    $"char:    {c}");
                */
                if (dataPos2 >= newCArray.Length)
                {
                    isDecrypting = false;
                    isDecrypted = true;

                    Debug.Log($"End ColorToString");
                }
            }

            //Debug.Log($"{s}");

            

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
            Debug.Log("Decrypt Shift started");
            int seedPos = 0;

            for (int i = 0; i < cArray.Length; i++)
            {
                /*Debug.Log(
                    $"PreShift" +
                    $"\nx: {cArray[i].x} y: {cArray[i].y} z: {cArray[i].z}");
                */

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

                /*
                Debug.Log(
                    $"PostShift" +
                    $"\nx: {cArray[i].x} y: {cArray[i].y} z: {cArray[i].z}");
                */
            }

            return cArray;
        }

        public string Decompress(string s)
        {
            string tempString = "";

            for(int i = 0; i < s.Length;i++)
            {
                int i1 = charactersS.IndexOf(s[i]);

                tempString = tempString + $"{i1.ToString("00")}";
            }

            Debug.Log(
                $"Decompressed:\n" +
                $"{tempString}");

            return tempString;
        }

        public Vector3Int[] Vector3Builder(string s)
        {
            Vector3Int[] tempV3 = new Vector3Int[s.Length / 3];

            int sPos = 0;
            int sLength = (int)Math.Ceiling(((decimal)s.Length / 3));

            Debug.Log(
                $"s.Length: {s.Length}\n" +
                $"sLength : {sLength}");

            Debug.Log($"{s}"); //still good here

            for (int i = 0; i < sLength; i++)
            {
                if(sPos < s.Length)
                {
                    string iS1;
                    string iS2;
                    string iS3;

                    string c1 = s[sPos].ToString();
                    sPos++;
                    string c2 = s[sPos].ToString();
                    sPos++;
                    string c3 = s[sPos].ToString();
                    sPos++;

                    iS1 = $"{c1}{c2}{c3}";

                    string c4 = s[sPos].ToString();
                    sPos++;
                    string c5 = s[sPos].ToString();
                    sPos++;
                    string c6 = s[sPos].ToString();
                    sPos++;

                    iS2 = $"{c4}{c5}{c6}";

                    string c7 = s[sPos].ToString();
                    sPos++;
                    string c8 = s[sPos].ToString();
                    sPos++;

                    //attempt fix - fails
                    string c9 = "";
                    if (sPos < s.Length)
                    {
                        c9 = s[sPos].ToString();
                        sPos++;
                    }
                    else
                    {
                        c9 = "0";
                    }
                    //attempt fix end

                    iS3 = $"{c7}{c8}{c9}";

                    int i1 = int.Parse(iS1.ToString());
                    int i2 = int.Parse(iS2.ToString());
                    int i3 = int.Parse(iS3.ToString());

                    try
                    {
                        if (i < tempV3.Length)
                            tempV3[i] = new Vector3Int(i1, i2, i3);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(
                            $"Outside bounds\n" +
                            $"Length: {tempV3.Length} Pos:{i}");
                    }
                }
                
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
