using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
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


namespace RAA_2_Module2_Bonus
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        public string CategoryName;
        public Document Doc;
        public List<string> TypeNames;
        public string ParamName;
        public string ParamDataType;
        public MyForm(Document doc, List<Category> catList)
        {
            InitializeComponent();

            Doc = doc;

            foreach(Category category in catList)
            {
                cmbCategory.Items.Add(category.Name);
            }
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbxTypes.Items.Clear();
            CategoryName = cmbCategory.SelectedItem.ToString();

            Category currentCat = Utils.GetCategoryByName(Doc, CategoryName);

            if(currentCat != null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(Doc);
                collector.OfCategoryId(currentCat.Id);
                collector.WhereElementIsElementType();

                List<Element> groupedList = collector.GroupBy(x => x.Name).Select(x => x.First()).ToList();
                List<Element> sortedList = groupedList.OrderBy(x => x.Name).ToList();

                foreach(Element curElem in sortedList)
                {
                    lbxTypes.Items.Add(curElem.Name);
                }
            }
        }

        private void lbxTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TypeNames = new List<string>();
            cmbParameter.Items.Clear();

            foreach(string item in lbxTypes.SelectedItems)
            {
                TypeNames.Add(item);
            }

            List<ElementType> selectedElemTypes = Utils.GetElementTypesByName(Doc, CategoryName, TypeNames);

            if(selectedElemTypes.Count > 0)
            {
                List<Parameter> paramList = new List<Parameter>();

                foreach(ElementType curType in selectedElemTypes)
                {
                    paramList.AddRange(Utils.GetAllParmatersFromElement(curType));
                }

                List<Parameter> groupedList = paramList.GroupBy(x => x.Definition.Name).Select(x => x.First()).ToList();
                List<Parameter> sortedList = groupedList.OrderBy(x => x.Definition.Name).ToList();

                foreach(Parameter curParam in sortedList)
                {
                    cmbParameter.Items.Add(curParam.Definition.Name);
                }
            }
        }

        private void cmbParameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbxValue.Text = "";
            tbxValue.IsEnabled = true;
            ParamName = cmbParameter.SelectedItem.ToString(); 

            Parameter curParam = Utils.GetParameterByName(Doc, CategoryName, TypeNames.FirstOrDefault(), ParamName);
        
            if(curParam.StorageType == StorageType.String)
            {
                lblValue.Content = "Set Parameter Value (as string):";
                ParamDataType = "string";
            }
            else if(curParam.StorageType == StorageType.Integer)
            {
                lblValue.Content = "Set Parameter Value (as integer):";
                ParamDataType= "integer";
            }
            else if(curParam.StorageType == StorageType.Double)
            {
                lblValue.Content = "Set Parameter Value (as double):";
                ParamDataType= "double";
            }
            else
            {
                ParamDataType = "none";
                lblValue.Content = "Set Parameter Value:";
                tbxValue.Text = "Cannot set this parameter";
                tbxValue.IsEnabled = false;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= false;
            this.Close();
        }

        public string GetNewValue()
        {
            return tbxValue.Text;
        }
    }
}
