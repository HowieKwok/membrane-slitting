using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlBoard.Interface;
using ControlBoard.Model;
using ControlBoard.服务类;
using FrameCommunity.Data;
using FrameCommunity.公共类;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlBoard.ViewModels.IO640
{

    public partial class IO640UserViewModel : ObservableObject
    {
        #region 原有注入与字段
        private  IControlCard _controlCard;
        private CancellationTokenSource _pollCts;
        private ParamControlCord _boardParam;
        // 记录切换之前的板卡型号，取消切换时回滚
        private EnumControlType _oldBoardType;
        private bool _disposed;

        #endregion

        #region 原有绑定属性
        public ParamControlCord BoardParam
        {
            get => _boardParam;
            set
            {
                if (SetProperty(ref _boardParam, value))
                {
                    if (_boardParam != null)
                    {
                        BoardTypeChangedCommand.Execute(_boardParam.ControlType);
                    }
                }
            }
        }
        public ObservableCollection<EnumControlType> BoardTypeItems { get; }
        #endregion

        #region 新增：硬件总状态 + IO点位集合
        private bool _hardwareConnected;
        public bool HardwareConnected
        {
            //get => _hardwareConnected;
            //set => SetProperty(ref _hardwareConnected, value);

            get
            {
                if(_controlCard!=null)
                {
                    _hardwareConnected= _controlCard.IsConnnected()!=0;
                }
                else
                {
                    _hardwareConnected = false; // 默认未连接
                }
                return _hardwareConnected;
            }
        }

        public ObservableCollection<IoPointStatus> DiPointList { get; }
        public ObservableCollection<IoPointStatus> DoPointList { get; }
        #endregion

        #region 【新增DO单点调试属性】独立调试参数，不侵入BoardParam
        private ushort _debugDoIndex;
        /// <summary>调试输出点位索引 0~31</summary>
        public ushort DebugDoIndex
        {
            get => _debugDoIndex;
            set => SetProperty(ref _debugDoIndex, value);
        }

        private int _debugDoLevel;
        /// <summary>输出电平 1=高电平，0=低电平</summary>
        public int DebugDoLevel
        {
            get => _debugDoLevel;
            set => SetProperty(ref _debugDoLevel, value);
        }


        /// <summary>
        /// 卡号
        /// </summary>
        [Description("卡号")]
        public ushort CardNo { get { return BoardParam.CardNo; } set { SetProperty(ref BoardParam.CardNo, value);  } }


        /// <summary>
        /// 端口号
        /// </summary>
        [Description("端口号")]
        public ushort m_PortNo { get { return BoardParam.m_PortNo; } set { SetProperty(ref BoardParam.m_PortNo, value);  } } 


        /// <summary>
        /// 控制卡类型
        /// </summary>
        [Description("控制卡类型")]  //枚举类型
        public EnumControlType ControlType { get {return BoardParam.ControlType; } set { SetProperty(ref BoardParam.ControlType, value); } }

        /// <summary>
        /// 设置滤波时间
        /// </summary>
        public double FilteTime { get { return BoardParam.FilteTime; } 
            set {
                SetProperty(ref BoardParam.FilteTime, value);  ;
            } }



        #endregion

        #region 原有命令
        public RelayCommand<EnumControlType> BoardTypeChangedCommand { get; }
        public RelayCommand SaveParamCommand { get; }

        public RelayCommand InitCommand { get; }
        public RelayCommand LoadParamCommand { get; }
        public RelayCommand ResetParamCommand { get; }
        public RelayCommand StartIoPollCommand { get; }
        public RelayCommand StopIoPollCommand { get; }
        #endregion

        #region 【新增】DO单点输出触发命令
        public RelayCommand DoSingleOutCommand { get; }
        #endregion

        #region 构造函数
        public IO640UserViewModel(IControlCard controlCard, ParamControlCord paramControlCord)
        {
            _controlCard = controlCard ?? throw new ArgumentNullException(nameof(controlCard));
            _boardParam = paramControlCord ?? throw new ArgumentNullException(nameof(paramControlCord));

            BoardTypeItems = new ObservableCollection<EnumControlType>(Enum.GetValues(typeof(EnumControlType)).Cast<EnumControlType>());

            //初始化32路IO指示灯
            DiPointList = new ObservableCollection<IoPointStatus>();
            DoPointList = new ObservableCollection<IoPointStatus>();
            for (int i = 0; i < 32; i++)
            {
                DiPointList.Add(new IoPointStatus { Index = i });
                DoPointList.Add(new IoPointStatus { Index = i });
            }

            //初始化调试默认值
            DebugDoIndex = 0;
            DebugDoLevel = 1;

            //原有命令实例化
            BoardTypeChangedCommand = new RelayCommand<EnumControlType>(OnBoardTypeSelectChanged);
            SaveParamCommand = new RelayCommand(ExecuteSaveParam);
            InitCommand = new RelayCommand(initTest);
            LoadParamCommand = new RelayCommand(ExecuteLoadParam);
            ResetParamCommand = new RelayCommand(ExecuteResetParam);
            StartIoPollCommand = new RelayCommand(StartIoPollTask);
            StopIoPollCommand = new RelayCommand(StopIoPollTask);

            //单点输出命令绑定执行方法
            DoSingleOutCommand = new RelayCommand(ExecuteSingleDoOutput);
        }
        #endregion

        #region 原有业务逻辑

        /// <summary>
        /// ComboBox选中变更入口，弹窗确认
        /// </summary>
        private void OnBoardTypeSelectChanged(EnumControlType newSelectType)
        {
            if (BoardParam == null) return;

            // 选中值没有变化，直接退出
            if (newSelectType == BoardParam.ControlType)
                return;

            // 保存当前旧型号
            _oldBoardType = BoardParam.ControlType;

            // 弹窗确认
            var dialogResult = MessageBox.Show(
                $"即将切换板卡类型为【{newSelectType}】，需要关闭现有板卡并重新初始化，是否继续？",
                "板卡切换确认",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                // 确认：执行板卡重载逻辑
                ExecuteBoardTypeChanged(newSelectType);
            }
            else
            {
                // 取消：直接回退ComboBox选中值为旧型号
                BoardParam.ControlType = _oldBoardType;
                OnPropertyChanged(nameof(BoardParam));
            }
        }


        public void initTest()
        {
          
        }

        private void ExecuteBoardTypeChanged(EnumControlType selectType)
        {
            if (BoardParam == null) return;

            // 1、停止IO轮询线程
            StopIoPollTask();

            // 2、安全关闭上一个板卡
            try
            {
                if (HardwareConnected)
                {
                    _controlCard.Board_Close();
                }
            }
            catch (Exception closeEx)
            {
                Console.WriteLine($"旧板卡关闭异常：{closeEx.Message}");
            }

            // 更新参数型号
            BoardParam.ControlType = selectType;

            try
            {
                // 重新初始化板卡
                int initRet = ServiceCardManage.Instance.InitServiceCard(ServiceCardManage.Instance.Param控制卡);
                _controlCard= ServiceCardManage.Instance.GetIControlCard();
        
                // 初始化成功设置滤波
                if (_controlCard.IsConnnected()!=0)
                {
                    _controlCard.Set_Filter(BoardParam.CardNo, BoardParam.FilteTime);
                }
                OnPropertyChanged(nameof(BoardParam));
            }
            catch (Exception ex)
            {
             
                MessageBox.Show($"板卡初始化失败：{ex.Message}", "初始化错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"板卡初始化异常：{ex.Message}");
            }
        }

        private void ExecuteSaveParam()
        {

            BoardParam.Save();
            _controlCard.Set_Filter(BoardParam.CardNo, BoardParam.FilteTime);
            //BoardParam?.SaveToJson("IOBoardConfig.json");
        }

        private void ExecuteLoadParam()
        {
          //  BoardParam = ParamControlCord.LoadFromJson<ParamControlCord>("IOBoardConfig.json");
        }

        private void ExecuteResetParam()
        {
            BoardParam = new ParamControlCord();
            BoardParam.Save();
        }
        #endregion

        #region 【新增】单点DO输出逻辑
        private void ExecuteSingleDoOutput()
        {
            //硬件未连接直接返回
            if (!HardwareConnected|| BoardParam == null)
            {
                MessageBox.Show("硬件未连接，请先连接硬件！");
                return;
            }
                
            //限制点位0~31，防止越界
            if (DebugDoIndex > 31)
                return;
            try
            {
                //调用底层接口：Write_outbit(卡号,点位,电平)
                _controlCard.Write_outbit(BoardParam.CardNo, DebugDoIndex, DebugDoLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"单点输出失败:{ex.Message}");
            }
        }
        #endregion

        #region IO轮询刷新逻辑
        private void StartIoPollTask()
        {
            StopIoPollTask();
            _pollCts = new CancellationTokenSource();
            var token = _pollCts.Token;
          if (!HardwareConnected || BoardParam == null)
           {
                MessageBox.Show("硬件未连接，请先连接硬件！");
                return;
            }
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (!HardwareConnected || BoardParam == null)
                    {
                        await Task.Delay(100, token);
                        continue;
                    }
                    try
                    {
                        int diPortValue = _controlCard.Read_Inport(BoardParam.CardNo, BoardParam.m_PortNo);
                        int doPortValue = _controlCard.Read_Outport(BoardParam.CardNo, BoardParam.m_PortNo);

                        for (int i = 0; i < 32; i++)
                        {
                            DiPointList[i].IsOn = (diPortValue & (1 << i)) != 0;
                            DoPointList[i].IsOn = (doPortValue & (1 << i)) != 0;
                        }
                    }
                    catch
                    {
                        //HardwareConnected = false;
                    }
                    await Task.Delay(50, token);
                }
            }, token);
        }

        #region 资源整体清理（窗口关闭调用）
        public void CleanResource()
        {
            // 1、强制停止轮询线程
            StopIoPollTask();

            // 2、清空绑定集合，解除UI引用
            DiPointList.Clear();
            DoPointList.Clear();
        }
        #endregion

        #region Dispose标准实现
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                CleanResource();
            }
            _disposed = true;
        }

        ~IO640UserViewModel() => Dispose(false);
        #endregion


        private void StopIoPollTask()
        {
            _pollCts?.Cancel();
            _pollCts?.Dispose();
            _pollCts = null;
        }

        public void DisposeResource()
        {
            StopIoPollTask();
            _controlCard?.Board_Close();
        }
        #endregion
    
}
}
