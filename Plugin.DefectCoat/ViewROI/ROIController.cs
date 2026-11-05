using System;
using HalconDotNet;
using System.Collections;



namespace ViewROI
{

    /// <summary>ROI 控制器外部通知委托，用于通知界面刷新或同步参数。</summary>
    public delegate void FuncROIDelegate();

    /// <summary>
    /// ROI 交互控制器。
    /// 负责管理 ROI 列表、当前激活 ROI、鼠标创建/选中/拖动动作，
    /// 并把正 ROI 和负 ROI 合成为最终 ModelROI。
    /// HWndCtrl 将鼠标事件转成图像坐标后交给该类处理。
    /// </summary>
    public class ROIController
    {
        /// <summary>
        /// 手柄命中半径缩放系数，窗口缩放后用于保持拖拽手感一致。
        /// </summary>
        public double size = 1;

        /// <summary>
        /// 正 ROI，参与最终区域并集。
        /// </summary>
        public const int MODE_ROI_POS = 21;

        /// <summary>
        /// 负 ROI，参与最终区域扣除。
        /// </summary>
        public const int MODE_ROI_NEG = 22;

        /// <summary>
        /// 未选择 ROI 绘制模式。
        /// </summary>
        public const int MODE_ROI_NONE = 23;

        /// <summary>ROI 列表或 ModelROI 更新事件。</summary>
        public const int EVENT_UPDATE_ROI = 50;

        /// <summary>ROI 正负标记变化事件。</summary>
        public const int EVENT_CHANGED_ROI_SIGN = 51;

        /// <summary>ROI 正在拖动事件。</summary>
        public const int EVENT_MOVING_ROI = 52;

        /// <summary>删除当前激活 ROI 事件。</summary>
        public const int EVENT_DELETED_ACTROI = 53;

        /// <summary>删除全部 ROI 事件。</summary>
        public const int EVENT_DELETED_ALL_ROIS = 54;

        /// <summary>激活某个 ROI 事件。</summary>
        public const int EVENT_ACTIVATED_ROI = 55;

        /// <summary>创建新 ROI 事件。</summary>
        public const int EVENT_CREATED_ROI = 56;


        /// <summary>待创建的 ROI 原型。为 null 时表示当前不处于新建 ROI 模式。</summary>
        public ROI roiMode;
        /// <summary>当前 ROI 绘制模式，取 MODE_ROI_POS、MODE_ROI_NEG 或 MODE_ROI_NONE。</summary>
        private int stateROI;
        /// <summary>最近一次鼠标移动的图像坐标，用于过滤重复移动事件。</summary>
        private double currX, currY;


        /// <summary>当前激活 ROI 在 ROIList 中的索引，-1 表示没有选中。</summary>
        public int activeROIidx;

        /// <summary>最近一次删除的 ROI 索引，供外部同步列表使用。</summary>
        public int deletedIdx;

        /// <summary>当前所有 ROI 对象列表。</summary>
        public ArrayList ROIList;

        /// <summary>
        /// 正 ROI 减去负 ROI 后得到的最终模型区域。
        /// </summary>
        public HRegion ModelROI;

        /// <summary>当前激活 ROI 的绘制颜色。</summary>
        private string activeCol = "orange";
        /// <summary>当前激活控制点的绘制颜色。</summary>
        private string activeHdlCol = "red";
        /// <summary>非激活 ROI 的绘制颜色。</summary>
        private string inactiveCol = "blue";

        /// <summary>
        /// 绑定的 HALCON 窗口控制器，用于重绘和坐标刷新。
        /// </summary>
        public HWndCtrl viewController;

        /// <summary>
        /// ROI 状态变化通知回调。
        /// </summary>
        public IconicDelegate NotifyRCObserver;

        /// <summary>创建 ROI 控制器并初始化为空 ROI 列表。</summary>
        public ROIController()
        {
            stateROI = MODE_ROI_NONE;
            ROIList = new ArrayList();
            activeROIidx = -1;
            ModelROI = new HRegion();
            NotifyRCObserver = new IconicDelegate(dummyI);
            deletedIdx = -1;
            currX = currY = -1;
        }

        /// <summary>绑定 HWndCtrl，后续 ROI 变化通过它触发窗口重绘。</summary>
        public void setViewController(HWndCtrl view)
        {
            viewController = view;
        }

        /// <summary>获取当前正负 ROI 合成后的模型区域。</summary>
        public HRegion getModelRegion()
        {
            return ModelROI;
        }

        /// <summary>获取当前 ROI 列表。</summary>
        public ArrayList getROIList()
        {
            return ROIList;
        }

        /// <summary>获取当前激活 ROI。</summary>
        public ROI getActiveROI()
        {
            if (activeROIidx != -1)
                return ((ROI)ROIList[activeROIidx]);

            return null;
        }

        /// <summary>获取当前激活 ROI 的索引，-1 表示没有选中 ROI。</summary>
        public int getActiveROIIdx()
        {
            return activeROIidx;
        }

        /// <summary>设置当前激活 ROI 索引。调用方必须保证索引存在于 ROIList 中。</summary>
        public void setActiveROIIdx(int active)
        {
            activeROIidx = active;
        }

        /// <summary>获取最近一次删除的 ROI 索引。</summary>
        public int getDelROIIdx()
        {
            return deletedIdx;
        }

        /// <summary>
        /// 设置接下来要创建的 ROI 形状。
        /// 鼠标下一次按下时会调用该 ROI 的 createROI，并把新对象加入 ROIList。
        /// </summary>
        /// <param name="r">
        /// ROI 形状对象，例如 ROIRectangle1、ROICircle。
        /// </param>
        public void setROIShape(ROI r)
        {
            roiMode = r;
            roiMode.setOperatorFlag(stateROI);
        }


        /// <summary>
        /// 设置当前 ROI 绘制模式。已有激活 ROI 时，同步更新该 ROI 的正负标记。
        /// </summary>
        public void setROISign(int mode)
        {
            stateROI = mode;

            if (activeROIidx != -1)
            {
                ((ROI)ROIList[activeROIidx]).setOperatorFlag(stateROI);
                viewController.repaint();
                NotifyRCObserver(ROIController.EVENT_CHANGED_ROI_SIGN);
            }
        }

        /// <summary>
        /// 删除当前激活 ROI，并通知外部刷新。
        /// </summary>
        public void removeActive()
        {
            if (activeROIidx != -1)
            {
                ROIList.RemoveAt(activeROIidx);
                deletedIdx = activeROIidx;
                activeROIidx = -1;
                viewController.repaint();
                NotifyRCObserver(EVENT_DELETED_ACTROI);
            }
        }


        /// <summary>
        /// 将 ROIList 中所有正 ROI 做并集，再扣除负 ROI，生成 ModelROI。
        /// </summary>
        public bool defineModelROI()
        {
            HRegion tmpAdd, tmpDiff, tmp;
            double row, col;

            if (stateROI == MODE_ROI_NONE)
                return true;

            tmpAdd = new HRegion();
            tmpDiff = new HRegion();
            tmpAdd.GenEmptyRegion();
            tmpDiff.GenEmptyRegion();

            for (int i = 0; i < ROIList.Count; i++)
            {
                switch (((ROI)ROIList[i]).getOperatorFlag())
                {
                    case ROI.POSITIVE_FLAG:
                        tmp = ((ROI)ROIList[i]).getRegion();
                        tmpAdd = tmp.Union2(tmpAdd);
                        break;
                    case ROI.NEGATIVE_FLAG:
                        tmp = ((ROI)ROIList[i]).getRegion();
                        tmpDiff = tmp.Union2(tmpDiff);
                        break;
                    default:
                        break;
                }//end of switch
            }//end of for

            ModelROI = null;

            if (tmpAdd.AreaCenter(out row, out col) > 0)
            {
                tmp = tmpAdd.Difference(tmpDiff);
                if (tmp.AreaCenter(out row, out col) > 0)
                    ModelROI = tmp;
            }

            //in case the set of positiv and negative ROIs dissolve 
            if (ModelROI == null || ROIList.Count == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 清空 ROI 列表和当前模型区域。
        /// </summary>
        public void reset()
        {
            ROIList.Clear();
            activeROIidx = -1;
            ModelROI = null;
            roiMode = null;
            NotifyRCObserver(EVENT_DELETED_ALL_ROIS);
        }


        /// <summary>
        /// 退出当前 ROI 创建/激活状态。
        /// 不清空 ROIList，只取消激活索引和待创建原型。
        /// </summary>
        public void resetROI()
        {
            activeROIidx = -1;
            roiMode = null;
        }

        /// <summary>设置 ROI 绘制颜色。</summary>
        /// <param name="aColor">Color for the active ROI object</param>
        /// <param name="inaColor">Color for the inactive ROI objects</param>
        /// <param name="aHdlColor">
        /// Color for the active handle of the active ROI object
        /// </param>
        public void setDrawColor(string aColor,
                                   string aHdlColor,
                                   string inaColor)
        {
            if (aColor != "")
                activeCol = aColor;
            if (aHdlColor != "")
                activeHdlCol = aHdlColor;
            if (inaColor != "")
                inactiveCol = inaColor;
        }


        /// <summary>
        /// 绘制 ROI 列表。非激活 ROI 用 inactiveCol，激活 ROI 和控制点分别用 activeCol、activeHdlCol。
        /// </summary>
        /// <param name="window">HALCON window</param>
        public void paintData(HalconDotNet.HWindow window)
        {
            window.SetDraw("margin");
            window.SetLineWidth(1);

            if (ROIList.Count > 0)
            {
                window.SetColor(inactiveCol);
                window.SetDraw("margin");

                for (int i = 0; i < ROIList.Count; i++)
                {
                    window.SetLineStyle(((ROI)ROIList[i]).flagLineStyle);
                    ((ROI)ROIList[i]).draw(window);
                }

                if (activeROIidx != -1)
                {
                    window.SetColor(activeCol);
                    window.SetLineStyle(((ROI)ROIList[activeROIidx]).flagLineStyle);
                    ((ROI)ROIList[activeROIidx]).draw(window);

                    window.SetColor(activeHdlCol);
                    ((ROI)ROIList[activeROIidx]).displayActive(window);
                }
            }
        }

        /// <summary>
        /// 鼠标按下处理：有 roiMode 时创建新 ROI；否则在已有 ROI 中查找最近控制点并激活。
        /// </summary>
        /// <param name="imgX">x coordinate of mouse event</param>
        /// <param name="imgY">y coordinate of mouse event</param>
        /// <returns></returns>
        public int mouseDownAction(double imgX, double imgY)
        {
            int idxROI = -1;
            double max = 10000, dist = 0;
            double epsilon = 11 * size; // 控制点命中距离阈值。


            if (roiMode != null) // 当前处于新建 ROI 模式。
            {
                try
                {
                    //roiMode = new ROI();
                    roiMode.createROI(imgX, imgY);
                    ROIList.Add(roiMode);
                    roiMode = null;
                    activeROIidx = ROIList.Count - 1;
                    viewController.repaint();

                    NotifyRCObserver(ROIController.EVENT_CREATED_ROI);

                }
                catch (Exception)
                { }

            }
            else if (ROIList.Count > 0) // 当前处于选择或激活已有 ROI 模式。
            {
                activeROIidx = -1;

                for (int i = 0; i < ROIList.Count; i++)
                {
                    dist = ((ROI)ROIList[i]).distToClosestHandle(imgX, imgY);
                    if ((dist < max) && (dist < epsilon))
                    {
                        max = dist;
                        idxROI = i;
                    }
                }

                if (idxROI >= 0)
                {
                    activeROIidx = idxROI;
                    NotifyRCObserver(ROIController.EVENT_ACTIVATED_ROI);
                }

                viewController.repaint();
            }
            return activeROIidx;
        }

        /// <summary>
        /// 鼠标移动处理：拖动当前激活 ROI 的控制点并实时重绘。
        /// </summary>
        /// <param name="newX">x coordinate of mouse event</param>
        /// <param name="newY">y coordinate of mouse event</param>
        public void mouseMoveAction(double newX, double newY)
        {
            try
            {
                if ((newX == currX) && (newY == currY))
                    return;

                ((ROI)ROIList[activeROIidx]).moveByHandle(newX, newY);
                viewController.repaint();
                currX = newX;
                currY = newY;
                NotifyRCObserver(ROIController.EVENT_MOVING_ROI);
            }
            catch
            {
            }
        }


        /// <summary>默认空通知回调，避免外部未订阅时出现空引用。</summary>
        public void dummyI(int v)
        {
        }

    }
}
