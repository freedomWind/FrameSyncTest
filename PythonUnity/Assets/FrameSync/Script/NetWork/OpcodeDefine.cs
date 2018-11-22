using UnityEngine;
using System.Collections;
using System;

namespace Game.TempNet
{
    public class OpcodeID
    {
        public const ushort TimeAdjust = 000;     //
        public const ushort Ping = 1001;
        public const ushort Connect = 0001;

        public const ushort GroupInit = 3100;               //阵营初始化消息
        public const ushort GameReady = 1101;               //游戏准备
        public const ushort TimeCount = 3102;               //倒计时
        public const ushort GameStartLoading = 3103;               //游戏开始加载,由主机端发起
        public const ushort GameStartPlay = 3104;                //同步进入主界面

        public const ushort Login = 2001;
        public const ushort loadingProgress = 2011;         //加载进度
        public const ushort loadingOver = 1012;             //加载完毕
        public const ushort BuildUnit = 2021;               //建造
        public const ushort UnBuildUnit = 2022;             //取消建造
        public const ushort Skill = 2061;                   //技能

        public const ushort MoveUnit = 2031;                //移动
        public const ushort RetreatArea = 2032;                 //撤退
        public const ushort RetreatHPLowest = 2033;         //撤退HP最少的单位
        public const ushort ClearMoveCommands = 2034;         //撤退HP最少的单位

        public const ushort FormationCmd = 2035;            //编队相关命令
        public const ushort FormationJoin = 2036;           //编队初始化加入移出

        public const ushort ArtilleryCmd = 2040;            //炮击相关命令
        public const ushort ArtilleryFire = 2041;
        public const ushort ArtilleryAI = 2042;

        #region 科技升级相关
        public const ushort HQUpgrade = 2201;               //总部升级
        public const ushort RMGrenadaUpgrade = 2211;        //RM升级手雷
        #endregion
        public const ushort SelectCountry = 2221;               //选择国家


    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MessageAttribute : Attribute
    {
        public ushort ID;
        public MessageAttribute(ushort id)
        {
            this.ID = id;
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OpcodeSyncAttribute : Attribute
    {
        public ushort ID;
        public OpcodeSyncAttribute(ushort id)
        {
            this.ID = id;
        }
    }
}