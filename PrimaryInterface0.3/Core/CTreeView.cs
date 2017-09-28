using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimaryInterface0._3.Core
{
    /// <summary>
    /// 依照步驟 1a 或 1b 執行，然後執行步驟 2，以便在 XAML 檔中使用此自訂控制項。
    ///
    /// 步驟 1a) 於存在目前專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    /// 要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:PrimaryInterface0._3.Core"
    ///
    ///
    /// 步驟 1b) 於存在其他專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    /// 要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:PrimaryInterface0._3.Core;assembly=PrimaryInterface0._3.Core"
    ///
    /// 您還必須將 XAML 檔所在專案的專案參考加入
    /// 此專案並重建，以免發生編譯錯誤: 
    ///
    ///     在 [方案總管] 中以滑鼠右鍵按一下目標專案，並按一下
    ///     [加入參考]->[專案]->[瀏覽並選取此專案]
    ///
    ///
    /// 步驟 2)
    /// 開始使用 XAML 檔案中的控制項。
    ///
    ///     <MyNamespace:CTreeView/>
    ///
    /// </summary>
    public class CTreeViewItem : TreeViewItem
    {
        static CTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CTreeViewItem), new FrameworkPropertyMetadata(typeof(CTreeViewItem)));

        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(_Direction), typeof(CTreeViewItem));

        public _Direction Direction
        {
            get { return (_Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        private int CTreeViewItemCount { get; set; }
        public bool IsFirstLevel { get; set; }
        public delegate void NumChangedHandler1(int num);
        public event NumChangedHandler1 RowNumChanged;
        public event NumChangedHandler1 SelectedRowIndex;
        public delegate void NumChangedHandler2(int Index, int Num, bool IsFirstLevel);
        public event NumChangedHandler2 ExpandedRowIndex;

        private int SelfIndex = 0;
        private bool DelayExpanded = false;

        private void CTreeViewItem_SelectedRowIndex(int Index)
        {
            if (SelectedRowIndex == null)
                Debug.WriteLine("SelectedRowIndex event is not subscribed");
            else
                SelectedRowIndex?.Invoke(Index + SelfIndex);
        }


        private void CTreeViewItem_ExpandedRowIndex(int Index, int num, bool Level)
        {
            if (SelectedRowIndex == null)
                Debug.WriteLine("ExpandedRowIndex event is not subscribed");
            else
                ExpandedRowIndex?.Invoke(Index + SelfIndex, num, Level);
        }
        /// <summary>
        /// Find the CTreeViewItem whose expanded is true at the same level
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private CTreeViewItem FindExpandedItemAtTheSameLevel(CTreeViewItem Item)
        {
            FrameworkElement ItemParent = LogicalTreeHelper.GetParent(Item) as FrameworkElement;
            foreach (var Element in LogicalTreeHelper.GetChildren(ItemParent))
            {
                if (Element is CTreeViewItem)
                {
                    CTreeViewItem Temp = Element as CTreeViewItem;
                    if (Temp.HasItems && Temp.IsExpanded && !Item.Equals(Temp))
                    {
                        return Temp;
                    }
                    if (Item.Equals(Temp))
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Find the CTreeViewItem whose expanded is true in it's children
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private CTreeViewItem FindExpandedItemInChildren(CTreeViewItem Item)
        {
            //if (!(Item is CTreeViewItem))
            //    return null;
            //CTreeViewItem ItemParent = Item as CTreeViewItem;
            foreach (var Element in LogicalTreeHelper.GetChildren(Item))
            {
                CTreeViewItem e = Element as CTreeViewItem;
                if (e != null && e.IsExpanded)
                    return e;
            }
            return null;
        }

        private int CalRow(object FristItem)
        {
            if (!(FristItem is CTreeViewItem))
                return 0;
            CTreeViewItem tempItem = FindExpandedItemAtTheSameLevel((CTreeViewItem)FristItem);
            int tempNum = 0;
            while (tempItem != null)
            {
                tempNum += tempItem.CTreeViewItemCount;
                tempItem = FindExpandedItemInChildren(tempItem);
            }
            return tempNum;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            int tempNum = 0;
            tempNum += CalRow(this);
            if (SelfIndex == 0 && Parent != null)
            {
                foreach (var Element in LogicalTreeHelper.GetChildren(Parent))
                {
                    if (Element is CTreeViewItem)
                        SelfIndex++;
                    if (Element.Equals(this))
                        break;
                }
            }
            if (SelectedRowIndex == null)
                Debug.WriteLine("SelectedRowIndex original event is not subscribed");
            else
                SelectedRowIndex?.Invoke(SelfIndex + tempNum);
            base.OnSelected(e);
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            if (this.DelayExpanded)
                return;
            foreach (var Element in LogicalTreeHelper.GetChildren(this))
            {
                if (Element is TreeViewItem && ((CTreeViewItem)Element).IsExpanded)
                    ((TreeViewItem)Element).IsExpanded = false;
            }

            int tempNum = 0;
            tempNum += CalRow(this);
            if (ExpandedRowIndex == null)
                Debug.WriteLine("ExpandedRowIndex event is not subscribed");
            else
                ExpandedRowIndex(SelfIndex + tempNum, -CTreeViewItemCount, IsFirstLevel);
            base.OnCollapsed(e);

            if (RowNumChanged == null)
                Debug.WriteLine("RowNumChanged event is not subsribed");
            else
                RowNumChanged?.Invoke(-CTreeViewItemCount);
        }

        protected override void OnExpanded(RoutedEventArgs e)
        {
            if (this.DelayExpanded)
                return;
            this.DelayExpanded = true;
            this.IsExpanded = false;
            if (LogicalTreeHelper.GetParent(this) is CTreeView)
            {
                CTreeView TreeParent = LogicalTreeHelper.GetParent(this) as CTreeView;
                foreach (var Element in LogicalTreeHelper.GetChildren(TreeParent))
                {
                    if (Element is CTreeViewItem && !Element.Equals(this))
                    {
                        CTreeViewItem tempItem = Element as CTreeViewItem;
                        if (tempItem.IsExpanded == true)
                        {
                            tempItem.IsExpanded = false;
                            break;
                        }
                    }
                }
            }
            else if (LogicalTreeHelper.GetParent(this) is CTreeViewItem)
            {
                CTreeViewItem TreeParent = LogicalTreeHelper.GetParent(this) as CTreeViewItem;
                foreach (var Element in LogicalTreeHelper.GetChildren(TreeParent))
                {
                    if (Element is CTreeViewItem && !Element.Equals(this))
                    {
                        CTreeViewItem tempItem = Element as CTreeViewItem;
                        if (tempItem.IsExpanded == true)
                        {
                            tempItem.IsExpanded = false;
                            break;
                        }
                    }
                }
            }
            this.IsExpanded = true;
            this.DelayExpanded = false;

            if (SelfIndex == 0 && Parent != null)
            {
                foreach (var Element in LogicalTreeHelper.GetChildren(Parent))
                {
                    if (Element is CTreeViewItem)
                        SelfIndex++;
                    if (Element.Equals(this))
                        break;
                }
            }

            int tempNum = 0;
            tempNum += CalRow(this);
            if (ExpandedRowIndex == null)
                Debug.WriteLine("ExpandedRowIndex event is not subscribed");
            else
                ExpandedRowIndex?.Invoke(SelfIndex + tempNum, CTreeViewItemCount, IsFirstLevel);
            base.OnExpanded(e);

            if (RowNumChanged == null)
                Debug.WriteLine("RowNumChanged event is not subsribed");
            else
                RowNumChanged?.Invoke(CTreeViewItemCount);
        }

        protected override void OnInitialized(EventArgs e)
        {
            IsFirstLevel = false;
            Style = (Style)FindResource("CTreeViewItemStyle1");
            if (Parent != null)
            {
                foreach (var Element in LogicalTreeHelper.GetChildren(Parent))
                {
                    if (Element is CTreeViewItem)
                        SelfIndex++;
                }
            }
            foreach (var Element in LogicalTreeHelper.GetChildren(this))
            {
                if (Element is CTreeViewItem)
                {
                    CTreeViewItemCount++;
                    ((CTreeViewItem)Element).SelectedRowIndex += CTreeViewItem_SelectedRowIndex;
                    ((CTreeViewItem)Element).ExpandedRowIndex += CTreeViewItem_ExpandedRowIndex;
                }
            }
            base.OnInitialized(e);
        }

        protected override void AddChild(object value)
        {
            if (value is CTreeViewItem)
            {
                CTreeViewItemCount++;
                CTreeViewItem temp = value as CTreeViewItem;
                temp.SelectedRowIndex += CTreeViewItem_SelectedRowIndex;
                temp.ExpandedRowIndex += CTreeViewItem_ExpandedRowIndex;
                if (temp.SelfIndex == 0)
                {
                    int Index = 0;
                    foreach (var e in LogicalTreeHelper.GetChildren(this))
                    {
                        if (e is CTreeViewItem)
                            Index++;
                        if (temp.Equals(e))
                            temp.SelfIndex = Index;
                    }
                }
            }
            base.AddChild(value);
        }

        public void AddCTreeViewItem(object value)
        {
            AddChild(value);
        }
    }

    public class CTreeView : TreeView
    {
        static CTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CTreeView), new FrameworkPropertyMetadata(typeof(CTreeView)));
        }

        private int CurrentSelectedRow = 0;
        private List<CTreeViewItem> FristLevelItem = new List<CTreeViewItem>();
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(_Direction), typeof(CTreeView), new PropertyMetadata(_Direction.Left));
        public _Direction Direction
        {
            get { return (_Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty TotalRowNumProperty = DependencyProperty.Register("TotalRowNum", typeof(int), typeof(CTreeView));
        public int TotalRowNum
        {
            get { return (int)GetValue(TotalRowNumProperty); }
            private set { SetValue(TotalRowNumProperty, value); }
        }

        public static readonly DependencyPropertyKey SelectRowIndexPropertyKey = DependencyProperty.RegisterReadOnly("SelectRowIndex", typeof(int), typeof(CTreeView), new PropertyMetadata(0));
        public static readonly DependencyProperty SelectRowIndexProperty = SelectRowIndexPropertyKey.DependencyProperty;
        public int SelectRowIndex
        {
            get { return (int)GetValue(SelectRowIndexPropertyKey.DependencyProperty); }
        }

        public static readonly DependencyProperty AddNewRanksProperty = DependencyProperty.Register("AddNewRanks", typeof(NewAddRanks), typeof(CTreeView));
        private NewAddRanks AddNewRanks
        {
            get { return (NewAddRanks)GetValue(AddNewRanksProperty); }
            set { SetValue(AddNewRanksProperty, value); }
        }

        public static readonly DependencyProperty ExpandedIndexProperty = DependencyProperty.Register("ExpandedIndex", typeof(ExpandedIndex), typeof(CTreeView), new PropertyMetadata(new ExpandedIndex() { Expanded = false, Index = -1 }, OnExpandedIndexChanged));
        /// <summary>
        /// It is to be called when the property changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        //private static object ExpandedIndexCoerce(DependencyObject d, object baseValue)
        //{
        //    Debug.WriteLine("baseValue is "+(int)baseValue);
        //    return baseValue;
        //}

        private static void OnExpandedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CTreeView tempTreeView = d as CTreeView;
            ExpandedIndex NewExpandedIndex = (ExpandedIndex)e.NewValue;
            if (NewExpandedIndex.Index == -1 && NewExpandedIndex.Index >= tempTreeView.TotalRowNum)
                return;
            Debug.WriteLine("index is " + NewExpandedIndex.Index + " expanded is " + NewExpandedIndex.Expanded);
            tempTreeView.FristLevelItem[NewExpandedIndex.Index].IsExpanded = NewExpandedIndex.Expanded;
        }

        public int ExpandedIndex
        {
            get { return (int)GetValue(ExpandedIndexProperty); }
            set { SetValue(ExpandedIndexProperty, value); }
        }

        private void InitialAddEvent(DependencyObject a)
        {
            foreach (var Element in LogicalTreeHelper.GetChildren(a))
            {
                if (Element is CTreeViewItem)
                {
                    CTreeViewItem temp = Element as CTreeViewItem;
                    if (a is TreeView)
                    {
                        TotalRowNum += 1;
                        temp.IsFirstLevel = true;
                        FristLevelItem.Add(temp);
                        temp.SelectedRowIndex += Temp_SelectedRowIndex;
                        temp.ExpandedRowIndex += Temp_ExpandedRowIndex;
                    }
                    if (temp.HasItems)
                    {
                        temp.RowNumChanged += Temp_RowNumChanged;
                        InitialAddEvent(temp);
                    }
                    else
                        continue;
                }
            }
        }

        private void Temp_ExpandedRowIndex(int Index, int num, bool Level)
        {
            AddNewRanks = new NewAddRanks() { RanksCount = num, RanksIndex = Index, IsFirstLevel = Level };
            if (Index < CurrentSelectedRow)
            {
                CurrentSelectedRow += num;
                this.SetValue(SelectRowIndexPropertyKey, CurrentSelectedRow);
                Debug.WriteLine("in top index is " + Index + "num is " + num);
            }
            Debug.WriteLine("New ranks index is " + Index + "count is " + num + Level);
        }

        private void Temp_SelectedRowIndex(int Index)
        {
            Debug.WriteLine("select index is " + Index);
            this.SetValue(SelectRowIndexPropertyKey, Index);
            CurrentSelectedRow = Index;
        }

        private void Temp_RowNumChanged(int Delta)
        {
            //TotalRowNum += Delta;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitialAddEvent(this);
        }

        protected override void AddChild(object value)
        {
            Debug.WriteLine("Add Child TreeView");
            if (value is CTreeViewItem)
            {
                AddChildEvent((CTreeViewItem)value);
            }
            base.AddChild(value);
        }

        public void AddCTreeViewItem(object value)
        {
            AddChild(value);
        }

        private void AddChildEvent(object value)
        {
            if (!(value is CTreeViewItem))
                return;
            CTreeViewItem temp = value as CTreeViewItem;
            TotalRowNum += 1;
            temp.IsFirstLevel = true;
            FristLevelItem.Add(temp);
            temp.SelectedRowIndex += Temp_SelectedRowIndex;
            temp.ExpandedRowIndex += Temp_ExpandedRowIndex;
            SubscribeRowNumChangeEvent(temp);
        }

        private void SubscribeRowNumChangeEvent(CTreeViewItem e)
        {
            if (e.HasItems)
            {
                e.RowNumChanged += Temp_RowNumChanged;
                InitialAddEvent(e);
            }
        }
    }

    public enum _Direction
    {
        Left,
        Top
    }
}
