using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;

public class App : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

/// <summary>
/// 游戏初始化
/// 初始化所有client
/// </summary>
public class GameInit
{
    GameModel.Game game;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="myID"></param>
    /// <param name="totalPalyers"></param>
    public void Init(int myID, List<PlayerInfo> totalPalyers)
    {
       game = new GameModel.Game();
    }
}

namespace GameModel
{
    using FrameSyncEx;

    /// <summary>
    /// 服务器信息
    /// </summary>
    public class ServerInfo
    {
        public int roomID;
        public string serIP;
        public int serPort;
        public string wwwUrl;
    }
    /// <summary>
    /// 玩家信息
    /// </summary>
    public class PlayerInfo
    {
        public int playerID;
    }
    public class Game
    {
        ServerInfo severInfo;
        List<PlayerInfo> playerInfos;
        GamePlayer player;

        public void Init()
        { }

        public void Start()
        {
            player.Start();
        }

        public void Stop()
        {
            player.Stop();
        }
    }
}


