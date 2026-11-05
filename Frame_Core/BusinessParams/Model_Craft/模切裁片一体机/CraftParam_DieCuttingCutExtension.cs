using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class CraftParam_DieCuttingCutExtension
    {
        public static List<string> Get_Cam_SizeNames(this CraftParam_DieCuttingCut Crafts, int camIndex)
        {
            if (Crafts == null)
            {
                return new List<string>();
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return new List<string>();
            }
            return Crafts.Crafts[camIndex].l_SizeCraft?.Select(p => p.Name).ToList() ?? new List<string>();
        }
        public static List<string> Get_Cam_DefectNames(this CraftParam_DieCuttingCut Crafts, int camIndex)
        {
            if (Crafts == null)
            {
                return new List<string>();
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return new List<string>();
            }
            return Crafts.Crafts[camIndex].l_DefectCraft?.Select(p => p.Name).ToList() ?? new List<string>();
        }

        public static List<string> Get_Station3_SizeNames(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return new List<string>();
            }
            if (Crafts.l_SizeCraftStation_3 == null)
            {
                return new List<string>();
            }
            return Crafts.l_SizeCraftStation_3?.Select(p => p.Name).ToList() ?? new List<string>();
        }
        public static List<string> Get_Station1_DefectNames(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return new List<string>();
            }
            if (Crafts.l_DefectCraftStation_1 == null)
            {
                return new List<string>();
            }
            return Crafts.l_DefectCraftStation_1?.Select(p => p.Name).ToList() ?? new List<string>();
        }

        public static List<string> Get_Station3_DefectNames(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return new List<string>();
            }
            if (Crafts.l_DefectCraftStation_3 == null)
            {
                return new List<string>();
            }
            return Crafts.l_DefectCraftStation_3?.Select(p => p.Name).ToList() ?? new List<string>();
        }

        public static string Get_SizeNames(this List<SizeCraft_DieCuttingCut>  sizeCraft_DieCuttingCuts, int index)
        {

            if (sizeCraft_DieCuttingCuts == null || index < 0 || index >= sizeCraft_DieCuttingCuts.Count)
            {
                return "";
            }

            return sizeCraft_DieCuttingCuts[index].Name;
        }

        public static string Get_SizeNames(this List<SizeCraftStation_DieCuttingCut> sizeCraftStation_DieCuttingCuts, int index)
        {

            if (sizeCraftStation_DieCuttingCuts == null || index < 0 || index >= sizeCraftStation_DieCuttingCuts.Count)
            {
                return "";
            }

            return sizeCraftStation_DieCuttingCuts[index].Name;
        }

        public static string Get_DefectNames(this List<DefectCraft_DieCuttingCut>  defectCraft_DieCuttingCuts, int index)
        {

            if (defectCraft_DieCuttingCuts == null || index < 0 || index >= defectCraft_DieCuttingCuts.Count)
            {
                return "";
            }

            return defectCraft_DieCuttingCuts[index].Name;
        }



        public static List<SizeInfo> Get_Cam_SizeDefaultInfo(this CraftParam_DieCuttingCut Crafts, int camIndex)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return null;
            }
            return Crafts.Crafts[camIndex].l_SizeCraft.Select(item =>
            {
                return item.SizeCraft_DieCuttingCutToSizeInfo();

            }).ToList();
        }
        public static SizeInfo SizeCraft_DieCuttingCutToSizeInfo(this SizeCraft_DieCuttingCut sizeCraft_DieCuttingCut)
        {
            SizeInfo sizeInfo = new SizeInfo();
            sizeInfo.Name = sizeCraft_DieCuttingCut.Name;
            sizeInfo.Level = sizeCraft_DieCuttingCut.Id;
            sizeInfo.StandardValue = sizeCraft_DieCuttingCut.StandardValue;
            sizeInfo.TolUpper = sizeCraft_DieCuttingCut.DBTolUpper;
            sizeInfo.TolLower = sizeCraft_DieCuttingCut.DBTolLower;
            sizeInfo.upperLimitValue = sizeInfo.StandardValue + sizeInfo.TolUpper;
            sizeInfo.lowerLimitValue = sizeInfo.StandardValue - sizeInfo.TolLower;
            return sizeInfo;
        }
        public static SizeInfo SizeCraftStation_DieCuttingCutToSizeInfo(this SizeCraftStation_DieCuttingCut sizeCraft_DieCuttingCut)
        {
            SizeInfo sizeInfo = new SizeInfo();
            sizeInfo.Name = sizeCraft_DieCuttingCut.Name;
            sizeInfo.Level = sizeCraft_DieCuttingCut.Id;
            sizeInfo.StandardValue = sizeCraft_DieCuttingCut.StandardValue;
            sizeInfo.TolUpper = sizeCraft_DieCuttingCut.DBTolUpper;
            sizeInfo.TolLower = sizeCraft_DieCuttingCut.DBTolLower;
            sizeInfo.upperLimitValue = sizeInfo.StandardValue + sizeInfo.TolUpper;
            sizeInfo.lowerLimitValue = sizeInfo.StandardValue - sizeInfo.TolLower;
            return sizeInfo;
        }

        public static List<SizeInfo> Get_Cam_SizeDefaultInfo(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_SizeCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_SizeCraftStation_3.Select(item =>
            {
                return item.SizeCraftStation_DieCuttingCutToSizeInfo();

            }).ToList();
        }



        public static List<SizeCraft_DieCuttingCut> Get_Cam_SizeCrafts(this CraftParam_DieCuttingCut Crafts, int camIndex)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return null;
            }
            return Crafts.Crafts[camIndex].l_SizeCraft;
        }



        public static List<DefectCraft_DieCuttingCut> Get_Cam_DefectCrafts(this CraftParam_DieCuttingCut Crafts, int camIndex)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return null;
            }
            return Crafts.Crafts[camIndex].l_DefectCraft;
        }

        public static SizeCraft_DieCuttingCut Get_Cam_SizeCraft(this CraftParam_DieCuttingCut Crafts, int camIndex, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return null;
            }
            return Crafts.Crafts[camIndex].l_SizeCraft?.Find(p => p.Name == name);
        }

        public static DefectCraft_DieCuttingCut Get_Cam_DefectCraft(this CraftParam_DieCuttingCut Crafts, int camIndex, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return null;
            }
            return Crafts.Crafts[camIndex].l_DefectCraft?.Find(p => p.Name == name);
        }

        public static DefectCraft_DieCuttingCut Get_Station1_DefectCrafts(this CraftParam_DieCuttingCut Crafts, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_DefectCraftStation_1 == null)
            {
                return null;
            }
            return Crafts.l_DefectCraftStation_1.Find(p => p.Name == name);
        }


        public static List<SizeCraftStation_DieCuttingCut> Get_Station3_SizeCrafts(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_SizeCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_SizeCraftStation_3;
        }

        public static List<SizeCraftStation_DieCuttingCut> Get_Station3_SizeCrafts(this CraftParam_DieCuttingCut Crafts, int camIndex)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_SizeCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_SizeCraftStation_3.FindAll(p => p.CameraId1 == camIndex && p.CameraId2 == camIndex);
        }

        public static List<SizeCraftStation_DieCuttingCut> Get_Station3_SizeCrafts_CamDifferent(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_SizeCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_SizeCraftStation_3.FindAll(p => p.CameraId1 != p.CameraId2);
        }

        public static SizeCraftStation_DieCuttingCut Get_Station3_SizeCraft(this CraftParam_DieCuttingCut Crafts, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_SizeCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_SizeCraftStation_3?.Find(p => p.Name == name);
        }

        public static List<DefectCraft_DieCuttingCut> Get_Station3_DefectCrafts(this CraftParam_DieCuttingCut Crafts)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_DefectCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_DefectCraftStation_3;
        }

        public static DefectCraft_DieCuttingCut Get_Station3_DefectCrafts(this CraftParam_DieCuttingCut Crafts, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.l_DefectCraftStation_3 == null)
            {
                return null;
            }
            return Crafts.l_DefectCraftStation_3.Find(p => p.Name == name);
        }



        public static GlobalVariableItem Get_GlobalVariableItem(this CraftParam_DieCuttingCut Crafts, int camIndex, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.Crafts == null || camIndex < 0 || camIndex >= Crafts.Crafts.Count)
            {
                return null;
            }
            return Crafts.Crafts[camIndex].M_GlobalVariable?.l_GlobalVariableItems?.Find(p => p.Name == name);
        }

        public static GlobalVariableItem Get_GlobalVariableItem(this CraftParam_DieCuttingCut Crafts, string name)
        {
            if (Crafts == null)
            {
                return null;
            }
            if (Crafts.M_GlobalVariable == null || Crafts.M_GlobalVariable.l_GlobalVariableItems == null)
            {
                return null;
            }
            return Crafts.M_GlobalVariable.l_GlobalVariableItems?.Find(p => p.Name == name);
        }

        public static double ToTryDouble(this string strType, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(strType))
                return defaultValue;

            return double.TryParse(strType, out double result)
              ? result
              : defaultValue;
        }

        public static double ToDouble(this string strType, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(strType))
                return defaultValue;

            return double.Parse(strType);
        }

        public static bool ToTryBool(this string strType, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(strType))
                return defaultValue;

            return bool.TryParse(strType, out bool result)
              ? result
              : defaultValue;
        }

        public static bool ToBool(this string strType, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(strType))
                return defaultValue;

            return bool.Parse(strType);
        }

        public static int ToTryInt(this string strType, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(strType))
                return defaultValue;

            return int.TryParse(strType, out int result)
              ? result
              : defaultValue;
        }

        public static int ToInt(this string strType, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(strType))
                return defaultValue;

            return int.Parse(strType);
        }
    }
}
