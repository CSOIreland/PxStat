

namespace XLsxHelper
{


    /// <summary>
    /// Class to be used as a container for an XLSX data cell
    /// </summary>
    public class XlsxValue
    {
        /// <summary>
        /// The contents of the cell
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// The StyleId. These correspond to the GenerateStyleSheet() index values
        /// </summary>
        public int StyleId { get; set; }
        /// <summary>
        /// Width of the cell. When calculating column widths, the application may set column width to the maximum column widths
        /// </summary>
        public int CellWidth { get; set; }

        public string FormulaText { get; set; }

        /// <summary>
        /// Constructor - set some default values
        /// </summary>
        public XlsxValue()
        {
            CellWidth = XlConstants.DEFAULT_CELL_WIDTH;
        }

    }

    internal static class XlConstants
    {
        internal const int DEFAULT_CELL_WIDTH = 12;
    }
}
