using Eholly.ZChart;
using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class VM_CraftDieCuttingCut
    {
        public CraftParam_DieCuttingCut M_CraftParam = new CraftParam_DieCuttingCut();


        public void Init(CraftParam_DieCuttingCut craftParam)
        {
            M_CraftParam = craftParam;
        }

        #region 缺陷工艺
        public void AddDefect(List<DefectCraft_DieCuttingCut> defectCraft_DieCuttingCuts, DefectCraft_DieCuttingCut defectCraft)
        {
            if (defectCraft_DieCuttingCuts == null) return;
            if (defectCraft == null) return;

            defectCraft_DieCuttingCuts?.Add(defectCraft);

            //ID重新排序
            defectCraft_DieCuttingCuts.SortID();          
        }

        public void DeleteDefect(List<DefectCraft_DieCuttingCut> defectCraft_DieCuttingCuts, int index)
        {
            if (defectCraft_DieCuttingCuts == null) return;
            if (index < 0 || index >= defectCraft_DieCuttingCuts.Count) return;

            if (defectCraft_DieCuttingCuts?.Count() > index)
                defectCraft_DieCuttingCuts?.RemoveAt(index);

            //ID重新排序
            defectCraft_DieCuttingCuts.SortID();
        }

        public void Top(List<DefectCraft_DieCuttingCut> defectCraft_DieCuttingCuts, int index)
        {
            if (defectCraft_DieCuttingCuts == null) return;
            if (index < 0 || index >= defectCraft_DieCuttingCuts.Count) return;

            // 如果已经在最顶部，无法上移
            if (index == 0) return;

            var item = defectCraft_DieCuttingCuts[index];
            defectCraft_DieCuttingCuts.RemoveAt(index);
            defectCraft_DieCuttingCuts.Insert(0, item);

            //ID重新排序
            defectCraft_DieCuttingCuts.SortID();
        }

        /// <summary>
        /// 将指定索引的元素向上移动一位
        /// </summary>
        public void MoveUp(List<DefectCraft_DieCuttingCut> defectCraft_DieCuttingCuts, int index)
        {
            if (defectCraft_DieCuttingCuts == null) return;
            if (index < 0 || index >= defectCraft_DieCuttingCuts.Count) return;

            // 如果已经在最顶部，无法上移
            if (index == 0) return;


            // 交换当前元素和前一个元素
            var current = defectCraft_DieCuttingCuts[index];
            defectCraft_DieCuttingCuts[index] = defectCraft_DieCuttingCuts[index - 1];
            defectCraft_DieCuttingCuts[index - 1] = current;

            //ID重新排序
            defectCraft_DieCuttingCuts.SortID();
        }

        /// <summary>
        /// 将指定索引的元素向下移动一位
        /// </summary>
        public void MoveDown(List<DefectCraft_DieCuttingCut> defectCraft_DieCuttingCuts, int index)
        {
            // 参数验证
            if (defectCraft_DieCuttingCuts == null)
                return;

            if (index < 0 || index >= defectCraft_DieCuttingCuts.Count)
                return;

            // 如果已经在最底部，无法下移
            if (index == defectCraft_DieCuttingCuts.Count - 1) return;


            // 交换当前元素和后一个元素
            var current = defectCraft_DieCuttingCuts[index];
            defectCraft_DieCuttingCuts[index] = defectCraft_DieCuttingCuts[index + 1];
            defectCraft_DieCuttingCuts[index + 1] = current;


            //ID重新排序
            defectCraft_DieCuttingCuts.SortID();
        }

        public void AddDefect(CamCraft camCraft, DefectCraft_DieCuttingCut defectCraft)
        {
            if (camCraft == null) return;
            if (defectCraft == null) return;
            if (camCraft?.l_DefectCraft == null) return;

            camCraft?.l_DefectCraft?.Add(defectCraft);

            //ID重新排序
            camCraft?.l_DefectCraft?.SortID();
        }

        public void DeleteDefect(CamCraft camCraft, int index)
        {
            if (camCraft?.l_DefectCraft == null) return;
            if (index < 0 || index >= camCraft?.l_DefectCraft.Count) return;

            if (camCraft?.l_DefectCraft?.Count() > index)
                camCraft?.l_DefectCraft?.RemoveAt(index);

            //ID重新排序
            camCraft?.l_DefectCraft?.SortID();
        }
        #endregion

        #region 尺寸工艺
        public void AddSizeCraft(List<SizeCraft_DieCuttingCut> sizeCrafts, SizeCraft_DieCuttingCut sizeCraft)
        {
            if (sizeCrafts == null) return;
            if (sizeCraft == null) return;

            sizeCrafts?.Add(sizeCraft);

            //ID重新排序
            sizeCrafts?.SortID();
        }

        public void DeleteSizeCraft(List<SizeCraft_DieCuttingCut> sizeCrafts, int index)
        {
            if (sizeCrafts == null) return;
            if (index < 0 || index >= sizeCrafts.Count) return;

            if (sizeCrafts?.Count() > index)
                sizeCrafts?.RemoveAt(index);

            //ID重新排序
            sizeCrafts?.SortID();
        }

        public void Top(List<SizeCraft_DieCuttingCut> crafts, int index)
        {
            if (crafts == null) return;
            if (index < 0 || index >= crafts.Count) return;

            // 如果已经在最顶部，无法上移
            if (index == 0) return;

            var item = crafts[index];
            crafts.RemoveAt(index);
            crafts.Insert(0, item);

            //ID重新排序
            crafts.SortID();
        }

        /// <summary>
        /// 将指定索引的元素向上移动一位
        /// </summary>
        public void MoveUp(List<SizeCraft_DieCuttingCut> crafts, int index)
        {
            if (crafts == null) return;
            if (index < 0 || index >= crafts.Count) return;

            // 如果已经在最顶部，无法上移
            if (index == 0) return;


            // 交换当前元素和前一个元素
            var current = crafts[index];
            crafts[index] = crafts[index - 1];
            crafts[index - 1] = current;

            //ID重新排序
            crafts.SortID();
        }

        /// <summary>
        /// 将指定索引的元素向下移动一位
        /// </summary>
        public void MoveDown(List<SizeCraft_DieCuttingCut> crafts, int index)
        {
            // 参数验证
            if (crafts == null)
                return;

            if (index < 0 || index >= crafts.Count)
                return;

            // 如果已经在最底部，无法下移
            if (index == crafts.Count - 1) return;


            // 交换当前元素和后一个元素
            var current = crafts[index];
            crafts[index] = crafts[index + 1];
            crafts[index + 1] = current;


            //ID重新排序
            crafts.SortID();
        }
        #endregion

        #region 工位三尺寸工艺
        public void AddSizeCraftStation_3(List<SizeCraftStation_DieCuttingCut> sizeCraftStations, SizeCraftStation_DieCuttingCut craftStation3)
        {
            if (sizeCraftStations == null) return;
            if (craftStation3 == null) return;

            sizeCraftStations?.Add(craftStation3);

            //ID重新排序
            sizeCraftStations?.SortID();
        }

        public void DeleteSizeCraftStation_3(List<SizeCraftStation_DieCuttingCut> sizeCraftStations, int index)
        {
            if (sizeCraftStations == null) return;
            if (index < 0 || index >= sizeCraftStations.Count) return;

            if (sizeCraftStations?.Count() > index)
                sizeCraftStations?.RemoveAt(index);

            //ID重新排序
            sizeCraftStations?.SortID();
        }

        public void Top(List<SizeCraftStation_DieCuttingCut> crafts, int index)
        {
            if (crafts == null) return;
            if (index < 0 || index >= crafts.Count) return;

            // 如果已经在最顶部，无法上移
            if (index == 0) return;

            var item = crafts[index];
            crafts.RemoveAt(index);
            crafts.Insert(0, item);

            //ID重新排序
            crafts.SortID();
        }

        /// <summary>
        /// 将指定索引的元素向上移动一位
        /// </summary>
        public void MoveUp(List<SizeCraftStation_DieCuttingCut> crafts, int index)
        {
            if (crafts == null) return;
            if (index < 0 || index >= crafts.Count) return;

            // 如果已经在最顶部，无法上移
            if (index == 0) return;


            // 交换当前元素和前一个元素
            var current = crafts[index];
            crafts[index] = crafts[index - 1];
            crafts[index - 1] = current;

            //ID重新排序
            crafts.SortID();
        }

        /// <summary>
        /// 将指定索引的元素向下移动一位
        /// </summary>
        public void MoveDown(List<SizeCraftStation_DieCuttingCut> crafts, int index)
        {
            // 参数验证
            if (crafts == null)
                return;

            if (index < 0 || index >= crafts.Count)
                return;

            // 如果已经在最底部，无法下移
            if (index == crafts.Count - 1) return;


            // 交换当前元素和后一个元素
            var current = crafts[index];
            crafts[index] = crafts[index + 1];
            crafts[index + 1] = current;


            //ID重新排序
            crafts.SortID();
        }
        #endregion


    }
}
