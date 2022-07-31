using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MyServer : MonoBehaviourPun
{
    public static MyServer Instance;

    Player _server;

    [SerializeField] CharacterFA _characterPrefab;
    [SerializeField] Brush _brushPrefab;
    [SerializeField] Brush[] _brushes;

    Dictionary<Player, CharacterFA> _dictModels = new Dictionary<Player, CharacterFA>();
    Dictionary<Player, List<Brush>> _dictBrushes = new Dictionary<Player, List<Brush>>();
    Dictionary<Player,CharacterViewFA> _dictViews = new Dictionary<Player, CharacterViewFA>();
    

    public int PackagePerSecond { get; private set;}
    

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            if (photonView.IsMine)
            {
                //Este RPC va en direccion a todos los Avatares que se crean
                //cada vez que un cliente nuevo entra a la sala
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);
            }
        }
    }

    [PunRPC]
    void SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _server = serverPlayer;

        PackagePerSecond = 60;

        PhotonNetwork.LoadLevel(sceneIndex);

        var playerLocal = PhotonNetwork.LocalPlayer;

        if (playerLocal != _server)
        {
            //Este RPC lo ejecuta cada servidor avatar en direccion al server original
            photonView.RPC("AddPlayer", _server, playerLocal);
           
        }

    }


    [PunRPC]
    void AddPlayer(Player player)
    {
        StartCoroutine(WaitForLevel(player));
    }

    IEnumerator WaitForLevel(Player player)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        CharacterFA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, Vector3.zero, Quaternion.identity)
            .GetComponent<CharacterFA>()
            .SetInitialParameters(player);

        _dictModels.Add(player, newCharacter);
        _dictBrushes.Add(player, new List<Brush>());
        _dictViews.Add(player, newCharacter.GetComponent<CharacterViewFA>());
        
        GameManager.Instance.Turn = _dictModels.Keys.First();
        
        GameManager.Instance.photonView.RPC("RPC_AddPlayerToScore", RpcTarget.AllBuffered, player.NickName);
    }
    


    #region REQUESTES QUE RECIBEN LOS SERVIDORES AVATARES

    //Esto lo recibe del Controller y va a llamar por RPC a la funcion Move del host real

    public void RequestCreateBrush(Player player,Vector2 startPos ,Vector2 endPos)
    {
        photonView.RPC("RPC_CreateBrush", _server, player,startPos ,endPos);
    }

    public void RequestChangeColor(Player player, string color)
    {
        photonView.RPC("RPC_ChangeBrush", _server,player, color);
    }
    public void RequestDrawAction(Player player,Vector2 actualPos)
    {
        photonView.RPC("RPC_DrawAction", _server, player,actualPos);
    }
    public void RequestClearDraw(Player player)
    {
       // photonView.RPC("RPC_ClearDraw", _server, player);
        photonView.RPC("RPC_ClearDraw", _server, player);
    }

    public void RequestDisconnection(Player player)
    {
        //PhotonNetwork.SendAllOutgoingCommands();
        photonView.RPC("RPC_PlayerDisconnect", _server, player);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public void RequestSendMessage(Player player, string nickName,string newMsg)
    {
        photonView.RPC("RPC_SendMessageToChat", _server, player,nickName, newMsg);
    }
    #endregion

    #region SERVER ORIGINAL
    
    [PunRPC] 
    void RPC_SendMessageToChat(Player playerRequested,string nickName,string newMsg)
    {
        if (_dictModels.ContainsKey(playerRequested))
        {
           // PlayerManager.instace.TryWord(nickName, newMsg);
         //   ChatSystem.Instance.SendMessageToChat(nickName,newMsg );
        }
    }

    [PunRPC]
    void RPC_ChangeBrush(Player playerRequested, string newBrush)
    {
        if (!Equals(GameManager.Instance.Turn, playerRequested)) return;

        if (_dictModels.ContainsKey(playerRequested))
        {
            Brush b = Array.Find(_brushes, brush => brush.name == newBrush);
            _brushPrefab = b;
        }
    }

    [PunRPC]
    void RPC_CreateBrush(Player playerRequested,Vector2 startPos ,Vector2 endPos)
    {
        if (!Equals(GameManager.Instance.Turn, playerRequested)) return;
        
        if (_dictModels.ContainsKey(playerRequested))
        {
            _dictModels[playerRequested].Move(startPos);
            
           Brush newBrush = PhotonNetwork.Instantiate(_brushPrefab.name,Vector3.zero, Quaternion.identity)
               .GetComponent<Brush>()
               .SetInitialParameters(playerRequested);
           
           _dictBrushes[playerRequested].Add(newBrush);
        }

        if (_dictBrushes.ContainsKey(playerRequested))
        {
            _dictBrushes[playerRequested].Last().photonView.RPC("RPC_SetStartingPosition",RpcTarget.All, startPos);
        }
    } 
    
    [PunRPC]
    void RPC_DrawAction(Player playerRequested,Vector2 actualPos)
    {
        if (!Equals(GameManager.Instance.Turn, playerRequested)) return;
        
        if (_dictModels.ContainsKey(playerRequested))
        { 
            _dictModels[playerRequested].Move(actualPos);
        }

        if (_dictBrushes.ContainsKey(playerRequested))
        {
            _dictBrushes[playerRequested].Last().photonView.RPC("RPC_DrawPoint",RpcTarget.All, actualPos);
            
        }
    }

    [PunRPC]
    private void RPC_ClearDraw(Player playerRequested)
    {
      //  if (!Equals(GameManager.Instance.Turn, playerRequested)) return;

        // for (int index = 0; index < _dictBrushes.Count; index++) 
        // {
        //     Debug.LogWarning(index);
        //     
        //     var item = GameManager.Instance.dictAreasPlayers.ElementAt(index);
        //     var itemKey = item.Key;
        //     var itemValue = item.Value;
        //     
        //     _dictBrushes[itemKey][itemValue].Clear();
        // }
        
        if (_dictBrushes.ContainsKey(playerRequested))
        {
            for (int i = 0; i < _dictBrushes[playerRequested].Count; i++)
            {
                _dictBrushes[playerRequested][i].Clear();
            }
            _dictBrushes[playerRequested].Clear();
        }
    }

    [PunRPC]
    public void RPC_PlayerDisconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictModels[player].gameObject);
        _dictModels.Remove(player);
    }
    #endregion
}
