using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FrameCommunity.MaskRoiTool
{
    public sealed class MaskRoiEditorViewModel : BindableOrdinary
    {
        private MaskRoiOptions options;
        private MaskRoiItemViewModel selectedMaskRoi;

        public MaskRoiEditorViewModel() : this(new MaskRoiOptions())
        {
        }

        public MaskRoiEditorViewModel(MaskRoiOptions options)
        {
            BindOptions(options);
            AddMaskRoiCommand = new RelayCommand<object>(_ => AddMaskRoi());
            DeleteMaskRoiCommand = new RelayCommand<object>(_ => DeleteSelectedMaskRoi(), _ => SelectedMaskRoi != null);
            MoveUpCommand = new RelayCommand<object>(_ => MoveSelected(-1), _ => CanMove(-1));
            MoveDownCommand = new RelayCommand<object>(_ => MoveSelected(1), _ => CanMove(1));
        }

        public ObservableCollection<MaskRoiItemViewModel> MaskRois { get; } = new ObservableCollection<MaskRoiItemViewModel>();
        public Array ShapeTypes { get { return Enum.GetValues(typeof(MaskRoiShapeType)); } }
        public RelayCommand<object> AddMaskRoiCommand { get; private set; }
        public RelayCommand<object> DeleteMaskRoiCommand { get; private set; }
        public RelayCommand<object> MoveUpCommand { get; private set; }
        public RelayCommand<object> MoveDownCommand { get; private set; }
        public Action<MaskRoiParam> InitializeNewMaskRoi { get; set; }
        public event EventHandler MaskRoisChanged;

        public bool EnableMaskRoi { get { return options.EnableMaskRoi; } set { if (options.EnableMaskRoi == value) return; options.EnableMaskRoi = value; OnPropertyChanged(); RaiseMaskRoisChanged(); } }
        public bool ShowMaskRoi { get { return options.ShowMaskRoi; } set { if (options.ShowMaskRoi == value) return; options.ShowMaskRoi = value; OnPropertyChanged(); RaiseMaskRoisChanged(); } }
        public bool EditMaskRoi { get { return options.EditMaskRoi; } set { if (options.EditMaskRoi == value) return; options.EditMaskRoi = value; OnPropertyChanged(); RaiseMaskRoisChanged(); } }

        public MaskRoiItemViewModel SelectedMaskRoi
        {
            get { return selectedMaskRoi; }
            set
            {
                if (SetProperty(ref selectedMaskRoi, value))
                {
                    options.SelectedIndex = selectedMaskRoi == null ? -1 : MaskRois.IndexOf(selectedMaskRoi);
                    RaiseCommandStates();
                }
            }
        }

        public MaskRoiOptions Options { get { return options; } }

        public void BindOptions(MaskRoiOptions newOptions)
        {
            DetachAllItems();
            options = newOptions ?? new MaskRoiOptions();
            options.EnsureList();
            ReloadItems();
            OnPropertyChanged(string.Empty);
        }

        public MaskRoiParam AddMaskRoi()
        {
            int index = MaskRois.Count + 1;
            var roi = new MaskRoiParam
            {
                Index = index,
                Name = "屏蔽ROI-" + index,
                Enabled = true,
                Shape = MaskRoiShapeType.Rectangle1,
                Row1 = 100,
                Col1 = 100,
                Row2 = 300,
                Col2 = 400
            };
            if (InitializeNewMaskRoi != null) InitializeNewMaskRoi(roi);
            if (roi.Shape == MaskRoiShapeType.Rectangle1) roi.NormalizeFromRectangle1();
            options.MaskRois.Add(roi);
            options.NotifyMaskRoisChanged();
            var vm = new MaskRoiItemViewModel(roi);
            AttachItem(vm);
            MaskRois.Add(vm);
            SelectedMaskRoi = vm;
            RaiseMaskRoisChanged();
            return roi;
        }

        public void DeleteSelectedMaskRoi()
        {
            //if (SelectedMaskRoi == null) return;
            //options.MaskRois.Remove(SelectedMaskRoi.Model);
            //options.NotifyMaskRoisChanged();
            //DetachItem(SelectedMaskRoi);
            //SelectedMaskRoi.Release();
            //MaskRois.Remove(SelectedMaskRoi);
            //Renumber();
            //SelectedMaskRoi = MaskRois.Count > 0 ? MaskRois[Math.Min(options.SelectedIndex < 0 ? 0 : options.SelectedIndex, MaskRois.Count - 1)] : null;
            //RaiseMaskRoisChanged();

            if (SelectedMaskRoi == null) return;

            MaskRoiItemViewModel removing = SelectedMaskRoi;
            int removingIndex = MaskRois.IndexOf(removing);

            // 1) 先处理编辑器本地状态：摘除事件、从列表移除、释放
            DetachItem(removing);
            MaskRois.Remove(removing);
            removing.Release();

            // 2) 再同步数据源，并重排序号
            options.MaskRois.Remove(removing.Model);
            Renumber();

            // 3) 选下一个：优先选被删项原位置，删的是最后一项就选新的最后一项；删空则为 null
            SelectedMaskRoi = MaskRois.Count > 0
                ? MaskRois[Math.Min(removingIndex, MaskRois.Count - 1)]
                : null;

            // 4) 最后通知宿主。此时本地状态已经一致，宿主同步回调读到的是完整状态
            options.NotifyMaskRoisChanged();

            // 5) 防一手：如果宿主回调把选中项清掉了（宿主记录的 SelectedMaskRoiParam 正好是被删项时会发生），补回
            if (MaskRois.Count > 0 && SelectedMaskRoi == null)
                SelectedMaskRoi = MaskRois[Math.Min(removingIndex, MaskRois.Count - 1)];

            RaiseMaskRoisChanged();
        }

        public void MoveSelected(int offset)
        {
            if (SelectedMaskRoi == null) return;
            int oldIndex = MaskRois.IndexOf(SelectedMaskRoi);
            int newIndex = oldIndex + offset;
            if (newIndex < 0 || newIndex >= MaskRois.Count) return;
            MaskRois.Move(oldIndex, newIndex);
            options.MaskRois.RemoveAt(oldIndex);
            options.MaskRois.Insert(newIndex, SelectedMaskRoi.Model);
            options.NotifyMaskRoisChanged();
            Renumber();
            SelectedMaskRoi = MaskRois[newIndex];
            RaiseMaskRoisChanged();
        }

        public void ReloadItems()
        {
            DetachAllItems();
            MaskRois.Clear();
            if (options.MaskRois != null)
            {
                foreach (MaskRoiParam roi in options.MaskRois)
                {
                    if (roi == null) continue;
                    var item = new MaskRoiItemViewModel(roi);
                    AttachItem(item);
                    MaskRois.Add(item);
                }
            }
            Renumber();
            if (MaskRois.Count == 0)
            {
                SelectedMaskRoi = null;
            }
            else
            {
                int selected = options.SelectedIndex;
                if (selected < 0 || selected >= MaskRois.Count) selected = 0;
                SelectedMaskRoi = MaskRois[selected];
            }
        }

        private bool CanMove(int offset)
        {
            if (SelectedMaskRoi == null) return false;
            int index = MaskRois.IndexOf(SelectedMaskRoi);
            int target = index + offset;
            return target >= 0 && target < MaskRois.Count;
        }

        private void Renumber()
        {
            for (int i = 0; i < MaskRois.Count; i++)
            {
                MaskRois[i].Index = i + 1;
                if (string.IsNullOrWhiteSpace(MaskRois[i].Name) || MaskRois[i].Name.StartsWith("屏蔽ROI-"))
                    MaskRois[i].Name = "屏蔽ROI-" + (i + 1);
                MaskRois[i].RefreshAll();
            }
        }

        private void RaiseCommandStates()
        {
            if (DeleteMaskRoiCommand != null) DeleteMaskRoiCommand.RaiseCanExecuteChanged();
            if (MoveUpCommand != null) MoveUpCommand.RaiseCanExecuteChanged();
            if (MoveDownCommand != null) MoveDownCommand.RaiseCanExecuteChanged();
        }

        private void RaiseMaskRoisChanged()
        {
            MaskRoisChanged?.Invoke(this, EventArgs.Empty);
        }

        private void AttachItem(MaskRoiItemViewModel item)
        {
            if (item != null) item.PropertyChanged += OnItemPropertyChanged;
        }

        private void DetachItem(MaskRoiItemViewModel item)
        {
            if (item != null) item.PropertyChanged -= OnItemPropertyChanged;
        }

        private void DetachAllItems()
        {
            foreach (MaskRoiItemViewModel item in MaskRois)
            {
                DetachItem(item);
                item.Release();
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 单个 ROI 的启用、形状或坐标变化都会影响外部显示和算法参数，
            // 因此统一通过公共事件通知宿主刷新，而不是让宿主订阅内部 Item VM。
            RaiseMaskRoisChanged();
        }
    }
}
