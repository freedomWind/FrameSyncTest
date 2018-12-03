using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;
using Game.TempNet;

namespace FrameSync
{
    public struct CommandDefine
    {
        //area 
        public const int area_select = 1001;                    //战区选中
        public const int area_unselect = 1002;                  //战区取消选中
        public const int area_move = 1003;                      //战区移动
        public const int area_stop = 1004;                      //战区停止 
        public const int area_retreat = 1005;                   //战区撤退
        public const int area_lowhpRetreat = 1006;              //战区残血撤退
        //formation
        public const int formation_select = 2001;               //编队选择
        public const int formation_unselect = 2002;             //编队取消选择
        public const int formation_move = 2003;                 //编队移动
        public const int formation_stop = 2004;                 //编队停止
        public const int formation_retreat = 2005;              //编队撤退
        public const int formation_lowhpRetreat = 2006;         //编队残血撤退
        public const int formation_editor = 2007;               //编队编辑
        //connon
        public const int connon_open = 3001;                    //炮击界面打开 
        public const int connon_close = 3002;                   //炮击界面关闭
        public const int connon_fire = 3004;                    //炮击开炮
        //other
        public const int build = 4001;                          //建造
        public const int cancelBuild = 4002;                    //取消建造
        public const int freeSkill = 4003;                      //技能释放
        public const int upgrade = 4004;                        //升级
        //test
        public const int testKeyPress = 5000;                   //测试按键操作
    }

    public class PlayerCommand
    {
        public int logicTick;
        public object[] data;
        public PlayerCommand(int tick,int cmdID,params object[] data)
        {
            this.logicTick = tick;
            if (data.Length != 0)
            {
                this.data = new object[data.Length];
                data.CopyTo(this.data, 0);
            }
        }

        public override string ToString()
        {
            return "";
        }
    }

    public class PlayerCommandsMessage : IMessage
    {
        public List<PlayerCommand> cmdList;
        public PlayerCommandsMessage(IEnumerable<PlayerCommand> _list)
        {
            cmdList = new List<PlayerCommand>();
            cmdList.AddRange(_list);
        }
        public static PlayerCommandsMessage NewEmptyMsg()
        {
            return null;
        }
    }

    public sealed class CommandSystem 
    {
        #region  inner
        private static CommandSystem _sys = new CommandSystem();
        public static CommandSystem GetInstance()
        {
            return _sys;
        }
        private CommandSystem()
        {
        }
        #endregion
        const int MAX_LOGIC_FRAME = 24000;  //20min
        private Queue<PlayerCommand> playerOperateQue = new Queue<PlayerCommand>(20);
        private Dictionary<int, List<PlayerCommand>> totalCommandDic = new Dictionary<int, List<PlayerCommand>>(MAX_LOGIC_FRAME);
        int localLogicFrameTick
        {
            get { return 0; }
        }

        public void Init()
        {
            playerOperateQue.Clear();
            totalCommandDic.Clear();
        }

        public void NewCommand(int cmdid,params object[] data)
        {
            PlayerCommand cmd = new PlayerCommand(FGame.nowRealFrameIndex, cmdid, data);
            playerOperateQue.Enqueue(cmd);
        }

        //上传操作
        public void UpLoadCommand()
        {
            PlayerCommandsMessage msg = null;
            if (playerOperateQue.Count == 0)
            {
                msg = PlayerCommandsMessage.NewEmptyMsg();
            }
            else
            {
                msg = new PlayerCommandsMessage(playerOperateQue.ToArray());
            }
            NetMgr.getIns().SyncMsg(msg);
            playerOperateQue.Clear();
        }
        public void RecordCommand(int tick,List<PlayerCommand> cmdlist)
        {
            if (cmdlist == null)
                return;
            List<PlayerCommand> list = new List<PlayerCommand>(cmdlist);
            cmdlist.Clear();
            totalCommandDic.Add(tick, list);
        }
    }
}
namespace FrameSync
{
    ///// <summary>
    ///// 帧管理
    ///// </summary>
    //public class FrameManager
    //{   
    //    public Dictionary<int, List<IAction>> allActions;
    //}
}
public interface IFrameRender
{
}
public class RenderSystem : IGameSystem
{ }