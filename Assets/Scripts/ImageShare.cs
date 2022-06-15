using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class ImageShare : MonoBehaviourPun
{

    //Creo una referencia a mi clase serializable (esta abajo de todo)
    SerializeTexture _shareableObj = new SerializeTexture();
    
    string _textImage = "";


    [SerializeField] RenderTexture _drawToShare;

    [SerializeField] RawImage _canvasImage;
    

    private Texture2D tex;

    [SerializeField] GameObject _shareCanvas;

    public void DisableShare()
    {
        _shareCanvas.SetActive(false);
        _canvasImage.gameObject.SetActive(true);
    }
    
    public void UnableShare()
    {
        _shareCanvas.SetActive(true);
        _canvasImage.gameObject.SetActive(false);
    }


    public void BTN_Share()
    {
        Debug.Log("Share btn");
        
        tex = toTexture2D(_drawToShare);
        
        UploadFromProject();
        
        DisableShare();
    }
    
    void UploadFromProject()
    {

        //Guardo en mi clase serializable las caracteristicas de la textura (una textura es basicamente una matriz)
        _shareableObj.x = tex.width; //Ancho de la matriz
        _shareableObj.y = tex.height; //Alto de la matriz
        _shareableObj.bytes = ImageConversion.EncodeToPNG(tex); //Los bytes de la imagen guardados en un array de bytes

        //Serializo todo en Json, pasando todo lo que tengo guardado en esa clase a un string gigante
        _textImage = JsonUtility.ToJson(_shareableObj);

        while (_textImage.Length > 0)
        {
            int indexesToRemove = 0;

         
            if (_textImage.Length > 32000)
            {
                indexesToRemove = 32000;
            }
            else
            {
                indexesToRemove = _textImage.Length;
            }
            
            string stringPartToSend = _textImage.Substring(0, indexesToRemove);

            _textImage = _textImage.Remove(0, indexesToRemove);

            photonView.RPC("ShareImage", RpcTarget.Others, stringPartToSend);

        }

        photonView.RPC("ConvertTextToImage", RpcTarget.Others);

        //to also see shared image
        _canvasImage.texture = _drawToShare;
    }


    /// <summary>
    /// A medida van llegando las partes del string, lo acumulo en mi variable
    /// </summary>
    /// <param name="stringImage"></param>
    [PunRPC]
    void ShareImage(string stringImage)
    {
        _textImage += stringImage;
    }


    /// <summary>
    /// Esta funcion se ejecuta una vez se termino de mandar tooodo el string
    /// </summary>
    [PunRPC]
    void ConvertTextToImage()
    {
        //Paso el texto a los valores de las variables de la clase serializable
        _shareableObj = JsonUtility.FromJson<SerializeTexture>(_textImage);

        //Creo una textura en base al ancho y alto
        Texture2D tex = new Texture2D(_shareableObj.x, _shareableObj.y);

        //Cargo la imagen en ese "lienzo" con los bytes
        ImageConversion.LoadImage(tex, _shareableObj.bytes);

        //Creo el sprite con esa textura
        Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);

        //La muestro en mi Image canvas
        _canvasImage.texture = mySprite.texture;
        
    }
    
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

}


//Con esta etiqueta le doy la caracteristica a mi clase de que va a poder ser serializable (pasada a string o binario)
[Serializable]
public class SerializeTexture
{
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;
    [SerializeField]
    public byte[] bytes;
}