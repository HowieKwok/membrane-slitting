using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace FrameCommunity.MaskRoiTool
{
    [Serializable]
    public class MaskRoiOptions : INotifyPropertyChanged
    {
        private bool enableMaskRoi;
        private bool showMaskRoi = true;
        private bool editMaskRoi;
        private int selectedIndex = -1;
        private List<MaskRoiParam> maskRois = new List<MaskRoiParam>();

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public bool EnableMaskRoi
        {
            get { return enableMaskRoi; }
            set { SetProperty(ref enableMaskRoi, value); }
        }

        public bool ShowMaskRoi
        {
            get { return showMaskRoi; }
            set { SetProperty(ref showMaskRoi, value); }
        }

        public bool EditMaskRoi
        {
            get { return editMaskRoi; }
            set { SetProperty(ref editMaskRoi, value); }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }

        [XmlArrayItem("MaskRoiParam", typeof(MaskRoiParam))]
        public List<MaskRoiParam> MaskRois
        {
            get { return maskRois; }
            set
            {
                maskRois = value ?? new List<MaskRoiParam>();
                OnPropertyChanged();
            }
        }

        public void EnsureList()
        {
            if (maskRois == null) maskRois = new List<MaskRoiParam>();
        }

        internal void NotifyMaskRoisChanged()
        {
            OnPropertyChanged(nameof(MaskRois));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
