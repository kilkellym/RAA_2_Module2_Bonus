using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAA_2_Module2_Bonus
{
    internal static class Utils
    {
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static List<Category> GetAllCategories(Document doc)
        {
            List<Category> categories = new List<Category>();

            foreach(Category curCat in doc.Settings.Categories)
            {
                categories.Add(curCat);
            }

            List<Category> sortedList = categories.OrderBy(x => x.Name).ToList();

            return sortedList;
        }

        internal static Category GetCategoryByName(Document doc, string categoryName)
        {
            List<Category> categories = GetAllCategories(doc);

            foreach(Category curCat in categories)
            {
                if (curCat.Name == categoryName)
                    return curCat;
            }

            return null;
        }

        internal static List<ElementType> GetElementTypesByName(Document doc, string categoryName, List<string> typeNames)
        {
            List<ElementType> types = new List<ElementType>();

            foreach(string type in typeNames)
            {
                ElementType currentType = GetElementTypeByName(doc, categoryName, type);

                if(currentType != null)
                    types.Add(currentType);
            }

            return types;
        }

        private static ElementType GetElementTypeByName(Document doc, string categoryName, string name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ElementType));

            foreach(ElementType curType in collector)
            {
                if(curType.Name == name && curType.Category.Name == categoryName)
                    return curType;
            }

            return null;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        internal static List<Parameter> GetAllParmatersFromElement(ElementType curType)
        {
            List<Parameter> returnList = new List<Parameter>();

            foreach(Parameter curParam in curType.Parameters)
            {
                returnList.Add(curParam);
            }

            return returnList;
        }

        internal static Parameter GetParameterByName(Document doc, string categoryName, string typeName, string paramName)
        {
            ElementType curType = GetElementTypeByName(doc, categoryName, typeName);
            Parameter curParam = curType.GetParameters(paramName).FirstOrDefault();

            if (curParam != null)
                return curParam;

            return null;
        }

        internal static List<Parameter> GetParametersByName(Document doc, string categoryName, List<string> typeNames, string paramName)
        {
            List<Parameter> returnList = new List<Parameter>();

            foreach(string typeName in typeNames)
            {
                Parameter curParam = GetParameterByName(doc, categoryName, typeName, paramName);

                if(curParam != null)
                    returnList.Add(curParam);
            }

            return returnList;
        }
    }
}
