using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameSync
{
    /// <summary>
    /// 关键帧
    /// </summary>
    public interface IKeyFrame
    { }

    public interface IFrameUpdate
    { }

    public interface IKeyFrameData
    {
        List<IAction> aActions { get; }
        List<IAction> bActions { get; }
    }

    /// <summary>
    /// 逻辑帧
    /// </summary>
    public interface IFrame
    { }



    /// <summary>
    /// 玩家操作行为
    /// </summary>
    public interface IAction
    {
        int turnID { get; }
        int commandID { get; }
        int uid { get; }
        object data { get; }   
    }

    public interface ICommand
    {
        void DoCommand(object data);
    }

    /// <summary>
    /// 逻辑单元
    /// </summary>
    public interface ILogicUnit
    {
        string unitCode { get; }
        IAction playerAction { get; }
    }

    public sealed class FGame
    {
        bool _isReady = false;
        static FGame _ins = new FGame();
        private FGame() { }
        public static FGame Instance { get { return _ins; } }
        public static int nowRealFrameIndex = -1;  //当前最新的逻辑帧
        public static int nowDoFrameIndex;         //当前渲染的帧
        public bool isReady { get { return _isReady; } }
        
        Client Aclient;
        Client Bclient;

        //客户端启动,开始逻辑帧tick
        public void Start()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);
            aTimer.Interval = 250;
            aTimer.Enabled = true;
            _isReady = true;
        }
        
        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DoFram();
        }
        //定时驱动
        void DoFram()
        {
            nowRealFrameIndex++;
            Debug.LogError("逻辑帧，客户端定时驱动帧："+nowRealFrameIndex);
            Aclient.FrameTick(nowRealFrameIndex);
            Bclient.FrameTick(nowRealFrameIndex);


        }
        /// <summary>
        /// 关键帧驱动
        /// </summary>
        /// <param name="keyFrameIndex"></param>
        /// <param name="data"></param>
        public void KeyFrameTick(ClientKeyFrameData keyFrameData)
        {
            
             
        }

    }

    public class Client
    {
        /// <summary>
        /// 收到关键帧
        /// </summary>
        void OnKeyframeTick()
        { }

        public void FrameTick(int frameIndex)
        {
            if (frameIndex % 5 == 0)       //关键帧上传
            {
                UpLoad();
            }
        }

        /// <summary>
        /// 上传关键帧数据
        /// </summary>
        public void UpLoad()
        {
            CommandSystem.GetInstance().UpLoadCommand();
        }
    }
    public struct ClientKeyFrameData
    {
        public int KeyFrameIndex;
        public List<IKeyFrameData> data;
    }
    /// <summary>
    /// 游戏服务器
    /// </summary>
    public class GameServer
    {
        FGame gameA;
        FGame gameB;
        public static GameServer Ins;
        Queue<ClientKeyFrameData> totalData;
        ClientKeyFrameData curKeyFrameData;   //当前等待的数据
        public void AddClientKeyFramedata(IKeyFrameData data)
        {
            curKeyFrameData.data.Add(data);
            //
            totalData.Enqueue(curKeyFrameData);
        }

        void Enqueue(ClientKeyFrameData data)
        {
            totalData.Enqueue(data);
            TranslateToClient();
        }

        /// <summary>
        /// 关键帧数据转发给每个客户端
        /// </summary>
        public void TranslateToClient()
        {
            ClientKeyFrameData data = totalData.Dequeue();
            gameA.KeyFrameTick(data);
            gameB.KeyFrameTick(data);
        }

        public void GameStart()
        {
            //通知客户端A,B分别启动
            gameA.Start();
            gameB.Start();
        }
    }
}
namespace FrameSyncEx
{
    //玩家操作：操作对应的逻辑帧，操作数据
    public interface IPlayerOperate
    {
        int FrameIndex { get; }
        object operateData { get; }
    }
    //关键帧数据：玩家id，所有操作
    public interface IKeyFrameData
    {
        int playerID { get; }
        int keyFrameIndex { get; }
        List<IPlayerOperate> operateList {get;}
        string ToString();
    }
    //服务器下发的所有玩家操作
    public interface IFrameDataBag
    {
        int keyFrameIndex { get; }
        List<IKeyFrameData> allPalyersData { get; }
        bool IsOK();
        void Clear();
    }
    //玩家
    public interface IPlayer
    {
        List<IClient> totalClients { get; }    //所有客户端
        void Start();
        void Stop();
        void FrameTick();                      //逻辑帧驱动
        void KeyFrameTick(IFrameDataBag bag);  //服务器关键帧驱动
        void FrameRender();                    //帧渲染
        void KeyFrameUpLoad();                 //关键帧上传
    }
    //客户端
    public interface IClient
    {
        void Start();
        void FrameTick();
        void FrameRender();
    }

    public interface IServer
    {
        void Start();
        void Stop();
        void KeyFrameTranslate();                          //关键帧数据转发给所有玩家
        void KeyFrameRecord(IKeyFrameData data);              //记录客户端上传的关键帧
    }


    public class GameServer : IServer
    {
        #region pro
        Queue<IFrameDataBag> gameFrameQue;
        IFrameDataBag thisFrameDataBag;
        List<IPlayer> _playerList;
        static GameServer _ins = new GameServer();
        #endregion
        private GameServer()
        {
            _playerList = new List<IPlayer>();
            _playerList.Add(new GamePlayer(111));
            _playerList.Add(new GamePlayer(222));
        }
        public static GameServer GetInstance()
        {
            return _ins;
        }
        public void Start()
        {
            _playerList.ForEach(player => {
                player.Start();
            });
        }
        public void Stop()
        {
            _playerList.ForEach(player => {
                player.Stop();
            });
        }
        public void KeyFrameTranslate()
        {
            IFrameDataBag bag = gameFrameQue.Dequeue();
            Util.LogServer(string.Format("下发关键帧{0}到所有玩家", bag.keyFrameIndex));
            _playerList.ForEach(player => {
                player.KeyFrameTick(bag);
            });
        }
        //搜集关键帧数据，当所有玩家关键帧数据搜集完毕，开始转发给所有玩家
        public void KeyFrameRecord(IKeyFrameData data)
        {
            Util.LogServer(string.Format("第{0}帧，收到玩家{1}的关键帧数据{2}",data.playerID,data.keyFrameIndex,data.ToString()));
            thisFrameDataBag.allPalyersData.Add(data);
            if (thisFrameDataBag.IsOK())
            {
                gameFrameQue.Enqueue(thisFrameDataBag);
                if (gameFrameQue.Count >= 2)
                {
                    KeyFrameTranslate();
                }
                thisFrameDataBag.Clear();
            }
        }
    }

    public class GamePlayer : IPlayer
    {
        #region   pro
        int logicFrameIndex = 0;
        int playerID;
        List<IClient> _totalClients;
        System.Timers.Timer aTimer;
        #endregion

        public List<IClient> totalClients { get { return _totalClients; } }
        public GamePlayer(int pid)
        {
            this.playerID = pid;
            _totalClients = new List<IClient>();
            _totalClients.Add(new Client(111));
            _totalClients.Add(new Client(222));
        }
        public void Start()
        {
            Util.LogPlayer(this.playerID, "玩家端开始");
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            aTimer.Interval = 250;
            aTimer.Enabled = true;
            totalClients.ForEach(client =>
            {
                client.Start();
            });
        }
        public void Stop()
        {
            if(aTimer != null)
                aTimer.Stop();
        }

        public void FrameTick()
        {
            logicFrameIndex++;
            totalClients.ForEach(client => {
                client.FrameTick();
            });
            if (logicFrameIndex % 5 == 0)
                KeyFrameUpLoad();
        }
        public void KeyFrameTick(IFrameDataBag bag)
        { }
        public void FrameRender()
        { }
        public void KeyFrameUpLoad()
        {
            GameServer.GetInstance().KeyFrameRecord(GetNowFrameData());
        }

        #region
        void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            FrameTick();
        }
        IKeyFrameData GetNowFrameData()
        {
            return null;
        }
        #endregion

    }
    public class Client : IClient
    {
        public int playerid;
        public Client(int pid)
        {
            this.playerid = pid;
        }
        public void Start()
        { }
        public void FrameTick()
        { }
        public void FrameRender()
        { }
    }
    public static class Util
    {
        public static void LogServer(string log)
        {
            Debug.Log(log);
        }

        public static void LogPlayer(int playerid, string log)
        {
            if (playerid == 111)
            {
                Debug.LogWarning(log);
            }
            else
                Debug.LogError(log);
        }
    }
}
namespace FrameSyncTest
{
    using FrameSyncEx;

    class MainTest
    {
        IServer ser;

        /// <summary>
        /// 1，服务器发送游戏开始命令，
        /// 2，所有玩家端执行游戏开始逻辑
        /// 3，玩家类开始帧tick，所有客户端根据tick驱动逻辑
        /// 
        /// 4，玩家每隔5个tick上传一次关键帧给服务器端
        /// 5，服务器收到所有客户端的关键帧后，开始下发上一个关键帧给所有玩家
        /// 6，玩家收到关键帧后，开始渲染
        /// 7，重复4，5，6，7过程
        /// 
        /// </summary>
        void Main()
        {
            GameServer.GetInstance().Start();    
        }
    }
}