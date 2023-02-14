#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAA_2_Module2_Bonus
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here
            List<Category> categories = Utils.GetAllCategories(doc);

            // open form
            MyForm currentForm = new MyForm(doc, categories)
            {
                Width = 450,
                Height = 550,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            // get form data and do something
            if (currentForm.DialogResult == false)
                return Result.Cancelled;

            List<Parameter> paramList = Utils.GetParametersByName(doc, currentForm.CategoryName, currentForm.TypeNames,
                currentForm.ParamName);

            if(paramList.Count > 0)
            {
                Transaction t = new Transaction(doc);
                t.Start("Set type parameters");

                foreach(Parameter param in paramList)
                {
                    string newValue = currentForm.GetNewValue();

                    if(currentForm.ParamDataType == "double")
                    {
                        double paramDouble = Convert.ToDouble(newValue);
                        param.Set(paramDouble);
                    }
                    else if(currentForm.ParamDataType == "integer")
                    {
                        int paramInt = Convert.ToInt32(newValue);
                        param.Set(paramInt);
                    }
                    else if(currentForm.ParamDataType == "string")
                    {
                        param.Set(newValue);
                    }
                }
                t.Commit();
                t.Dispose();
            }

            TaskDialog.Show("Complete", "Updated " + paramList.Count.ToString() + " types.");

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
