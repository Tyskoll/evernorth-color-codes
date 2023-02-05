using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evernorth.ColourCodes
{
    public class Controller : MonoBehaviour
    {
        public Encrypt encrypt;
        public Decrypt decrypt;
        public SendReceive sendReceive;



        public TMP_Text textInput;
        public TMP_Text textOutput;

        public Button btn_encrypt;
        public Button btn_send;
        public Button btn_decrypt;


        Vector3Int[] eV3Data;


        public Image colorImage;
        public Light spotLight;
        public Color32 currentColor;


        public bool hasData;
        public bool endOfStream;
        public bool bCasting;

        public float ticker;



        // Start is called before the first frame update
        void Start()
        {
            // Instantiate new Encrypt and generate KeyValuePairs
            encrypt = new Encrypt();

            // Swap pairs
            Dictionary<Vector3Int, char> colorToCharKey = SwapPairs();
            //Instantiate new Decrypt with swapped KeyValuePairs
            decrypt = new Decrypt(colorToCharKey);

            sendReceive = new SendReceive(colorImage, spotLight);
        }

        Dictionary<Vector3Int, char> SwapPairs()
        {
            Dictionary<Vector3Int, char> colorToCharKey = new Dictionary<Vector3Int, char>();

            foreach(KeyValuePair<char, Vector3Int> pair in encrypt.CharToColorKey)
            {
                colorToCharKey.Add(pair.Value, pair.Key);
            }

            return colorToCharKey;
        }

        public void EncryptString()
        {
            if(!hasData && !endOfStream)
            {
                string s = textInput.text;
                eV3Data = encrypt.StringToColor(s);
                Debug.Log("btn_Encrypt->");
            }
        }

        public void SendData()
        {
            if (!hasData && !endOfStream)
            {
                Debug.Log("btn_Send->");
                sendReceive.ReceiveData(eV3Data);
            }
        }

        public void DecryptData()
        {
            string s = decrypt.ReceiveData(sendReceive.dataStream);

            textOutput.text = s;
        }

        // Update is called once per frame
        void Update()
        {
            ticker = sendReceive.ticker;
            bCasting = sendReceive.bCasting;
            hasData = sendReceive.hasData;
            endOfStream = sendReceive.endOfStream;
            currentColor = sendReceive.colorOut;
            
            sendReceive.Update();

            if(bCasting)
            {
                btn_encrypt.interactable = false;
                btn_send.interactable = false;
                //btn_decrypt.interactable = false;
            }
            else if(!bCasting)
            {
                btn_encrypt.interactable = true;
                btn_send.interactable = true;
                //btn_decrypt.interactable = false;
            }
            else if(!bCasting && endOfStream)
            {
                btn_encrypt.interactable = false;
                btn_send.interactable = false;
                //btn_decrypt.interactable = true;
            }
        }
    }

}
