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
    
    public Brush brush;

    [SerializeField] CharacterFA _characterPrefab;

    Dictionary<Player, CharacterFA> _dictModels = new Dictionary<Player, CharacterFA>();
    Dictionary<Player,CharacterViewFA> _dictViews = new Dictionary<Player, CharacterViewFA>();

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
            Debug.Log("je");
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

       _dictModels.Add(player, newCharacter);
       _dictViews.Add(player, newCharacter.GetComponent<CharacterViewFA>());
    }


    #region REQUESTES QUE RECIBEN LOS SERVIDORES AVATARES

    //Esto lo recibe del Controller y va a llamar por RPC a la funcion Move del host real
    
    public void RequestCreateBrush(Player player,Vector2 startPos ,Vector2 endPos)
    {
        photonView.RPC("RPC_CreateBrush", _server, player,startPos ,endPos);
        
    }
    public void RequestDrawAction(Player player,Vector2 actualPos)
    {
        photonView.RPC("RPC_DrawAction", _server, player,actualPos);
        //photonView.RPC("RPC_DrawAction", _server, player,actualPos);
        
    }
    public void RequestEndDrawAction(Player player)
    {
        photonView.RPC("RPC_EndDrawAction", _server, player);
        
    }    
    public void RequestClearDraw(Player player)
    {
        photonView.RPC("RPC_ClearDraw", _server, player);
        
    }

    public void RequestDisconnection(Player player)
    {
        Debug.LogWarning("ENVIO RPC");

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
            _dictModels[playerRequested].CreateBrush( startPos, endPos);
        }
    } 
    [PunRPC]
    void RPC_DrawAction(Player playerRequested,Vector2 actualPos)
    {
        if (_dictModels.ContainsKey(playerRequested))
        { 
            Debug.Log("buenas que tal");
            _dictModels[playerRequested].DrawAction(actualPos);
            _dictModels[playerRequested].Move(actualPos);
        }
    }
    
    [PunRPC]
    private void RPC_EndDrawAction(Player playerRequested)
    {
        if (_dictModels.ContainsKey(playerRequested))
        {
            _dictModels[playerRequested].EndDrawAction();
        }
    }

    [PunRPC]
    private void RPC_ClearDraw(Player playerRequested)
    {
        if (_dictModels.ContainsKey(playerRequested))
        {
            _dictModels[playerRequested].ClearDraw();
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
