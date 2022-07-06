

namespace Px5Migrator
{
    public interface IPxStatEngine
    {
        /// <summary>
        /// Saves the designated Px file into the StatBank DB
        /// </summary>
        /// <param name="inputFilePath">The path to the Px file to save into the DB.</param>
        void SavePxFileToDB(string inputFilePath);

        /// <summary>
        /// Loads the designated matrix from the DB Stat Bank and saves it into the Px file in the specified file name
        /// </summary>
        /// <param name="destinationFilePath">The destination file path where to save the px file</param>
        /// <param name="matrixId">The matrix id identifier on the satbank DB</param>
        void LoadPxFileFromDB(string destinationFilePath, string matrixId);

        /// <summary>
        /// Deserializes the given Px file into a sequence of elements and Serializes them back to a new file using the PxParser
        /// </summary>
        /// <param name="sourcePath">The original file we want to parse and deserialize into memory</param>
        /// <param name="destinationPath">The new file Px result from parsing the original file</param>
        void ProcessPxFile(string sourcePath, string destinationPath);

        /// <summary>
        /// Deserializes all the Px files in the  into a sequence of elements and Serializes them back to a new file using the PxParser
        /// </summary>
        /// <param name="sourcePath">The folder path where the original Px files are to be parsed</param>
        /// <param name="destinationPath">The folder where to save the resulting parsed Px files</param>
        void ProcessPxFilesInFolder(string originalFolderPath, string destinationFolderPath);

        /// <summary>
        /// Serializes the given Px document object to a destination file
        /// </summary>
        /// <param name="pxDoc">the Px doc object</param>
        /// <param name="destinationFilePath">the destination file path</param>
        void SavePxDocumentToFile(PxDocument pxDoc, string destinationFilePath);

        /// <summary>
        /// Returns a PxDocument with the result of the parsed Px file
        /// </summary>
        /// <param name="sourcePxFilePath">the path of the source px file to parse</param>
        /// <returns>The PxDocument that represents the parsed px file</returns>
        PxDocument GetPxDocumentFromFile(string sourcePxFilePath);

        PxDocument ParsePxFile(string sourcePxFilePath);
    }
}
