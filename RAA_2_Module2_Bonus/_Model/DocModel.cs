using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RAA_2_Module2_Bonus._Model
{
    public class DocModel
    {
        public Document Doc { get; set; }
        public DocModel(Document doc) 
        {
            Doc = doc;
        }
        internal List<Element> GetAllElementTypesByCategory(Category category)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            collector.OfCategoryId(category.Id);
            collector.WhereElementIsElementType();

            List<Element> groupedList = collector.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            List<Element> sortedList = groupedList.OrderBy(x => x.Name).ToList();

            return sortedList;
        }

        internal List<Parameter> GetAllParmatersFromElement(Element curType)
        {
            List<Parameter> returnList = new List<Parameter>();

            foreach (Parameter curParam in curType.Parameters)
            {
                returnList.Add(curParam);
            }

            return returnList;
        }

        internal Parameter GetParameterByName(string categoryName, string typeName, string paramName)
        {
            ElementType curType = GetElementTypeByName(categoryName, typeName);
            Parameter curParam = curType.GetParameters(paramName).FirstOrDefault();

            if (curParam != null)
                return curParam;

            return null;
        }

        internal List<Parameter> GetParametersByName(Category category, List<Element> types, Parameter param)
        {
            List<Parameter> returnList = new List<Parameter>();

            foreach (Element curType in types)
            {
                Parameter curParam = GetParameterByName(category.Name, curType.Name, param.Definition.Name);

                if (curParam != null)
                    returnList.Add(curParam);
            }

            return returnList;
        }

        internal List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            foreach (Category curCat in Doc.Settings.Categories)
            {
                categories.Add(curCat);
            }

            List<Category> sortedList = categories.OrderBy(x => x.Name).ToList();

            return sortedList;
        }

        internal Category GetCategoryByName(string categoryName)
        {
            List<Category> categories = GetAllCategories();

            foreach (Category curCat in categories)
            {
                if (curCat.Name == categoryName)
                    return curCat;
            }

            return null;
        }

        internal List<ElementType> GetElementTypesByName(string categoryName, List<string> typeNames)
        {
            List<ElementType> types = new List<ElementType>();

            foreach (string type in typeNames)
            {
                ElementType currentType = GetElementTypeByName(categoryName, type);

                if (currentType != null)
                    types.Add(currentType);
            }

            return types;
        }

        private ElementType GetElementTypeByName(string categoryName, string name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            collector.OfClass(typeof(ElementType));

            foreach (ElementType curType in collector)
            {
                if (curType.Name == name && curType.Category.Name == categoryName)
                    return curType;
            }

            return null;
        }

        internal List<Parameter> GetAllParmatersFromElementTypes(List<Element> selectedElemTypes)
        {
            List<Parameter> paramList = new List<Parameter>();

            foreach (Element curType in selectedElemTypes)
            {
                paramList.AddRange(GetAllParmatersFromElement(curType));
            }

            List<Parameter> groupedList = paramList.GroupBy(x => x.Definition.Name).Select(x => x.First()).ToList();
            List<Parameter> sortedList = groupedList.OrderBy(x => x.Definition.Name).ToList();

            return sortedList;
        }
    }
}
