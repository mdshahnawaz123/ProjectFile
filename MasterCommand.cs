using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFile
{
    [Transaction(TransactionMode.Manual)]
    public class MasterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var app = uidoc.Application;
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Result.Succeeded;
        }
    }
}
