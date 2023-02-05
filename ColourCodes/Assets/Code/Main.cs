using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evernorth.ColourCodes;
using System;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;

namespace Evernorth.ColourCodes
{
    public class Main : MonoBehaviour
    {
        public GameObject colourCube;


        //Start Arrays & Characters
        CharArrays charArray;


        KeyCode kCode;
        int xPos;
        int yPos;
        int zPos;

        string cTest;
        //End Arrays & Characters

        //Start Image & Colour
        public Image colourImage;
        public Color32 charColour;
        public Color32 noColour = new Color32(0, 0, 0, 0);

        public Color32 colorOut;
        public Light spotLight;
        //End Image & Colour

        public float ticker = 1.0f;

        //Start Colour Reading & Text
        public TMP_Text textInput;
        public TMP_Text textOutput;
        //End Colour Reading & Text

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Instantiating new 3D array...");

            charArray = new CharArrays();


            kCode = KeyCode.A;
            xPos = 125;
            yPos = 150;
            zPos = 15;

            Debug.Log($"Inserting character: {kCode}");

            SetTest(kCode, xPos, yPos, zPos);

            cTest = GetTest(xPos, yPos, zPos);

            Debug.Log(
                $"\nCharacter retrieved: {cTest}" +
                $"\nArray position: [{xPos},{yPos},{zPos}]");

            InsertLoop();
        }
        void SetTest(KeyCode k, int x, int y, int z)
        {
            Vector3Int cPos = new Vector3Int(x, y, z);

            charArray.SetChar((char)k, cPos);
            SetCharColour(cPos);
        }
        string GetTest(int x, int y, int z)
        {
            Vector3Int cPos = new Vector3Int(x, y, z);

            string c = charArray.GetChar(cPos).ToString();
            return c;
        }

        void SetCharColour(Vector3Int cArray)
        {
            charColour = new Color32((byte)cArray.x, (byte)cArray.y, (byte)cArray.z, 255);
        }
        Color32 GetCharColour()
        {
            Color32 newColour = charColour;
            return newColour;
        }

        public void InstCube(Vector3Int cubePos)
        {
            Instantiate(colourCube, cubePos, colourCube.transform.rotation);
            charColour = new Color32((byte)cubePos.x, (byte)cubePos.y, (byte)cubePos.z, 255);
            colourCube.GetComponent<Renderer>().sharedMaterial.color = charColour;
            
        }

        public void InsertLoop()
        {
            int x;
            int y;
            int z;
            Vector3Int cPos;

            foreach (char c in charArray.characters)
            {
                x = UnityEngine.Random.Range(0, 255);
                y = UnityEngine.Random.Range(0, 255);
                z = UnityEngine.Random.Range(0, 255);

                cPos = new Vector3Int(x, y, z);

                try
                {
                    if (charArray.IndexZero(cPos))
                    {
                        charArray.CharArray3D[cPos.x, cPos.y, cPos.z] = c;
                        charArray.SetCharColorPair(cPos, c);

                        //InstCube(cPos);
                        Debug.Log($"Inserted {c} at {cPos.ToString()}");
                    }
                    else
                    {
                        Debug.Log($"Index not empty at: {cPos.ToString()}, occupied by: {charArray.CharArray3D[cPos.x, cPos.y, cPos.z].ToString()}");
                    }

                }
                catch (Exception e)
                {
                    Debug.Log($"Error: {e}");
                }

            }

            Debug.Log(
                $"-- Dictionary Key -- \n" +
                $"Length: {charArray.charToColorTable.Count}");
        }

        char Decrypt(Color32 col)
        {
            char c;
            Vector3Int colV = new Vector3Int(col.r, col.g, col.b);
            charArray.charToColorTable.TryGetValue(colV, out c);

            return c;
        }

        bool hasPrinted;

        // Update is called once per frame
        void Update()
        {
            
            if (ticker > 0f)
            {
                ticker -= Time.deltaTime;
            }
            else if (ticker <= 0f)
            {
                ticker = 1.0f;
                hasPrinted = false;
            }

            if (ticker > 0.5f)
            {
                colourImage.color = noColour;
                spotLight.color = noColour;
            }
            else if (ticker <= 0.5f && !hasPrinted)
            {
                Color32 newColor = GetCharColour();
                colourImage.color = newColor;
                colorOut = newColor;
                spotLight.color = newColor;









                string newText = textOutput.text + $" " + GetTest((int)colorOut.r, (int)colorOut.g, (int)colorOut.b);
                textOutput.text = newText;

                hasPrinted = true;
            }
            

        }
    }

}
