using FrameCommunity;
using FrameCommunity.DataOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frame_Core
{
    public class VM_ImageSave : BindableBase
    {
        public NewImageSaveParamer Paramer;
        public void Init(NewImageSaveParamer newImageSaveParamer)
        {
            Paramer = newImageSaveParamer;
        }

        public ICommand SaveCommand => new RelayCommand<object>(_ =>
        {
            try
            {
                Paramer.Save();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("保存失败：" + ex.Message);
            }

        });
    }
}
