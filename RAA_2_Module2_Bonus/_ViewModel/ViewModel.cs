using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAA_2_Module2_Bonus._ViewModel
{
    public class ViewModel
    {
        public _Model.DocModel model { get; set; }
        public ObservableCollection<Category> CatList { get; set; }
        public ObservableCollection<Element> ElemTypeList { get; set; }
        public ObservableCollection<Parameter> ParamList { get; set; }
        public Category SelectedCategory { get; set; }
        public List<Element> SelectedElemTypes { get; set; }
        public Parameter SelectedParam { get; set; }
        public string LabelContent { get; set; }
        public string ParamDataType { get; set; }
        public string NewValue { get; set; }

        public ViewModel(UIApplication uiapp)
        {
            model = new _Model.DocModel(uiapp.ActiveUIDocument.Document);
            CatList = new ObservableCollection<Category>(model.GetAllCategories());
            ElemTypeList = new ObservableCollection<Element>();
            ParamList = new ObservableCollection<Parameter>();
            SelectedElemTypes = new List<Element>();
        }

        public void UpdateTypes()
        {
            if(SelectedCategory != null)
            {
                ElemTypeList.Clear();
                foreach(Element curElem in model.GetAllElementTypesByCategory(SelectedCategory))
                    ElemTypeList.Add(curElem);
            }
        }

        public void UpdateParameters()
        {
            if(SelectedElemTypes != null)
            {
                ParamList.Clear();
                foreach(Parameter curParam in model.GetAllParmatersFromElementTypes(SelectedElemTypes))
                {
                    ParamList.Add(curParam);
                }
            }
        }
        internal void UpdateParamValueString()
        {
            if (SelectedParam.StorageType == StorageType.String)
            {
                LabelContent = "Set Parameter Value (as string):";
                ParamDataType = "string";
            }
            else if (SelectedParam.StorageType == StorageType.Integer)
            {
                LabelContent = "Set Parameter Value (as integer):";
                ParamDataType = "integer";
            }
            else if (SelectedParam.StorageType == StorageType.Double)
            {
                LabelContent = "Set Parameter Value (as double):";
                ParamDataType = "double";
            }
            else
            {
                ParamDataType = "none";
                LabelContent = "Set Parameter Value:";
            }
        }

        public void Run()
        {
            List<Parameter> paramList = model.GetParametersByName(SelectedCategory, SelectedElemTypes, SelectedParam);
            
            using (Transaction t = new Transaction(model.Doc))
            {
                t.Start("Update parameter values");

                foreach (Parameter param in paramList)
                {
                    if (ParamDataType == "double")
                    {
                        double paramDouble = Convert.ToDouble(NewValue);
                        param.Set(paramDouble);
                    }
                    else if (ParamDataType == "integer")
                    {
                        int paramInt = Convert.ToInt32(NewValue);
                        param.Set(paramInt);
                    }
                    else if (ParamDataType == "string")
                    {
                        param.Set(NewValue);
                    }
                }

                t.Commit();
            }

            TaskDialog.Show("Complete", "Updated " + paramList.Count.ToString() + " types.");
        }
    }
}
