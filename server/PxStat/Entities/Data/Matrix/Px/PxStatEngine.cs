using System;
using System.IO;
using PxParser.Resources.Parser;
using API;

namespace PxStat.Data.Px
{
    /// <summary>
    /// Class
    /// </summary>
    public class PxStatEngine : IPxStatEngine
    {
        /// <summary>
        /// LoadPxFileFromDB
        /// </summary>
        /// <param name="destinationFilePath"></param>
        /// <param name="matrixId"></param>
        public void LoadPxFileFromDB(string destinationFilePath, string matrixId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ProcessPxFile
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public void ProcessPxFile(string sourcePath, string destinationPath)
        {

            ADO ado = new ADO("defaultConnection");

            try
            {
                var pxDoc = GetPxDocumentFromFile(sourcePath);

                if (pxDoc == null)
                {
                    return;
                }

                // Change to PxSchemaValidator
                var validator = new PxSchemaValidator();
                var result = validator.Validate(pxDoc);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        Log.Instance.Debug(error.ErrorMessage);
                    }
                    return;
                }

                Log.Instance.Debug("The PX Schema is valid");

                // Create another validator : SettingsValidator
                var matrix = new Matrix(pxDoc);

                var settingsValidator = new PxSettingsValidator(ado);
                var settingsResult = settingsValidator.Validate(matrix);
                if (!settingsResult.IsValid)
                {
                    foreach (var error in settingsResult.Errors)
                    {
                        Log.Instance.Debug(error.ErrorMessage);
                    }
                    return;
                }

                Log.Instance.Debug("The PX Settings are valid");

                // only do this if we are in debug mode!
                SavePxDocumentToFile(pxDoc, destinationPath);  //check this!!!

                PxKeywordDictionary.GetTypesFromFile(pxDoc);

            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }
            finally
            {
                ado.Dispose();
            }


        }

        /// <summary>
        /// ProcessPxFilesInFolder
        /// </summary>
        /// <param name="originalFolderPath"></param>
        /// <param name="destinationFolderPath"></param>
        public void ProcessPxFilesInFolder(string originalFolderPath, string destinationFolderPath)
        {
            if (originalFolderPath == null)
            {
                throw new ArgumentNullException(nameof(originalFolderPath));
            }

            string dirName = new DirectoryInfo(originalFolderPath).FullName;

            Log.Instance.DebugFormat("Parsing *.px files in folder {0}", dirName);

            if (!Directory.Exists(destinationFolderPath))  // if it doesn't exist, create
                Directory.CreateDirectory(destinationFolderPath);

            foreach (string f in Directory.GetFiles(originalFolderPath, "*.px"))
            {
                string fileName = new FileInfo(f).Name;
                ProcessPxFile(f, Path.Combine(destinationFolderPath, string.Format("{0}.txt", fileName)));
            }

            Log.Instance.Debug("Parsing of all *.px files completed.");

            //TODO: check config first before generate dictionary information files!!!
            PxKeywordDictionary.SaveToFile("AllTypes.txt", "AllExceptions.txt");
        }

        /// <summary>
        /// SavePxFileToDB
        /// </summary>
        /// <param name="inputFilePath"></param>
        public void SavePxFileToDB(string inputFilePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SavePxDocumentToFile
        /// </summary>
        /// <param name="pxDoc"></param>
        /// <param name="destinationFilePath"></param>
        public void SavePxDocumentToFile(PxDocument pxDoc, string destinationFilePath)
        {
            string resultFileName = new FileInfo(destinationFilePath).FullName;

            File.WriteAllText(resultFileName, pxDoc.ToPxString());

            Log.Instance.DebugFormat("Result saved to new file {0}", resultFileName);
        }

        /// <summary>
        /// GetPxDocumentFromFile
        /// </summary>
        /// <param name="fileNameToParse"></param>
        /// <returns></returns>
        public PxDocument GetPxDocumentFromFile(string fileNameToParse)
        {
            if (fileNameToParse == null)
            {
                throw new ArgumentNullException(nameof(fileNameToParse));
            }

            string fileName = new FileInfo(fileNameToParse).FullName;
            Log.Instance.DebugFormat("Parsing File {0}", fileName);

            string input = File.ReadAllText(fileNameToParse);
            return CreatePxDocument(fileName, input);
        }

        /// <summary>
        /// ParsePxInput
        /// </summary>
        /// <param name="pxInput"></param>
        /// <returns></returns>
        internal static PxDocument ParsePxInput(string pxInput)
        {

            return PxParser.Resources.Parser.PxParser.Parse(pxInput);

        }

        /// <summary>
        /// CreatePxDocument
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pxInput"></param>
        /// <returns></returns>
        internal static PxDocument CreatePxDocument(string fileName, string pxInput)
        {
            PxDocument result = null;

            try
            {
                result = PxParser.Resources.Parser.PxParser.Parse(pxInput);
            }
            catch (Exception e)
            {
                Log.Instance.ErrorFormat("Error Parsing File {0} Exception:", fileName);
                Log.Instance.Error(e);
            }

            return result;
        }

    }
}