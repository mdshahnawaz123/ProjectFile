using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace ProjectFile
{
    [Transaction(TransactionMode.Manual)]
    public class SharedPram : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            var app = commandData.Application.Application;

            try
            {
                // Check if Shared Parameter file exists
                var sharedParamPath = app.SharedParametersFilename;

                if (string.IsNullOrEmpty(sharedParamPath) || !System.IO.File.Exists(sharedParamPath))
                {
                    var td = new TaskDialog("Shared Parameter");
                    td.MainInstruction = "Shared Parameter file not found.";
                    td.MainContent = "Do you want to create a new Shared Parameter file?";
                    td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                    if (td.Show() == TaskDialogResult.Ok)
                    {
                        // Ask user where to save the file
                        var saveDialog = new FileSaveDialog("Text Files (*.txt)|*.txt");

                        if (saveDialog.Show() != ItemSelectionDialogResult.Confirmed)
                        {
                            return Result.Cancelled;
                        }

                        var modelPath = saveDialog.GetSelectedModelPath();
                        var filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

                        // Create the file
                        System.IO.File.Create(filePath).Close();

                        // Assign the file to Revit
                        app.SharedParametersFilename = filePath;

                        TaskDialog.Show("Success", "Shared Parameter file created successfully.");
                    }
                    else
                    {
                        return Result.Cancelled;
                    }
                }

                // Now open the shared parameter file
                var defFile = app.OpenSharedParameterFile();

                if (defFile == null)
                {
                    TaskDialog.Show("Error", "Unable to open Shared Parameter file.");
                    return Result.Failed;
                }

                // Now Check the Def GroupName:-

                var DefGroups = defFile.Groups.get_Item("ECD_SharedParameter") ?? defFile.Groups.Create("ECD_SharedParameter");

                // Now Create the Parameters

                var parameters = new (string PramName, ForgeTypeId DataType)[]
                {
                    ("(01)ECD_ABS_Asset",SpecTypeId.String.Text),
                    ("(01)ECD_ABS_Level",SpecTypeId.String.Text)
                };

                //L

                foreach (var param in parameters)
                {
                    using(var tx = new Transaction(doc,"Create the Shared Parameter"))
                    {
                        tx.Start();
                        var options = new ExternalDefinitionCreationOptions(param.PramName, param.DataType);
                        var def = DefGroups.Definitions.Create(options);
                        SharedParameterElement.Create(doc, def as ExternalDefinition);

                        //Now Let's Bind based on category:


                        tx.Commit();

                    }

                }
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}