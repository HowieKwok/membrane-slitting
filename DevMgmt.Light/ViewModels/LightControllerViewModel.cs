using DevMgmt.Light.Models;
using FrameCommunity;
using FrameCommunity.MessageNotification;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevMgmt.Light.SDK;

namespace DevMgmt.Light.ViewModels
{
    public class LightControllerViewModel : BindableBase
    {

        public LightControllerParam Param { get; set; }

        public LightControllerViewModel(LightControllerParam param)
        {
            Param = param;

        }

        // + 按钮命令：新增设备
        public ICommand AddDeviceCommand => new RelayCommand<object>(_ =>
        {
            var existingNames = Param.l_LightControllerDevs
        .Where(c => !string.IsNullOrEmpty(c.LightControllerParameter?.LightControllerName))
        .Select(c => c.LightControllerParameter.LightControllerName)
        .ToList();

            var ids = Param.l_LightControllerDevs
        .Where(c => !string.IsNullOrEmpty(c.LightControllerParameter?.LightControllerName))
        .Select(c => c.LightControllerParameter.ID)
        .ToList();

            var newDev = new LightControllerDev
            {
                LightControllerParameter = new LightControllerParameterBase
                {
                    ID = MessageHelper.FindMinimumId(ids),
                    LightControllerName = $"Light{MessageHelper.FindMinimumId(ids)}",
                    Model = 0,
                    PortName = 0,
                    BaudRate = 19200,
                    DataBits = 8,
                    Parity = 0, // 0:无 1:奇 2:偶 可自行映射
                    StopBits = 1
                    // 其他通道亮度会在构造里自动初始化默认值
                },
                ConnStatus = false
            };

            Param.l_LightControllerDevs.Add(newDev);

            // 默认选中新设备

            SelectSelectionIndex?.Invoke(Param.l_LightControllerDevs.Count - 1);
        });

        // 重命名命令
        public ICommand RenameCommand => new RelayCommand<LightControllerDev>(dev =>
        {
            if (dev == null) return;

            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "请输入新的设备名称：", "重命名", dev.LightControllerParameter.LightControllerName);

            if (!string.IsNullOrWhiteSpace(input))
            {
                dev.LightControllerParameter.LightControllerName = input;
            }
        });

        // 删除命令
        public ICommand DeleteCommand => new RelayCommand<LightControllerDev>(dev =>
        {
            if (dev == null) return;

            Param.l_LightControllerDevs.Remove(dev);
            SelectSelectionIndex?.Invoke(0);
        });

        // 连接/断开 切换命令
        public ICommand ToggleConnectCommand => new RelayCommand<LightControllerDev>(dev =>
        {
            if (dev == null) return;

            // 取反状态：断开 ↔ 连接
            //dev.ConnStatus = !dev.ConnStatus;

            // ========== 在这里补充你的硬件/串口连接、断开业务逻辑 ==========
            if (dev.ConnStatus)
            {
                dev.ConnectDev();
            }
            else
            {
                dev.CloseDev();
            }
        });

        public ICommand SelectFile => new RelayCommand<object>(dev =>
        {
            try
            {
                // 解包元组
                Tuple<LightControllerParameterBase, string> tuple = dev as Tuple<LightControllerParameterBase, string>;
                if (tuple == null)
                    return;
                var (param, propName) = tuple;

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "配置文件 (*.config;*.xml;*.cam)|*.config;*.xml;*.cam|所有文件 (*.*)|*.*",
                    Title = "选择相机配置文件",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // 反射赋值对应字符串属性，UI自动双向刷新
                    PropertyInfo propInfo = param.GetType().GetProperty(propName);
                    if (propInfo != null && propInfo.PropertyType == typeof(string))
                    {
                        propInfo.SetValue(param, openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        });
        public ICommand SelectFolder => new RelayCommand<object>(dev =>
        {
            try
            {
                // 解包元组
                Tuple<LightControllerParameterBase, string> tuple = dev as Tuple<LightControllerParameterBase, string>;
                if (tuple == null)
                    return;
                var (param, propName) = tuple;

                using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderDialog.Description = "请选择仿真图像目录";
                    //if (!string.IsNullOrEmpty(currentPath))
                    //{
                    //    folderDialog.SelectedPath = currentPath;
                    //}

                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // 5. 直接更新Model，依靠绑定自动刷新界面，不操作TextBox控件
                        PropertyInfo propInfo = param.GetType().GetProperty(propName);
                        propInfo.SetValue(param, folderDialog.SelectedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        });

        public ICommand SaveCommand => new RelayCommand<object>(_ =>
        {
            try
            {
                Param.Save();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("保存失败：" + ex.Message);
            }

        });

        public ICommand OepnClosingLight => new RelayCommand<object>(obj =>
        {
            try
            {
                // 强制转换元组
                var tuple = obj as Tuple<LightControllerDev, LightChannelItem>;
                if (tuple == null) return;

                // 取出两个参数
                var dev = tuple.Item1;      // LightControllerDev实例
                var channel = tuple.Item2; // 当前通道LightChannelItem

                if (!dev.ConnStatus)
                {
                    System.Windows.Forms.MessageBox.Show("设备未连接");
                    return;
                }

                if (channel.SwitchState)
                {
                    dev.ColseLight(channel.PortNo);
                }
                else
                {
                    dev.OpenLight(channel.PortNo);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        });

        // 新增通道
        public ICommand AddLightChannel => new RelayCommand<LightControllerDev>(dev =>
        {
            try
            {
                dev.LightControllerParameter.LightChannelItems.Add(new LightChannelItem()
                {
                    DisplayName = $"通道{dev.LightControllerParameter.LightChannelItems.Count + 1}亮度",
                    PortNo = dev.LightControllerParameter.LightChannelItems.Count + 1,

                });
                UpdateUI?.Invoke(dev);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        });
        // 删除最后一条通道
        public ICommand RemoveLightChannel => new RelayCommand<LightControllerDev>(dev =>
        {
            try
            {
                if (dev.LightControllerParameter.LightChannelItems.Count > 0)
                    dev.LightControllerParameter.LightChannelItems.RemoveAt(dev.LightControllerParameter.LightChannelItems.Count - 1);

                UpdateUI?.Invoke(dev);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        });

        // 选中设备并更新右侧参数
        public Action<int> SelectSelectionIndex;

        // 选中设备并更新右侧参数
        public Action<LightControllerDev> UpdateUI;
    }
}
