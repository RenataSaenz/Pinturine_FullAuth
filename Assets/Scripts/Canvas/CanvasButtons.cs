using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
public class CanvasButtons : MonoBehaviour
{
    //[SerializeField] private List<string> _words = new List<string>();
    //private TMP_Text _wordSpace;
    [SerializeField]private List<GameObject> _buttons= new List<GameObject>();

    
    public void BTN_SelectWord(TMP_Text word)
    {
        PlayerManager.instace.StartGame(word.text);
        foreach (var button in _buttons) {button.SetActive(false);}
    }
}
