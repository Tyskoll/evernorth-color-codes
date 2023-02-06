using UnityEngine;
using UnityEngine.UI;

namespace Evernorth.ColourCodes
{
    public class SendReceive
    {
        public Vector3Int[] dataStream;

        // Rendering
        public Image colorImage;
        public Light spotLight;
        public Color32 charColour;
        public Color32 noColour = new Color32(255, 255, 255, 255);
        public Color32 colorOut;
        
        // Helpers
        public int dataPos;
        public bool hasData = false;
        public bool endOfStream = false;
        public bool bCasting = false;

        public float ticker = 0.0f;

        public SendReceive(Image colourImage, Light spotLight)
        {
            this.colorImage = colourImage;
            this.spotLight=spotLight;
        }

        // Receive Vector3Int array and assign values to dataStream[]
        public void ReceiveData(Vector3Int[] dStream)
        {
            Debug.Log("ReceiveData->");
            dataStream = dStream;
            //dataStream = new Vector3Int[dStream.Length];
            //System.Array.Copy(dStream, dataStream, dStream.Length);
            
            hasData = true;
        }

        // Retrieve the Vector3Int value cast as bytes to encode Color32
        public Color32 NextColor()
        {
            Color32 nCol = Color.white;

            nCol = new Color32((byte)dataStream[dataPos].x, (byte)dataStream[dataPos].y, (byte)dataStream[dataPos].z, 255);
            dataPos++;
            return nCol;

        }

        public void RenderColor()
        {
            // Check if data has arrived and we're not at end of the data
            if (hasData && !endOfStream)
            {
                bCasting = true;

                for (int i = 0; i < dataStream.Length; i++)
                {
                    // Grab new colour and assign it to our renderered objects
                    Color32 newColor = NextColor();
                    colorImage.color = newColor;
                    colorOut = newColor;
                }

                endOfStream = true;
                bCasting = false;
                Debug.Log("-- End of Stream --");
            }
        }
            // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void Update()
        {
            // Check if data has arrived and we're not at end of the data
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
                    //spotLight.color = noColour;
                }
                else if (ticker <= 0.25f)
                {
                    // Grab new colour and assign it to our renderered objects
                    Color32 newColor = NextColor();
                    colorImage.color = newColor;
                    colorOut = newColor;
                    //spotLight.color = newColor;
                    // Debug.Log($"Current color: {colorOut}");
                }
            }
            else if(endOfStream)
            {
                //some stop timer thing
            }
        }
    }
}

