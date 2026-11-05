using ControlBoard.Interface;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// IO输出脉冲状态机 - 专用于发送脉冲信号
    /// </summary>
    public class IOPulseOutput
    {
        #region 私有字段
        private readonly object _lock = new object();
        private readonly int _cardNo;
        private readonly int _portNo;
        private readonly IControlCard _cardService;

        // 脉冲参数
        private int _pulseWidthMs = 10;
        private bool _idleLevel = true;      // 空闲电平：true=高, false=低
        private bool _pulseLevel = false;    // 脉冲电平：true=高, false=低（NPN通常为低）

        // 状态
        private OutputState _state = OutputState.Idle;
        private DateTime _stateStartTime;
        private bool _isEnabled = true;
        private bool _isSaveLog = true;

        // 回调
        private Action _onPulseStarted;
        private Action _onPulseCompleted;
        private Action<Exception> _onError;
        #endregion

        #region 枚举
        private enum OutputState
        {
            Idle,       // 空闲（保持空闲电平）
            ActivePulse // 脉冲激活中
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建输出脉冲状态机（默认NPN：低电平脉冲，空闲高电平）
        /// </summary>
        public IOPulseOutput(int cardNo, int portNo, IControlCard cardService,
                             int pulseWidthMs = 10)
        {
            _cardNo = cardNo;
            _portNo = portNo;
            _cardService = cardService ?? throw new ArgumentNullException(nameof(cardService));
            _pulseWidthMs = pulseWidthMs > 0 ? pulseWidthMs : 10;


            // 默认NPN：空闲高电平，脉冲低电平
            AsNPN();
        }
        #endregion

        #region 配置方法
        /// <summary>
        /// 设置为NPN模式（低电平脉冲，空闲高电平）
        /// </summary>
        public IOPulseOutput AsNPN()
        {
            lock (_lock)
            {
                _idleLevel = true;
                _pulseLevel = false;
                if (_state == OutputState.Idle)
                    WriteIO(_idleLevel, "设置NPN模式");
                return this;
            }
        }

        /// <summary>
        /// 设置为PNP模式（高电平脉冲，空闲低电平）
        /// </summary>
        public IOPulseOutput AsPNP()
        {
            lock (_lock)
            {
                _idleLevel = false;
                _pulseLevel = true;
                if (_state == OutputState.Idle)
                    WriteIO(_idleLevel, "设置PNP模式");
                return this;
            }
        }

        /// <summary>
        /// 设置脉冲宽度（毫秒）
        /// </summary>
        public IOPulseOutput SetPulseWidth(int ms)
        {
            lock (_lock)
            {
                _pulseWidthMs = ms > 0 ? ms : 10;
                return this;
            }
        }

        /// <summary>
        /// 设置空闲电平（true=高电平，false=低电平）
        /// </summary>
        public IOPulseOutput SetIdleLevel(bool highLevel)
        {
            lock (_lock)
            {
                _idleLevel = highLevel;
                if (_state == OutputState.Idle)
                    WriteIO(_idleLevel, "设置空闲电平");
                return this;
            }
        }
        #endregion

        #region 触发方法
        /// <summary>
        /// 触发脉冲（使用配置的脉冲电平）
        /// </summary>
        public bool Trigger(string message)
        {
            lock (_lock)
            {
                if (!_isEnabled) return false;
                if (_state != OutputState.Idle) return false;  // 正在脉冲中，拒绝重入

                // 输出脉冲电平
                WriteIO(_pulseLevel, message);
                _state = OutputState.ActivePulse;
                _stateStartTime = DateTime.Now;

                _onPulseStarted?.Invoke();
                return true;
            }
        }

        /// <summary>
        /// 触发低电平脉冲（覆盖配置，仅本次有效）
        /// </summary>
        public bool TriggerLow(string message)
        {
            lock (_lock)
            {
                if (!_isEnabled) return false;
                if (_state != OutputState.Idle) return false;

                WriteIO(false, message);
                _state = OutputState.ActivePulse;
                _stateStartTime = DateTime.Now;

                _onPulseStarted?.Invoke();
                return true;
            }
        }

        /// <summary>
        /// 触发高电平脉冲（覆盖配置，仅本次有效）
        /// </summary>
        public bool TriggerHigh(string message)
        {
            lock (_lock)
            {
                if (!_isEnabled) return false;
                if (_state != OutputState.Idle) return false;

                WriteIO(true, message);
                _state = OutputState.ActivePulse;
                _stateStartTime = DateTime.Now;

                _onPulseStarted?.Invoke();
                return true;
            }
        }

        /// <summary>
        /// 立即停止脉冲，恢复到空闲电平
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (_state == OutputState.Idle) return;

                WriteIO(_idleLevel, "StopReset");
                _state = OutputState.Idle;
            }
        }
        #endregion

        #region 定时器处理
        /// <summary>
        /// 定时器处理（需要外部每1ms调用）
        /// </summary>
        public void OnTimerTick()
        {
            lock (_lock)
            {
                if (_state != OutputState.ActivePulse) return;

                var elapsed = (DateTime.Now - _stateStartTime).TotalMilliseconds;
                if (elapsed >= _pulseWidthMs)
                {
                    // 脉冲结束，恢复空闲电平
                    WriteIO(_idleLevel, "StopReset");
                    _state = OutputState.Idle;

                    _onPulseCompleted?.Invoke();
                }
            }
        }

        /// <summary>
        /// 获取上一次输出时间是否大于设置时间
        /// </summary>
        /// <param name="intervalTime"></param>
        /// <returns></returns>
        public bool GetIsArrivalTime(int intervalTime)
        {
            if ((DateTime.Now - _stateStartTime).TotalMilliseconds > intervalTime)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 回调注册
        public IOPulseOutput OnStarted(Action callback)
        {
            _onPulseStarted = callback;
            return this;
        }

        public IOPulseOutput OnCompleted(Action callback)
        {
            _onPulseCompleted = callback;
            return this;
        }

        public IOPulseOutput OnError(Action<Exception> callback)
        {
            _onError = callback;
            return this;
        }
        #endregion

        #region 控制方法
        /// <summary>
        /// 启用/禁用输出
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            lock (_lock)
            {
                _isEnabled = enabled;
                if (!enabled && _state != OutputState.Idle)
                {
                    WriteIO(_idleLevel, "StopReset");
                    _state = OutputState.Idle;
                }
            }
        }

        /// <summary>
        /// 重置状态（异常恢复）
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                WriteIO(_idleLevel, "StopReset");
                _state = OutputState.Idle;
                _isEnabled = true;
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        public IOPulseOutput SetIsSaveLog(bool isSaveLog)
        {
            _isSaveLog = isSaveLog;
            return this;
        }
        #endregion

        #region 属性
        public int CardNo => _cardNo;
        public int PortNo => _portNo;
        public bool IsBusy => _state != OutputState.Idle;
        public bool IsEnabled => _isEnabled;
        public string State => _state.ToString();
        #endregion

        #region 私有方法
        private void WriteIO(bool value, string message)
        {
            try
            {
                int net = value ? 1 : 0;
                int nResult = _cardService.Write_outbit((ushort)_cardNo, (ushort)_portNo, net);
                if (_isSaveLog)
                {
                    if (nResult == 0)
                    {
                        MessageNotify.AddCommLog($"[IOPulseOutput] 写入成功{nResult}: Card{_cardNo}-Port{_portNo}-State{net}--{message}");
                    }
                    else
                    {
                        MessageNotify.AddCommLog($"[IOPulseOutput] 写入失败{nResult}: Card{_cardNo}-Port{_portNo}-State{net}--{message}");
                    }
                        
                }
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"[IOPulseOutput] 写入异常: Card{_cardNo}-Port{_portNo}, {ex.Message}");
                _onError?.Invoke(ex);
                throw;
            }
        }

        #endregion
    }
}
