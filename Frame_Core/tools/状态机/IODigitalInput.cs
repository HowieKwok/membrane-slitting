using ControlBoard.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// IO数字输入状态机 - 专用于监控输入信号
    /// </summary>
    public class IODigitalInput
    {
        #region 私有字段
        private readonly object _lock = new object();
        private readonly int _cardNo;
        private readonly int _portNo;
        private readonly IControlCard _cardService;

        // 防抖参数
        private int _debounceMs = 10;

        // 状态
        private InputState _state = InputState.Normal;
        private DateTime _stateStartTime;
        private bool _lastReadValue;
        private bool _currentValue;
        private bool _isValueValid;
        private bool _isEnabled = true;
        private bool _invertLogic = false;  // 是否反转逻辑
        private int _lastReadValueRecord = 0;

        // 回调
        private Action<bool> _onValueChanged;
        private Action<bool> _onRisingEdge;   // 上升沿（低→高）
        private Action<bool> _onFallingEdge;  // 下降沿（高→低）
        private Action<Exception> _onError;
        #endregion

        #region 枚举
        private enum InputState
        {
            Normal,     // 正常监控
            Debouncing  // 防抖中
        }
        #endregion

        #region 构造函数
        public IODigitalInput(int cardNo, int portNo, IControlCard cardService,
                              int debounceMs = 10)
        {
            _cardNo = cardNo;
            _portNo = portNo;
            _cardService = cardService ?? throw new ArgumentNullException(nameof(cardService));
            _debounceMs = debounceMs > 0 ? debounceMs : 10;

            // 读取初始值
            _currentValue = ReadIO();
            _lastReadValue = _currentValue;
            _isValueValid = true;
        }
        #endregion

        #region 配置方法
        /// <summary>
        /// 设置防抖时间（毫秒）
        /// </summary>
        public IODigitalInput SetDebounce(int ms)
        {
            lock (_lock)
            {
                _debounceMs = ms > 0 ? ms : 10;
                return this;
            }
        }

        /// <summary>
        /// 反转逻辑（NC常闭模式）
        /// </summary>
        public IODigitalInput InvertLogic(bool invert = true)
        {
            lock (_lock)
            {
                _invertLogic = invert;
                return this;
            }
        }
        #endregion

        #region 读取方法
        /// <summary>
        /// 获取当前稳定值（带防抖处理）
        /// </summary>
        public bool GetValue()
        {
            lock (_lock)
            {
                if (!_isValueValid) return false;
                return _invertLogic ? !_currentValue : _currentValue;
            }
        }

        /// <summary>
        /// 获取原始值（不带防抖，用于调试）
        /// </summary>
        public bool GetRawValue()
        {
            bool raw = ReadIO();
            return _invertLogic ? !raw : raw;
        }

        /// <summary>
        /// 检查输入是否为高电平
        /// </summary>
        public bool IsHigh()
        {
            return GetValue();
        }

        /// <summary>
        /// 检查输入是否为低电平
        /// </summary>
        public bool IsLow()
        {
            return !GetValue();
        }
        #endregion

        #region 定时器处理
        /// <summary>
        /// 定时器处理（需要外部每1ms调用）
        /// </summary>    
        public void OnTimerTick()
        {
            if (!_isEnabled) return;

            lock (_lock)
            {
                bool currentRead = ReadIO();
                bool processedRead = _invertLogic ? !currentRead : currentRead;

                if (_state == InputState.Normal)
                {
                    // 检测到电平变化
                    if (processedRead != _lastReadValue)
                    {
                        _state = InputState.Debouncing;
                        _stateStartTime = DateTime.Now;
                        _lastReadValue = processedRead;

                        // ✅ 进入防抖，标记值无效
                        _isValueValid = false;
                    }
                }
                else if (_state == InputState.Debouncing)
                {
                    var elapsed = (DateTime.Now - _stateStartTime).TotalMilliseconds;

                    if (elapsed >= _debounceMs)
                    {
                        // 防抖结束，读取最终值
                        bool finalRead = ReadIO();
                        bool finalValue = _invertLogic ? !finalRead : finalRead;

                        // 值变化时触发回调
                        if (finalValue != _currentValue)
                        {
                            bool oldValue = _currentValue;
                            _currentValue = finalValue;
                            // ✅ 值已更新，标记有效
                            _isValueValid = true;

                            // 触发回调
                            _onValueChanged?.Invoke(finalValue);

                            if (finalValue && !oldValue)
                                _onRisingEdge?.Invoke(finalValue);
                            else if (!finalValue && oldValue)
                                _onFallingEdge?.Invoke(finalValue);
                        }
                        else
                        {
                            // ⚠️ 防抖后值没变，但也要恢复有效状态
                            _isValueValid = true;
                        }

                        _state = InputState.Normal;
                        _lastReadValue = finalValue;
                    }
                }
            }
        }
        #endregion

        #region 回调注册
        public IODigitalInput OnChanged(Action<bool> callback)
        {
            _onValueChanged = callback;
            return this;
        }

        /// <summary>
        /// 上升沿（低→高）
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IODigitalInput OnRising(Action<bool> callback)
        {
            _onRisingEdge = callback;
            return this;
        }

        /// <summary>
        /// /下降沿（高→低）
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IODigitalInput OnFalling(Action<bool> callback)
        {
            _onFallingEdge = callback;
            return this;
        }

        public IODigitalInput OnError(Action<Exception> callback)
        {
            _onError = callback;
            return this;
        }
        #endregion

        #region 控制方法
        public void SetEnabled(bool enabled)
        {
            lock (_lock)
            {
                _isEnabled = enabled;
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _state = InputState.Normal;
                _currentValue = ReadIO();
                _lastReadValue = _currentValue;
                _isValueValid = true;
                _isEnabled = true;
            }
        }
        #endregion

        #region 属性
        public int CardNo => _cardNo;
        public int PortNo => _portNo;
        public bool IsStable => _state == InputState.Normal;
        public bool IsEnabled => _isEnabled;
        public string State => _state.ToString();
        #endregion

        #region 私有方法
        private bool ReadIO()
        {
            try
            {
                // 根据实际SDK实现读取
                int net = _cardService.Read_Inbit((ushort)_cardNo, (ushort)_portNo);

                if (_lastReadValueRecord != net)
                {
                    _lastReadValueRecord = net;
                    MessageNotify.AddCommLog($"[IODigitalInput] 读取: Card{_cardNo}-Port{_portNo}, 值{net}");
                }
                return net == 1;
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"[IODigitalInput] 读取异常: Card{_cardNo}-Port{_portNo}, {ex.Message}");
                _onError?.Invoke(ex);
                return false;
            }
        }
        #endregion
    }
}
