using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Evernorth.ColourCodes
{
    public class Encrypt
    {
        public char[,,] CharArray3D;
        public string characters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$£%^&*()-_=+[{]}|\\;:'\",./<>?`~ \n\r\t";
        public string charactersE = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890™!%^&*<>?¤†×÷‡±¶§«»©®æÆÇ;()ÊËÉÈìÍÌÎÏ.œŒôöòõøÓÔÕØÒšŠûÙÚÛŸýÝžŽªÞþƒßµðÐ¬¿¡¥£€¢¹²³½¼¾¦üéâäàåçêëèïîùÿÖÜáíóúñÑАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяᚠᚢᚦᚨᚱᚲᚷᚹᚺᚾᛁᛃᛇᛈᛉᛊᛏᛒᛖᛗᛚᛜᛞᛟ";
        public string charactersS;

        public int[] iSeed;
        public string iSeedTxt;
        public int uniqueValueCount;

        public string stringData;
        public string vString;
        public Vector3Int[] eV3Data;
        public string eStringData;

        public Dictionary<char, Vector3Int> CharToColorKey;
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public Dictionary<int, char> IntToCharKey;

        public List<char> UsedChars;

        public int dataLengthTotal;
        public int dataPos1 = 0;
        public int dataPos2 = 0;
        public bool isEncrypted;
        public bool isEncrypting;
        public bool isString;

        public void UniqueStringCheck()
        {
            //For testing
            CheckUnique(charactersE);
        }

        public void CheckUnique(string str)
        {
            //For testing
            string repeated = "";
            string one = "";
            string two = "";
            for (int i = 0; i < str.Length; i++)
            {
                one = str.Substring(i, 1);
                for (int j = 0; j < str.Length; j++)
                {
                    two = str.Substring(j, 1);
                    if ((one == two) && (i != j))
                    {
                        repeated = repeated + $"{one}\n";
                    } 
                }
            }
        }

        public Encrypt()
        {
            //For testing
            //UniqueStringCheck();

            iSeed = Seed();

            CharArray3D = new char[255, 255, 255];
            CharToColorKey = new Dictionary<char, Vector3Int>();
            AssignCharColors();

            charactersS = Shuffle(charactersE);

            IntToCharKey = new Dictionary<int, char>();
        }

        public void EncryptStart()
        {
            isEncrypting = true;

            if (isString)
            {
                dataLengthTotal = stringData.Length * 2;// + (stringData.Length * 3);

                eV3Data = new Vector3Int[stringData.Length];
                eV3Data = StringToColor(stringData);
                eStringData = VectorToString(eV3Data);
            }
            else if (!isString)
            {
                dataLengthTotal = CharArray3D.Length;
                eV3Data = StringToColor(stringData);
            }

            isEncrypting = false;
        }

        // Generate random Vector3Int for each char in character string
        public void AssignCharColors()
        {
            int x;
            int y;
            int z;
            Vector3Int cPos;

            foreach (char c in characters)
            {
                x = UnityEngine.Random.Range(10, 246);
                y = UnityEngine.Random.Range(10, 246);
                z = UnityEngine.Random.Range(10, 246);

                cPos = new Vector3Int(x, y, z);

                try
                {
                    if (IndexZero(cPos))
                    {
                        CharArray3D[cPos.x, cPos.y, cPos.z] = c;
                        SetCharColorPair(cPos, c);
                    }
                    else
                    {
                        Debug.Log($"Index not empty at: {cPos.ToString()}, occupied by: {CharArray3D[cPos.x, cPos.y, cPos.z].ToString()}");
                    }

                }
                catch (Exception e)
                {
                    Debug.Log($"Error: {e}");
                }
            }
        }

        // Cheeck if the index position is empty
        public bool IndexZero(Vector3Int cPos)
        {
            string c = CharArray3D[cPos.x, cPos.y, cPos.z].ToString();

            bool isZero = CharArray3D[cPos.x, cPos.y, cPos.z] == '\u0000';

            switch (isZero)
            {
                case true: return true;
                case false: return false;
            }
        }

        // Add a KeyValuePair to the dictionary
        public void SetCharColorPair(Vector3Int col, char c)
        {
            CharToColorKey.Add(c, col);
        }

        // Encode our provided string as Vector3Int using ColorLookup()
        public Vector3Int[] StringToColor(string s)
        {
            Vector3Int[] cArray = new Vector3Int[s.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                Vector3Int colV = ColorLookup(s[i]);
                cArray[i] = colV;
                dataPos1++;
            }

            Shift(cArray);

            if (!isString)
                isEncrypted = true;

            return cArray;
        }

        public string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            System.Random rng = new System.Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

        // Look up a character and get a Vector3Int from the dictionary
        Vector3Int ColorLookup(char c)
        {
            Vector3Int colV;
            CharToColorKey.TryGetValue(c, out colV);

            return colV;
        }

        char CharLookup(Vector3Int ev3)
        {
            char c;
            ColorToCharKey.TryGetValue(ev3, out c);

            return c;
        }

        public int[] Seed()
        {
            int[] seedValue = new int[256];

            for(int i = 0; i < 255; i++)
            {
                int d = UnityEngine.Random.Range(0, 10);
                seedValue[i] = d;
            }

            //iSeed printing
            string s = "";
            for (int i = 0; i < seedValue.Length; i++)
            {
                s += seedValue[i];
            }

            iSeed = seedValue;
            iSeedTxt = s;

            return seedValue;
        }

        
        public Vector3Int[] Shift(Vector3Int[] cArray)
        {
            int seedPos = 0;

            for(int i = 0; i < cArray.Length; i++)
            {
                if (seedPos >= 255)
                    seedPos = 0;

                cArray[i].x += iSeed[seedPos];
                seedPos++;
                cArray[i].y -= iSeed[seedPos];
                seedPos++;
                cArray[i].z += iSeed[seedPos];
                seedPos++;
                seedPos++;  //with room to include alpha value
            }

            uniqueValueCount = cArray.Distinct().Count();

            return cArray;
        }

        public string VectorToString(Vector3Int[] cArray)
        {
            Debug.Log("VectorToString");
            string tempString = "";

            for (int i = 0; i < cArray.Length; i++)
            {
                int i1 = int.Parse(cArray[i].x.ToString("000"));
                int i2 = int.Parse(cArray[i].y.ToString("000"));
                int i3 = int.Parse(cArray[i].z.ToString("000"));

                tempString = tempString + charactersS[i1] + charactersS[i2] + charactersS[i3];

                dataPos2 += 1;
            }

            vString = tempString;

            if (isString)
                isEncrypted = true;

            return vString;
        }
    }



}
