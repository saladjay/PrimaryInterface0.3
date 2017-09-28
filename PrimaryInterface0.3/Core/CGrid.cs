using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    ///     <MyNamespace:CGrid/>
    ///
    /// </summary>
    public class CGrid : Grid
    {
        static CGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CGrid), new FrameworkPropertyMetadata(typeof(CGrid)));
        }

        private CToggleBtn[,] CToggleBtnArray = null;
        private CLabel[,] CLabelArray = null;
        private int TempRow = -1;
        private int TempColumn = -1;
        private int PreTempRow = -1;
        private int PreTempColumn = -1;

        public static readonly DependencyProperty PrimaryRowProperty = DependencyProperty.Register("PrimaryRow", typeof(int),
            typeof(CGrid), new PropertyMetadata(0, OnPrimaryRowChanged));

        private static void OnPrimaryRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CGrid Temp = d as CGrid;
            if (Temp == null)
            {
                Debug.WriteLine("This DependencyObject is null");
                return;
            }
            int Delta = (int)e.NewValue - (int)e.OldValue;
            for (int i = 0; i < Delta; i++)
            {
                RowDefinition TempRow = new RowDefinition();
                TempRow.Height = new GridLength(20);
                Temp.RowDefinitions.Add(TempRow);
            }
            Temp.InitialAddBtn();
        }

        public int PrimaryRow
        {
            get { return (int)GetValue(PrimaryRowProperty); }
            set { SetValue(PrimaryRowProperty, value); }
        }

        public static readonly DependencyProperty PrimaryColumnProperty = DependencyProperty.Register("PrimaryColumn",
            typeof(int), typeof(CGrid), new PropertyMetadata(0, OnPrimaryColumnChanged));

        private static void OnPrimaryColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CGrid Temp = d as CGrid;
            if (Temp == null)
            {
                Debug.WriteLine("This DependencyObject is null");
                return;
            }
            int Delta = (int)e.NewValue - (int)e.OldValue;
            for (int i = 0; i < Delta; i++)
            {
                ColumnDefinition TempColumn = new ColumnDefinition();
                TempColumn.Width = new GridLength(20);
                Temp.ColumnDefinitions.Add(TempColumn);
            }
            Temp.InitialAddBtn();
        }

        public int PrimaryColumn
        {
            get { return (int)GetValue(PrimaryColumnProperty); }
            set { SetValue(PrimaryColumnProperty, value); }
        }

        public static readonly DependencyProperty NewRowProperty = DependencyProperty.Register("NewRow", typeof(NewAddRanks),
            typeof(CGrid), new PropertyMetadata(new NewAddRanks() { RanksCount = 0, RanksIndex = 0, IsFirstLevel = false }, OnNewRowChanged));

        private static void OnNewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NewAddRanks RowChanged = (NewAddRanks)e.NewValue;
            CGrid Temp = d as CGrid;
            for (int i = 0; i < RowChanged.RanksCount; i++)
            {
                RowDefinition TempRow = new RowDefinition();
                TempRow.Height = new GridLength(20);
                TempRow.Tag = true;
                TempRow.MinHeight = 20;
                Temp.RowDefinitions.Insert(RowChanged.RanksIndex, TempRow);
            }
            Temp.MoveBtn(RowChanged.RanksIndex, RowChanged.RanksCount, true);
            if (RowChanged.RanksCount < 0)
            {
                for (int DeleteRow = RowChanged.RanksCount; DeleteRow < 0; DeleteRow++)
                {
                    Temp.RowDefinitions.RemoveAt(RowChanged.RanksIndex);
                }
            }
            Debug.WriteLine("the amount of rows is" + Temp.RowDefinitions.Count);
            Temp.AddNewArea();
            if (RowChanged.IsFirstLevel)
            {
                Temp.TempRow = RowChanged.RanksIndex - 1;
                if (Temp.TempColumn != -1 && Temp.TempRow != -1)
                {
                    if (RowChanged.RanksCount > 0)
                    {
                        if (Temp.PreTempRow != -1 && Temp.PreTempColumn != -1)
                        {
                            Temp.CToggleBtnArray[Temp.PreTempRow, Temp.PreTempColumn].RowExpanded = false;
                            Temp.CToggleBtnArray[Temp.PreTempRow, Temp.PreTempColumn].ColumnExpanded = false;
                        }
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].RowExpanded = true;
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].ColumnExpanded = true;
                    }
                    else
                    {
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].RowExpanded = false;
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].ColumnExpanded = false;

                    }
                    Temp.PreTempColumn = Temp.TempColumn;
                    Temp.PreTempRow = Temp.TempRow;
                }
                if (Temp.RowDefinitions.Count == Temp.PrimaryRow && Temp.ColumnDefinitions.Count == Temp.PrimaryColumn)
                {
                    Temp.TempColumn = -1;
                    Temp.TempRow = -1;
                    Temp.PreTempRow = -1;
                    Temp.PreTempColumn = -1;
                }
            }
        }

        public NewAddRanks NewRow
        {
            get { return (NewAddRanks)GetValue(NewRowProperty); }
            set { SetValue(NewRowProperty, value); }
        }

        public static readonly DependencyProperty NewColumnProperty = DependencyProperty.Register("NewColumn", typeof(NewAddRanks),
            typeof(CGrid), new PropertyMetadata(new NewAddRanks() { RanksCount = 0, RanksIndex = 0, IsFirstLevel = false }, OnNewColumnChanged));

        private static void OnNewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NewAddRanks ColumnChanged = (NewAddRanks)e.NewValue;
            CGrid Temp = d as CGrid;
            if (ColumnChanged.RanksCount > 0)
            {
                for (int i = 0; i < ColumnChanged.RanksCount; i++)
                {
                    ColumnDefinition TempColumn = new ColumnDefinition();
                    TempColumn.Tag = true;
                    TempColumn.Width = new GridLength(20);
                    TempColumn.MinWidth = 20;
                    Temp.ColumnDefinitions.Insert(ColumnChanged.RanksIndex, TempColumn);
                }
            }
            Temp.MoveBtn(ColumnChanged.RanksIndex, ColumnChanged.RanksCount, false);
            if (ColumnChanged.RanksCount < 0)
            {
                for (int DeleteRow = ColumnChanged.RanksCount; DeleteRow < 0; DeleteRow++)
                {
                    Temp.ColumnDefinitions.RemoveAt(ColumnChanged.RanksIndex);
                }
            }

            Debug.WriteLine("the amount of column is " + Temp.ColumnDefinitions.Count);
            Temp.AddNewArea();

            if (ColumnChanged.IsFirstLevel)
            {
                Temp.TempColumn = ColumnChanged.RanksIndex - 1;
                if (Temp.TempColumn != -1 && Temp.TempRow != -1)
                {
                    if (ColumnChanged.RanksCount > 0)
                    {
                        if (Temp.PreTempRow != -1 && Temp.PreTempColumn != -1)
                        {
                            Temp.CToggleBtnArray[Temp.PreTempRow, Temp.PreTempColumn].RowExpanded = false;
                            Temp.CToggleBtnArray[Temp.PreTempRow, Temp.PreTempColumn].ColumnExpanded = false;
                        }
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].RowExpanded = true;
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].ColumnExpanded = true;
                    }
                    else
                    {
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].RowExpanded = false;
                        Temp.CToggleBtnArray[Temp.TempRow, Temp.TempColumn].ColumnExpanded = false;
                    }
                    Temp.PreTempColumn = Temp.TempColumn;
                    Temp.PreTempRow = Temp.TempRow;
                }
                if (Temp.RowDefinitions.Count == Temp.PrimaryRow && Temp.ColumnDefinitions.Count == Temp.PrimaryColumn)
                {
                    Temp.TempColumn = -1;
                    Temp.TempRow = -1;
                    Temp.PreTempRow = -1;
                    Temp.PreTempColumn = -1;
                }
            }
        }

        public NewAddRanks NewColumn
        {
            get { return (NewAddRanks)GetValue(NewColumnProperty); }
            set { SetValue(NewColumnProperty, value); }
        }

        private void AddNewArea()
        {
            int PrimaryChildrenCount = this.PrimaryColumn * this.PrimaryRow;
            while (this.Children.Count > PrimaryChildrenCount)
            {
                this.Children.RemoveAt(PrimaryChildrenCount);
            }

            int StartColumn = 0;
            int NewAddColumnCount = this.ColumnDefinitions.Count - this.PrimaryColumn;
            for (int i = 0; i < this.ColumnDefinitions.Count; i++)
            {
                if (this.ColumnDefinitions[i].Tag != null)
                {
                    StartColumn = i;
                    break;
                }
            }

            int StartRow = 0;
            int NewAddRowCount = this.RowDefinitions.Count - this.PrimaryRow;
            for (int i = 0; i < this.RowDefinitions.Count; i++)
            {
                if (this.RowDefinitions[i].Tag != null)
                {
                    StartRow = i;
                    break;
                }
            }

            this.CLabelArray = null;
            this.CLabelArray = new CLabel[this.RowDefinitions.Count, this.ColumnDefinitions.Count];
            for (int j = 0; j < this.ColumnDefinitions.Count; j++)
            {
                for (int i = 0; i < this.RowDefinitions.Count; i++)
                {
                    if ((i >= StartRow && i < StartRow + NewAddRowCount) || (j >= StartColumn && j < StartColumn + NewAddColumnCount))
                    {
                        if (this.CLabelArray[i, j] != null)
                            continue;
                        CLabel NewLabel = new CLabel();
                        this.CLabelArray[i, j] = NewLabel;
                        this.Children.Add(NewLabel);
                        Grid.SetRow(NewLabel, i);
                        Grid.SetColumn(NewLabel, j);
                        if ((i >= StartRow && i < StartRow + NewAddRowCount) && (j >= StartColumn && j < StartColumn + NewAddColumnCount))
                            NewLabel.IsCommon = true;
                    }
                }
            }
            HighLightSelectedBox(-1, true, true);
        }

        public static readonly DependencyProperty SelectedRowProperty = DependencyProperty.Register("SelectedRow", typeof(int),
            typeof(CGrid), new PropertyMetadata(0, OnSelectedRowChanged));

        private static void OnSelectedRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int SelectedRowIndex = (int)e.NewValue - 1;
            int PreRowIndex = (int)e.OldValue - 1;
            CGrid Temp = d as CGrid;
            Temp.HighLightSelectedBox(SelectedRowIndex, true);
        }

        public int SelectedRow
        {
            get { return (int)GetValue(SelectedRowProperty); }
            set { SetValue(SelectedRowProperty, value); }
        }

        public static readonly DependencyProperty SelectedColumnProperty = DependencyProperty.Register("SelectedColumn",
            typeof(int), typeof(CGrid), new PropertyMetadata(0, OnSelectedColumnChanged));

        private static void OnSelectedColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int SelectedColumnIndex = (int)e.NewValue - 1;
            int PreColumnIndex = (int)e.OldValue - 1;
            CGrid Temp = d as CGrid;
            Temp.HighLightSelectedBox(SelectedColumnIndex, false);
        }

        public int SelectedColumn
        {
            get { return (int)GetValue(SelectedColumnProperty); }
            set { SetValue(SelectedColumnProperty, value); }
        }

        private int HighLightRow = -1;
        private int HighLightColumn = -1;
        private void HighLightSelectedBox(int Index, bool IsRow, bool ReDo = false)
        {
            if (CLabelArray == null)
                return;
            if (ReDo)
            {
                for (int i = 0; i < this.CLabelArray.GetLength(0); i++)
                {
                    for (int j = 0; j < this.CLabelArray.GetLength(1); j++)
                    {
                        if (this.CLabelArray[i, j] == null)
                            continue;
                        if (i == HighLightRow || j == HighLightColumn)
                            this.CLabelArray[i, j].IsSelected = true;
                        else
                            this.CLabelArray[i, j].IsSelected = false;
                    }
                }
            }
            else
            {
                if (IsRow)
                    HighLightRow = Index;
                else
                    HighLightColumn = Index;
                for (int i = 0; i < this.CLabelArray.GetLength(0); i++)
                {
                    for (int j = 0; j < this.CLabelArray.GetLength(1); j++)
                    {
                        if (this.CLabelArray[i, j] == null)
                            continue;
                        if (i == HighLightRow || j == HighLightColumn)
                            this.CLabelArray[i, j].IsSelected = true;
                        else
                            this.CLabelArray[i, j].IsSelected = false;
                    }
                }
            }
        }

        public static readonly DependencyProperty ExpandedRowProperty = DependencyProperty.Register("ExpandedRow", typeof(ExpandedIndex),
            typeof(CGrid), new PropertyMetadata(new ExpandedIndex() { Expanded = false, Index = -1, }));
        public ExpandedIndex ExpandedRow
        {
            get { return (ExpandedIndex)GetValue(ExpandedRowProperty); }
            set { SetValue(ExpandedRowProperty, value); }
        }

        public static readonly DependencyProperty ExpandedColumnProperty = DependencyProperty.Register("ExpandedColumn", typeof(ExpandedIndex),
            typeof(CGrid), new PropertyMetadata(new ExpandedIndex() { Expanded = false, Index = -1 }));
        public ExpandedIndex ExpandedColumn
        {
            get { return (ExpandedIndex)GetValue(ExpandedColumnProperty); }
            set { SetValue(ExpandedColumnProperty, value); }
        }

        private void FindSomething()
        {
            RowDefinition temp = this.RowDefinitions.First();
        }

        private void InitialAddBtn()
        {
            CToggleBtnArray = null;
            while (Children.Count > 0)
            {
                Children.RemoveAt(0);
            }
            int RowMax = this.RowDefinitions.Count;
            int ColumnMax = this.ColumnDefinitions.Count;
            CToggleBtnArray = new CToggleBtn[RowMax, ColumnMax];
            for (int R = 0; R < RowMax; R++)
            {
                for (int C = 0; C < ColumnMax; C++)
                {
                    CToggleBtn TempBtn = new CToggleBtn()
                    {
                        PositionRow = R,
                        PositionColumn = C,
                        RowIndex = R,
                        ColumnIndex = C,
                    };
                    TempBtn.CheckedAndExpanded += InitialBtn_CheckedAndExpanded;
                    CToggleBtnArray[R, C] = TempBtn;
                    this.Children.Add(TempBtn);
                    Grid.SetRow(TempBtn, R);
                    Grid.SetColumn(TempBtn, C);
                }
            }
        }

        private void InitialBtn_CheckedAndExpanded(int Row, int Column, bool Sign)
        {
            this.ExpandedColumn = new ExpandedIndex() { Expanded = Sign, Index = Column };
            this.ExpandedRow = new ExpandedIndex() { Expanded = Sign, Index = Row };
            Debug.WriteLine("Expanded Row is " + Row + " Column " + Column);
        }

        private void MoveBtn(int Index, int count, bool Row)
        {
            if (Row)
            {
                int StartRow = -1;
                int MoveRow = 0;
                for (int i = 0; i < PrimaryRow; i++)
                {
                    if (CToggleBtnArray[i, 0].PositionRow >= Index)
                    {
                        StartRow = i;
                        MoveRow = CToggleBtnArray[i, 0].PositionRow;
                        break;
                    }
                }
                if (StartRow == -1)
                {
                    Debug.WriteLine("wrong startRow");
                    return;
                }

                for (int i = StartRow; i < PrimaryRow; i++)
                {
                    for (int j = 0; j < PrimaryColumn; j++)
                    {
                        BtnSetRow(CToggleBtnArray[i, j], MoveRow + count);
                    }
                    MoveRow++;
                }
            }
            else
            {
                int StartColumn = -1;
                int MoveColumn = 0;
                for (int i = 0; i < PrimaryColumn; i++)
                {
                    if (CToggleBtnArray[0, i].PositionColumn >= Index)
                    {
                        StartColumn = i;
                        MoveColumn = CToggleBtnArray[0, i].PositionColumn;
                        break;
                    }
                }
                if (StartColumn == -1)
                {
                    Debug.WriteLine("wrong startColumn");
                    return;
                }
                for (int i = StartColumn; i < PrimaryColumn; i++)
                {
                    for (int j = 0; j < PrimaryRow; j++)
                    {
                        BtnSetColumn(CToggleBtnArray[j, i], MoveColumn + count);
                    }
                    MoveColumn++;
                }
            }
        }

        private void BtnSetRow(CToggleBtn Btn, int Index)
        {
            Btn.PositionRow = Index;
            Grid.SetRow(Btn, Index);
        }

        private void BtnSetColumn(CToggleBtn Btn, int Index)
        {
            Btn.PositionColumn = Index;
            Grid.SetColumn(Btn, Index);
        }

        protected override void OnInitialized(EventArgs e)
        {
            InitialAddBtn();
            base.OnInitialized(e);
        }
    }



    public class CToggleBtn : ToggleButton
    {
        private static CToggleBtn PreCheckedBtn = null;
        public delegate void CheckedAndExpandedHandler(int Row, int Column, bool Sign);
        public event CheckedAndExpandedHandler CheckedAndExpanded;

        public static readonly DependencyProperty ChangedIconProperty = DependencyProperty.Register("ChangedIcon", typeof(bool), typeof(CToggleBtn), new PropertyMetadata(false, OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CToggleBtn temp = d as CToggleBtn;
        }

        public bool ChangedIcon
        {
            get { return (bool)GetValue(ChangedIconProperty); }
            set { SetValue(ChangedIconProperty, value); }
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if (PreCheckedBtn != null && !PreCheckedBtn.Equals(this))
                PreCheckedBtn.IsChecked = false;
            PreCheckedBtn = this;
            RowExpanded = ColumnExpanded = true;
            if (CheckedAndExpanded == null)
                Debug.WriteLine("CheckedAndExpanded event is not subscribed");
            else
                CheckedAndExpanded?.Invoke(RowIndex, ColumnIndex, true);
            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            RowExpanded = false;
            ColumnExpanded = false;
            RowExpanded = ColumnExpanded = false;
            if (CheckedAndExpanded == null)
                Debug.WriteLine("CheckedAndExpanded event is not subscribed");
            else
                CheckedAndExpanded?.Invoke(RowIndex, ColumnIndex, false);
            base.OnUnchecked(e);
        }

        public int PositionRow { get; set; }
        public int PositionColumn { get; set; }

        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        private bool rowexpanded;
        public bool RowExpanded
        {
            get { return rowexpanded; }
            set
            {
                rowexpanded = value;
                if (rowexpanded && columnexpanded)
                    ChangedIcon = true;
                else
                    ChangedIcon = false;
            }
        }
        private bool columnexpanded;
        public bool ColumnExpanded
        {
            get { return columnexpanded; }
            set
            {
                columnexpanded = value;
                if (rowexpanded && columnexpanded)
                    ChangedIcon = true;
                else
                    ChangedIcon = false;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            this.Style = (Style)FindResource("CBtnStyle1");
            base.OnInitialized(e);
        }
    }

    public class CLabel : Control
    {
        private static int id;
        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(CLabel));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(CLabel),new PropertyMetadata(false,OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine("id is " + ((CLabel)d).ID + "isSelected" + ((CLabel)d).IsSelected);     
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsCommonProperty = DependencyProperty.RegisterAttached("IsCommon", typeof(bool), typeof(CLabel));
        public bool IsCommon
        {
            get { return (bool)GetValue(IsCommonProperty); }
            set { SetValue(IsCommonProperty, value); }
        }

        public int PositionRow { get; set; }
        public int PositionColumn { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            ID = id++;
            if (Parent != null)
                Debug.WriteLine("id is " + ID + " parent" + Parent.GetType().ToString());
            else
                Debug.WriteLine("id is " + ID);
            Style = (Style)FindResource("ExpandedLabelStyle");
            base.OnInitialized(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //IsSelected = !IsSelected;
            //IsCommon = !IsCommon;
            if (Parent != null)
                Debug.WriteLine("id is " + ID + " parent" + Parent.GetType().ToString());
            else
                Debug.WriteLine("id is " + ID);
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //    IsSelected = true;
            base.OnMouseEnter(e);
        }
    }

    public struct NewAddRanks
    {
        public int RanksCount;
        public int RanksIndex;
        public bool IsFirstLevel;
    }

    public struct ExpandedIndex
    {
        public bool Expanded;
        public int Index;
    }

}
