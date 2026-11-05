using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlBoard.工具
{
    /// <summary>
    /// 边沿检测器（无定时器，需外部循环调用 Update 方法）
    /// </summary>
    /// <typeparam name="T">必须实现 IEquatable<T>，可选实现 IComparable<T> 以支持上升/下降沿判断</typeparam>
    public class EdgeDetector<T> where T : IEquatable<T>
    {
        private T _lastValue;
        private bool _hasValue;  // 是否已初始化

        public event Action OnRisingEdge;      // 上升沿 (low->high)
        public event Action OnFallingEdge;     // 下降沿 (high->low)
        public event Action<T, T> OnChanged;   // 任意变化 (old, new)

        /// <summary>
        /// 设置初始值（可选）。若不调用，第一次 Update 时只记录状态不触发事件。
        /// </summary>
        public void SetInitial(T value)
        {
            _lastValue = value;
            _hasValue = true;
        }

        /// <summary>
        /// 检测当前值，若有变化则触发相应事件。
        /// 建议在外部循环中定期调用（如每 10~50ms 一次）。
        /// </summary>
        /// <param name="currentValue">当前读取的值</param>
        public void Update(T currentValue)
        {
            if (!_hasValue)
            {
                // 首次调用，只记录，不触发事件
                _lastValue = currentValue;
                _hasValue = true;
                return;
            }

            if (currentValue.Equals(_lastValue))
                return;

            // 尝试判断上升/下降沿（仅当 T 支持比较时）
            if (currentValue is IComparable<T> comparable)
            {
                int cmp = comparable.CompareTo(_lastValue);
                if (cmp > 0)
                    OnRisingEdge?.Invoke();
                else if (cmp < 0)
                    OnFallingEdge?.Invoke();
            }

            OnChanged?.Invoke(_lastValue, currentValue);
            _lastValue = currentValue;
        }

        /// <summary>
        /// 重置检测器（清除上一次状态）
        /// </summary>
        public void Reset()
        {
            _hasValue = false;
            _lastValue = default(T);
        }
    }
}
