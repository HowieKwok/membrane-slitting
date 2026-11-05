using DevMgmt.Cameras.Models;
using FrameCommunity;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevMgmt.Cameras.ViewModels
{
    public class AddCameraViewModel : BindableBase
    {
        // 🔥 关键：你的相机管理实例（外部传入）
        public CameraManagerViewModel CameraManager { get; set; }

        public List<CamTypeItem> AllCamTypes { get; set; } = new List<CamTypeItem>();
        public bool CanConfirm => AllCamTypes.Any(x => x.IsSelected);
        public CamTypeEnum? SelectedType => AllCamTypes.FirstOrDefault(x => x.IsSelected)?.Type;

        public event Action RequestClose;
        public ICommand ConfirmCommand { get; }
        public ICommand CloseCommand { get; }

        public AddCameraViewModel(CameraManagerViewModel _CameraManager)
        {
            CameraManager = _CameraManager;
            // 自动生成所有枚举选项
            foreach (CamTypeEnum type in Enum.GetValues(typeof(CamTypeEnum)))
            {
                AllCamTypes.Add(new CamTypeItem { Type = type });
            }

            // 选中变化刷新按钮
            foreach (var item in AllCamTypes)
            {
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CamTypeItem.IsSelected))
                        OnPropertyChanged(nameof(CanConfirm));
                };
            }

            // ✅ 确定按钮：执行 AddCam
            ConfirmCommand = new RelayCommand<object>(_ => ConfirmClick());
         
            //CloseCommand = new RelayCommand(() => RequestClose?.Invoke());
        }
        public void ConfirmClick()
        {
            if (SelectedType.HasValue && CameraManager != null)
            {
                // 🔥 直接调用你现有的方法
                CameraManager.AddCam(SelectedType.Value);
                //MessageBox.Show($"已添加相机：{SelectedType.Value}");
            }
            RequestClose?.Invoke();         
        }
    }


    public class CamTypeItem : INotifyPropertyChanged
    {
        public CamTypeEnum Type { get; set; }
        public string DisplayName => Type.ToString();

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 通用 RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged;
    }
}
