using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Black_N_White
{
    public class Plate_Struct
    {
        public struct UInt64Plate
        {
            public Boolean isInit;
            public UInt64 plate;
            public UInt64 white;
            public UInt64 black;
            public UInt64[] Mask;
            public UInt64 Lastplate;
            public UInt64 Lastwhite;
            public UInt64 Lastblack;
            public int whitecount;
            public int blackcount;
            public int count;
            public int lastwhitecount;
            public int lastblackcount;
            public int id;
        }

        private static UInt64Plate Plate;

        public static UInt64Plate getPlate()
        {
            if (!Plate.isInit)
            {
                InitPlate();
            }
            return Plate;
        }

        public static int[] DirectionArray = new int[] { +1, -1, +8, -8, +9, +7, -7, -9 };

        public static int getColor(int id, UInt64Plate Plate)
        {
            if ((Plate.Mask[id] & Plate.white) != 0)
                return 1;
            if ((Plate.Mask[id] & Plate.black) != 0)
                return -1;

            return 0;
        }

        private static void InitPlate()
        {
            Plate.Mask = new UInt64[64];
            Plate.isInit = true;
            for (int i = 0; i < 64; i++)
            {
                Plate.Mask[i] = setMask(i);
            }
            Plate.black |= (Plate.Mask[27] | Plate.Mask[36]);
            Plate.white |= (Plate.Mask[35] | Plate.Mask[28]);
            Plate.plate = Plate.black | Plate.white;
            Plate.blackcount = 2;
            Plate.whitecount = 2;
            Plate.lastblackcount = 0;
            Plate.lastwhitecount = 0;

            for (int i = 0; i < 8; i++)
            {
                Line1_8 |= (UInt64)(1 | (((UInt64)1) << 7)) << i * 8;
            }
            cellValue = new int[64];

            for (int i = 0; i < 64; i++)
            {
                int value = 1;
                if (i < 8 || i > 55)
                    value++;
                if ((i + 1) % 8 == 0 || (i % 8 == 0))
                    value++;

                cellValue[i] = (int)Math.Pow(value, 3);
            }
        }

        private static UInt64 setMask(int i)
        {
            return ((UInt64)1) << i;

        }

        /// <summary>
        /// 获取需要被设置的id列表
        /// </summary>
        /// <param name="x0">列</param>
        /// <param name="y0">行</param>
        /// <param name="currentColor">当前颜色</param>
        /// <param name="Plate">棋盘</param>
        /// <param name="set">需要被设置的棋盘id组成的UInt64</param>
        /// <returns></returns>
        public static List<int> getsetid(int x0, int y0, int currentColor, Plate_Struct.UInt64Plate Plate, out UInt64 set)
        {
            List<int> directionList = Plate_Struct.DirectionArray.ToList();
            List<int> setid = new List<int>();
            int id0 = y0 * 8 + x0 - 9;//id

            set = Plate.Mask[id0];
            setid.Add(id0);
            for (int a = 1; ; a++)
            {
                if (directionList.Count == 0)
                    break;
                for (int i = 0; i < directionList.Count; i++)//8个方向
                {
                    int id = id0 + directionList[i] * a;
                    if (id < 0 || id > 63)//超出边界时，不计算这个方向
                    {
                        directionList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    int y = id / 8 + 1;
                    int x = id - (y - 1) * 8 + 1;

                    int dx = Math.Abs(x - x0);
                    int dy = Math.Abs(y - y0);

                    if (!(dx == 0 || dx == a) || !(dy == 0 || dy == a))//不连续，跨行或跨列,移除方向
                    {
                        directionList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    else
                    {
                        int Color = Plate_Struct.getColor(id, Plate);
                        if (Color == currentColor && a == 1)
                        {
                            directionList.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (Color == 0)
                        {
                            directionList.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (Color == currentColor && a > 1)
                        {
                            for (int j = 1; j <= a; j++)
                            {
                                set |= Plate.Mask[id0 + directionList[i] * j];
                                if (j != a)
                                    setid.Add(id0 + directionList[i] * j);
                            }
                            directionList.RemoveAt(i);
                            i--;
                            continue;
                        }

                    }
                }
            }
            return setid;
        }


        static UInt64 Line1_8 = 0;
        /// <summary>
        /// 获取可以下子的位置
        /// </summary>
        /// <param name="Color">颜色</param>
        /// <param name="Plate">棋盘</param>
        /// <param name="set">UInt64</param>
        /// <returns></returns>
        public static List<int> getidList(int Color, UInt64Plate Plate, out UInt64 set)
        {
            set = (UInt64)0;
            List<int> idList = new List<int>();
            UInt64 surround = (Plate.plate << 8) | (Plate.plate >> 8) | Plate.plate;

            UInt64 PlateWithoutLine1_8 = surround & (~Line1_8);
            surround |= (PlateWithoutLine1_8 << 1 | PlateWithoutLine1_8 >> 1);
            surround &= ~Plate.plate;

            if (Color == -1)
            {
                UInt64 whitesurround = (Plate.white << 8) | (Plate.white >> 8) | Plate.white;
                PlateWithoutLine1_8 = whitesurround & (~Line1_8);
                whitesurround |= (PlateWithoutLine1_8 << 1 | PlateWithoutLine1_8 >> 1);
                whitesurround &= ~Plate.white;

                set = surround & whitesurround;
            }
            else
            {
                UInt64 blacksurround = (Plate.black << 8) | (Plate.black >> 8) | Plate.black;
                PlateWithoutLine1_8 = blacksurround & (~Line1_8);
                blacksurround |= (PlateWithoutLine1_8 << 1 | PlateWithoutLine1_8 >> 1);
                blacksurround &= ~Plate.black;

                set = surround & blacksurround;
            }

            //set = surround;

            for (int i = 0; i < 64; i++)
            {
                if ((set & Plate.Mask[i]) != 0)
                {
                    idList.Add(i);
                }
            }

            return idList;

        }

        //每格价值：四角：27 边界：8 普通：1
        private static int[] cellValue;
        /// <summary>
        /// 估值函数
        /// </summary>
        /// <param name="Plate"></param>
        /// <param name="WhiteValue"></param>
        /// <param name="BlackValue"></param>
        private static void Evaluate(UInt64Plate Plate, out double WhiteValue, out double BlackValue)
        {
            WhiteValue = 0;
            BlackValue = 0;
            int Count = 0;
            for (int i = 0; i < 64; i++)
            {
                Count += (Plate.plate & Plate.Mask[i]) == 0 ? 0 : 1;
                WhiteValue += (Plate.white & Plate.Mask[i]) != 0
                    ? cellValue[i]
                    : 0;// ((Plate.black & Plate.Mask[i]) != 0 ? -cellValue[i] : 0);
                BlackValue += (Plate.black & Plate.Mask[i]) != 0
                    ? -cellValue[i]
                    : 0;// ((Plate.white & Plate.Mask[i]) != 0 ? cellValue[i] : 0);
            }

            int WhiteMobility = 0;
            int BlackMobility = 0;

            UInt64 set;
            List<int> WhiteMobilityList = getidList(1, Plate, out set);
            List<int> BlackMobilityList = getidList(-1, Plate, out set);

            double tag = Count / 64F * 1;//行动力权重，格子越多，行动力权重越小，格子越多格子权重越高

            foreach (var i in WhiteMobilityList)
            {
                int thisCellMobility = -cellValue[i];
                List<int> getSetChessList = getSetChess(i, 1, Plate);
                if (getSetChessList.Count == 0)
                    continue;
                foreach (var a in getSetChessList)
                {
                    thisCellMobility += cellValue[a];
                }
                WhiteMobility += thisCellMobility;
            }

            WhiteValue = (WhiteValue * tag + (WhiteMobility + BlackMobility) * (1 - tag));

            foreach (var i in BlackMobilityList)
            {
                int thisCellMobility = cellValue[i];
                List<int> getSetChessList = getSetChess(i, -1, Plate);
                if (getSetChessList.Count == 0)
                    continue;
                foreach (var a in getSetChessList)
                {
                    thisCellMobility -= cellValue[a];
                }
                BlackMobility -= thisCellMobility;
            }

            BlackValue = (BlackValue * tag + (WhiteMobility + BlackMobility) * (1 - tag));
        }

        public static Boolean CheckPlate(int Color, UInt64Plate Plate, out List<int> CellList)
        {
            UInt64 set;
            CellList = new List<int>();
            List<int> idList = getidList(Color, Plate, out set);//可能可以下子的id列表
            foreach (var id in idList)
            {
                UInt64 ChesstoSetUInt64;
                int y = id / 8 + 1;
                int x = id - y * 8 + 1 + 8;
                if (x <= 0 || y <= 0 || id < 0 || id > 63)
                    continue;
                List<int> ChesstoSet = getsetid(x, y, Color, Plate, out ChesstoSetUInt64);
                if (ChesstoSet.Count <= 1)
                    continue;
                CellList.Add(id);
            }
            if (CellList.Count == 0)
                return false;
            else
                return true;
        }
        public static int CacuLate(UInt64Plate Plate, int CurrentColor, int depth)
        {
            List<UInt64Plate> Depth_0 = new List<UInt64Plate>();
            List<UInt64Plate> Depth_1 = new List<UInt64Plate>();

            Depth_0.Add(Plate);
            List<int> thisturnidList = new List<int>();
            int Color = -CurrentColor;
            for (int d = 0; d < depth; d++)//搜索某深度下的所有着法
            {
                if (d != 0)
                    Depth_0.Clear();
                Depth_0.AddRange(Depth_1);
                Depth_1.Clear();
                foreach (var i in Depth_0)
                {
                    UInt64 set;
                    List<int> idList = getidList(Color, Plate, out set);//可能可以下子的id列表
                    foreach (var id in idList)
                    {
                        UInt64 ChesstoSetUInt64;
                        int y = id / 8 + 1;
                        int x = id - y * 8 + 1 + 8;
                        if (x <= 0 || y <= 0 || id < 0 || id > 63)
                            continue;
                        List<int> ChesstoSet = getsetid(x, y, Color, Plate, out ChesstoSetUInt64);
                        if (ChesstoSet.Count <= 1)
                            continue;
                        UInt64Plate NextPlate = setPlate(Plate, Color, ChesstoSet, ChesstoSetUInt64);
                        if (d == 0)
                        {
                            NextPlate.id = id;
                        }
                        else
                            NextPlate.id = i.id;
                        Depth_1.Add(NextPlate);

                    }
                    if (d == 0)//第一层时
                    {
                        thisturnidList = idList;
                    }
                    Color *= -1;
                }
            }

            double BlackValue_0, WhiteValue_0;
            Evaluate(Plate, out WhiteValue_0, out BlackValue_0);//计算原先分差


            Dictionary<int, int> Best = new Dictionary<int, int>();
            double Max = 0;


            Color = -CurrentColor;

            for (int i = 0; i < Depth_1.Count; i++)
            {
                if (i < 0 || i > 63)
                    continue;
                double blackValue, whiteValue;
                Evaluate(Depth_1[i], out whiteValue, out blackValue);

                if (Color * ((whiteValue + blackValue) - (BlackValue_0 + WhiteValue_0)) < 0)//n深度下的着法局面评估
                {
                    //局势更糟了
                    continue;
                }
                else
                {
                    if (Max < Color * ((whiteValue + blackValue) - (BlackValue_0 + WhiteValue_0)))
                    {
                        Max = Color * ((whiteValue + blackValue) - (BlackValue_0 + WhiteValue_0));
                        Best.Clear();
                    }
                    else if (Max == Color * ((whiteValue + blackValue) - (BlackValue_0 + WhiteValue_0)))
                    {
                        if (Best.ContainsKey(Depth_1[i].id))
                        {
                            Best[Depth_1[i].id]++;
                        }
                        else
                            Best.Add(Depth_1[i].id, 1);
                    }
                }
            }

            int BestinBest = -1;
            int MaxCount = 0;
            foreach (var i in Best)
            {
                if (i.Value > MaxCount)
                {
                    BestinBest = i.Key;
                    MaxCount = i.Value;
                }
            }


            return BestinBest;
        }



        /// <summary>
        /// 获取需要被翻的列表
        /// </summary>
        /// <param name="id0"></param>
        /// <param name="currentColor"></param>
        /// <param name="Plate"></param>
        /// <returns></returns>
        private static List<int> getSetChess(int id0, int currentColor, Plate_Struct.UInt64Plate Plate)
        {
            List<int> directionList = Plate_Struct.DirectionArray.ToList();
            List<int> setid = new List<int>();
            int x0 = id0 - id0 / 8 * 8 + 1;
            int y0 = id0 / 8 + 1;
            //int id0 = y0 * 8 + x0 - 9;//id
            //set = Plate.Mask[id0];
            setid.Add(id0);
            for (int a = 1; ; a++)
            {
                if (directionList.Count == 0)
                    break;
                for (int i = 0; i < directionList.Count; i++)//8个方向
                {
                    int id = id0 + directionList[i] * a;
                    if (id < 0 || id > 63)//超出边界时，不计算这个方向
                    {
                        directionList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    int y = id / 8 + 1;
                    int x = id - (y - 1) * 8 + 1;

                    int dx = Math.Abs(x - x0);
                    int dy = Math.Abs(y - y0);

                    if (!(dx == 0 || dx == a) || !(dy == 0 || dy == a))//不连续，跨行或跨列,移除方向
                    {
                        directionList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    else
                    {
                        int Color = Plate_Struct.getColor(id, Plate);
                        if (Color == currentColor && a == 1)
                        {
                            directionList.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (Color == 0)
                        {
                            directionList.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (Color == currentColor && a > 1)
                        {
                            for (int j = 1; j <= a; j++)
                            {
                                //set |= Plate.Mask[id0 + directionList[i] * j];
                                if (j != a)
                                    setid.Add(id0 + directionList[i] * j);
                            }
                            directionList.RemoveAt(i);
                            i--;
                            continue;
                        }

                    }
                }
            }
            return setid;
        }

        /// <summary>
        /// 设置棋盘
        /// </summary>
        /// <param name="Plate">棋盘</param>
        /// <param name="currentColor">当前需要被设置的颜色</param>
        /// <param name="setid">需要被设置的格子id</param>
        /// <param name="set">需要被设置的格子id组成的UInt64</param>
        /// <returns></returns>
        public static Plate_Struct.UInt64Plate setPlate(Plate_Struct.UInt64Plate Plate, int currentColor, List<int> setid, UInt64 set)
        {
            Plate.Lastplate = Plate.plate;
            Plate.Lastblack = Plate.black;
            Plate.Lastwhite = Plate.white;
            Plate.lastblackcount = Plate.blackcount;
            Plate.lastwhitecount = Plate.whitecount;

            if (currentColor == -1)
            {
                Plate.blackcount += setid.Count;
                Plate.whitecount -= setid.Count - 1;
                Plate.black |= set;
                Plate.plate |= set;
                Plate.white &= (~set);
            }
            else if (currentColor == 1)
            {
                Plate.whitecount += setid.Count;
                Plate.blackcount -= setid.Count - 1;
                Plate.white |= set;
                Plate.plate |= set;
                Plate.black &= (~set);
            }
            Plate.count = Plate.whitecount + Plate.blackcount;
            return Plate;
        }
    }

}
