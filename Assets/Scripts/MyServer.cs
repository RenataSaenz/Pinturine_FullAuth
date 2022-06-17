using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class MyServer : MonoBehaviourPun
{
    public static MyServer Instance;

    Player _server;

    [SerializeField] CharacterFA _characterPrefab;
    [SerializeField] Brush _brushPrefab;

    Dictionary<Player, CharacterFA> _dictModels = new Dictionary<Player, CharacterFA>();
    Dictionary<Player, List<Brush>> _dictBrushes = new Dictionary<Player, List<Brush>>();
    //Dictionary<Player, Brush> _dictBrushes = new Dictionary<Player, Brush>();
    Dictionary<Player,CharacterViewFA> _dictViews = new Dictionary<Player, CharacterViewFA>();
   // public List<Brush> list = new List<Brush>();

    public int PackagePerSecond { get; private set; }

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

    // private void Update()
    // {
    //     var brushToList = FindObjectOfType<Brush>();
    //     if (brushToList == null) return;
    //     if (!list.Contains(brushToList))list.Add(brushToList);
    // }

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
        while(PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

       CharacterFA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, Vector3.zero, Quaternion.identity)
                                               .GetComponent<CharacterFA>()
                                               .SetInitialParameters(player);
       // Brush newBrush = PhotonNetwork.Instantiate(_brushPrefab.name,Vector3.zero, Quaternion.identity)
       //     .GetComponent<Brush>()
       //     .SetInitialParameters(player, Vector3.zero);

       _dictModels.Add(player, newCharacter);
       _dictBrushes.Add(player,new List<Brush>());
       _dictViews.Add(player, newCharacter.GetComponent<CharacterViewFA>());
    }


    #region REQUESTES QUE RECIBEN LOS SERVIDORES AVATARES

    //Esto lo recibe del Controller y va a llamar por RPC a la funcion Move del host real
    
    public void RequestCreateBrush(Player player,Vector2 startPos ,Vector2 endPos)
    {
        // Brush newBrush = PhotonNetwork.Instantiate(_brushPrefab.name,Vector3.zero, Quaternion.identity)
        //     .GetComponent<Brush>()
        //     .SetInitialParameters(player, startPos);
        //
        // _dictBrushes[player] = newBrush;
        
        //     _dictBrushes[player].Add(newBrush.GetComponent<Brush>());
        // list.Add(newBrush);
        
        photonView.RPC("RPC_CreateBrush", _server, player,startPos ,endPos);
    }
    public void RequestDrawAction(Player player,Vector2 actualPos)
    {
        photonView.RPC("RPC_DrawAction", _server, player,actualPos);
    }
    public void RequestClearDraw(Player player)
    {
        photonView.RPC("RPC_ClearDraw", _server, player);
    }

    public void RequestDisconnection(Player player)
    {
        //PhotonNetwork.SendAllOutgoingCommands();
        photonView.RPC("RPC_PlayerDisconnect", _server, player);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    #endregion

    #region SERVER ORIGINAL
    
    [PunRPC]
    void RPC_CreateBrush(Player playerRequested,Vector2 startPos ,Vector2 endPos)
    {
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
