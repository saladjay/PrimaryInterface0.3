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
using PrimaryInterface0._3.Core;
namespace PrimaryInterface0._3
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CLabel> CLabelList = new List<CLabel>();
        public MainWindow()
        {
            InitializeComponent();
            //Window1 win1 = new Window1();
            //win1.Show();
            //while (WrapPanel.Children.Count < 80)
            //    WrapPanel.Children.Add(new CLabel());
            ZoneInfo first = new ZoneInfo(6);
            int[] abc = new int[5] { 0,1,2,3,4};//every row must be different to the each row of the rest
            ZoneInfo[] abcd = new ZoneInfo[5]
            {
                new ZoneInfo() {ZoneName="01" },
                new ZoneInfo() {ZoneName="02" },
                new ZoneInfo() {ZoneName="03" },
                new ZoneInfo() {ZoneName="04" },
                new ZoneInfo() {ZoneName="05" }
            };
            InitialDataGrid(first, testdatagrid);
            testdatagrid.ItemsSource = abcd;
            testdatagrid.AutoGenerateColumns = false;
            testdatagrid.SelectedCellsChanged += Testdatagrid_SelectedCellsChanged;
            testdatagrid.SelectionChanged += Testdatagrid_SelectionChanged;
            testdatagrid.BeginningEdit += Testdatagrid_BeginningEdit;
        }

        private void Testdatagrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void Testdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("item is" + e.AddedItems.Count);
        }


        private void Testdatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var cellinfo in e.AddedCells)
            {
                var cellContent = cellinfo.Column.GetCellContent(cellinfo.Item);
                if (cellContent == null)
                    continue;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(cellContent); i++)
                {
                    CLabel tempCLabel = VisualTreeHelper.GetChild(cellContent, i) as CLabel;
                    if (tempCLabel == null)
                        continue;
                    if (CLabelList.Contains(tempCLabel))
                        continue;
                    CLabelList.Add(tempCLabel);
                    tempCLabel.IsCommon = true;
                    Debug.WriteLine("-----------------------add id is " + tempCLabel.ID);
                }
            }
            foreach (var cellinfo in e.RemovedCells)
            {
                var cellContent = cellinfo.Column.GetCellContent(cellinfo.Item);
                if (cellContent == null)
                    continue;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(cellContent); i++)
                {
                    CLabel tempCLabel = VisualTreeHelper.GetChild(cellContent, i) as CLabel;
                    if (tempCLabel == null)
                        continue;
                    if (CLabelList.Contains(tempCLabel))
                        CLabelList.Remove(tempCLabel);
                    tempCLabel.IsCommon = false;
                    Debug.WriteLine("-----------------------remove id is " + tempCLabel.ID);
                }
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CLabelList.Count; i++)
            {
                CLabelList[i].IsCommon = !CLabelList[i].IsCommon;
                Debug.WriteLine("actually id is " + CLabelList[i].ID);
            }
        }                           

        private void testdatagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex()+1;
            Debug.WriteLine("row item is " + e.Row.Item.ToString());
        }


        public void InitialDataGrid(ZoneInfo First,CDataGrid PrimaryDataGrid)
        {
            for (int i = 0; i < First.OutPutState.Count(); i++)
            {
                if(PrimaryDataGrid.Columns.Count==0)
                {
                    DataGridTemplateColumn Column = new DataGridTemplateColumn()
                    {
                        Header = "Name",
                        Width = 200,
                    };
                    DataTemplate TempCellTemplate = new DataTemplate();
                    FrameworkElementFactory CellTextBox = new FrameworkElementFactory(typeof(TextBox));
                    CellTextBox.SetBinding(TextBox.TextProperty, new Binding("ZoneName"));
                    TempCellTemplate.VisualTree = CellTextBox;
                    Column.CellTemplate = TempCellTemplate;
                    PrimaryDataGrid.Columns.Add(Column);
                }
                else
                {
                    DataGridTemplateColumn Column = new DataGridTemplateColumn()
                    {
                        Header = string.Format("OutPut{0}", PrimaryDataGrid.Columns.Count),
                        Width = 40,
                        HeaderStyle = (Style)FindResource("DataGridColumnHeaderStyle1"),
                    };
                    DataTemplate TempCellTemplate = new DataTemplate();
                    FrameworkElementFactory CellCLabel = new FrameworkElementFactory(typeof(CLabel));
                    TempCellTemplate.VisualTree = CellCLabel;
                    Column.CellTemplate = TempCellTemplate;
                    PrimaryDataGrid.Columns.Add(Column);
                }
            }
        }
    }

    public class ZoneInfo:DependencyObject
    {
        public static readonly DependencyProperty ZoneNameProperty = DependencyProperty.Register("ZoneName", typeof(string), typeof(ZoneInfo));
        public string ZoneName
        {
            get { return (string)GetValue(ZoneNameProperty); }
            set { SetValue(ZoneNameProperty, value); }
        }
            
        private bool[] _OutPutState = null;
        private static int _Count;
        public bool[] OutPutState
        {
            get
            {
                if (_OutPutState == null)
                    _OutPutState = new bool[_Count];
                return _OutPutState;
            }
        }

        public ZoneInfo()
        {
            if (_Count == 0)
                return;
            _OutPutState = new bool[_Count];
        }

        public ZoneInfo(int Length)
        {
            _Count = Length;
            _OutPutState = new bool[_Count];
        }
    }
}


