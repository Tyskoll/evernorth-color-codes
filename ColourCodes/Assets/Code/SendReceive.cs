using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace Evernorth.ColourCodes
{
    public class SendReceive
    {
        public Image colorImage;
        public Light spotLight;
        public Color32 charColour;
        public Color32 noColour = new Color32(255, 255, 255, 255);

        public Color32 colorOut;

        public Vector3Int[] dataStream;
        public int dataPos;
        public bool hasData = false;
        public bool endOfStream = false;
        public bool bCasting = false;

        public float ticker = 0.25f;

        public SendReceive(Image colourImage, Light spotLight)
        {
            this.colorImage = colourImage;
            this.spotLight=spotLight;
        }

        public void ReceiveData(Vector3Int[] dStream)
        {
            Debug.Log("ReceiveData->");
            dataStream = new Vector3Int[dStream.Length];
            System.Array.Copy(dStream, dataStream, dStream.Length);
            hasData = true;
        }

        public Color32 NextColor()
        {
            Color32 nCol = Color.white;

            if (dataPos < dataStream.Length)
            {
                nCol = new Color32((byte)dataStream[dataPos].x, (byte)dataStream[dataPos].y, (byte)dataStream[dataPos].z, 255);
                dataPos++;
                return nCol;
            }
            else if(dataPos >= dataStream.Length)
            {
                Debug.Log("-- End of Stream --");
                endOfStream = true;
                bCasting = false;
                return nCol;
            }
            else
            {
                
                hasData = false;
                endOfStream = true;
                return nCol;
            }
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void Update()
        {
            if (hasData && !endOfStream)
            {
                bCasting = true;

                if (ticker > 0f)
                {
                    ticker -= Time.deltaTime;
                }
                else if (ticker <= 0f)
                {
                    ticker = 0.25f;
                }

                if (ticker > 0.25f)
                {
                    colorImage.color = noColour;
                    spotLight.color = noColour;
                }
                else if (ticker <= 0.25f)
                {
                    Color32 newColor = NextColor();
                    colorImage.color = newColor;
                    colorOut = newColor;
                    //Debug.Log($"Current color: {colorOut}");
                    spotLight.color = newColor;
                }
            }
        }
    }
}

