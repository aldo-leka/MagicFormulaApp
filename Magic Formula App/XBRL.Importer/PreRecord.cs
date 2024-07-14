namespace XBRL.Importer
{
    /// <summary>
    /// A simple wrapper over a quarterly database record to provide property names
    /// reflecting the columns in the PRE file.
    /// </summary>
    public class PreRecord : QuarterlyDatabaseRecord
    {
        internal const int UniqueIdentifierColumn = 0;
        internal const int ReportGroupingColumn = 1;
        internal const int PresentationLineColumn = 2;
        internal const int FinancialStatementLocationColumn = 3;
        internal const int ParentheticalValueColumn = 4;
        internal const int InteractiveDataFileTypeColumn = 5;
        internal const int TagColumn = 6;
        internal const int VersionColumn = 7;
        internal const int PreferredLabelColumn = 8;

        /// <summary>
        /// Possible financial statement location values.
        /// </summary>
        public enum FinancialStatementLocationValue
        {
            BalanceSheet,
            IncomeStatement,
            CashFlow,
            Equity,
            ComprehensiveIncome,
            UnclassifiableStatement,
            Unknown
        }

        /// <summary>
        /// Possible interactive data file type values.
        /// </summary>
        public enum InteractiveDataFileTypeValue
        {
            Html,
            Xml,
            Unknown
        }

        /// <summary>
        /// The unique identifier (name) for a tag in a specific taxonomy release.
        /// </summary>
        public string AccessionNumber { get { return this[UniqueIdentifierColumn]; } }

        /// <summary>
        /// Represents the report grouping. This field corresponds to the statement(stmt) field, which
        /// indicates the type of statement. The numeric value refers to the “R file” as posted on the
        /// EDGAR Web site.
        /// </summary>
        public int ReportGrouping { get { return ConvertToInt(this[ReportGroupingColumn]); } }

        /// <summary>
        /// Represents the tag’s presentation line order for a given report. Together with the statement
        /// and report field, presentation location, order and grouping can be derived.
        /// </summary>
        public int PresentationLine { get { return ConvertToInt(this[PresentationLineColumn]); } }

        /// <summary>
        /// The financial statement location to which the value of the report field pertains.
        /// </summary>
        public FinancialStatementLocationValue FinancialStatementLocation
        {
            get
            {
                var stringValue = this[FinancialStatementLocationColumn];
                var status = FinancialStatementLocationValue.Unknown;
                switch (stringValue)
                {
                    case "BS":
                        status = FinancialStatementLocationValue.BalanceSheet;
                        break;
                    case "IS":
                        status = FinancialStatementLocationValue.IncomeStatement;
                        break;
                    case "CF":
                        status = FinancialStatementLocationValue.CashFlow;
                        break;
                    case "EQ":
                        status = FinancialStatementLocationValue.Equity;
                        break;
                    case "CI":
                        status = FinancialStatementLocationValue.ComprehensiveIncome;
                        break;
                    case "UN":
                        status = FinancialStatementLocationValue.UnclassifiableStatement;
                        break;
                    default:
                        break;
                }
                return status;
            }
        }

        /// <summary>
        /// Value was presented “parenthetically” instead of in columns within the financial statements.
        /// For example: Receivables(net of allowance for bad debts of $200 in 2012) $700.
        /// </summary>
        public bool ParentheticalValue { get { return ConvertToBool(this[ParentheticalValueColumn]); } }

        /// <summary>
        /// The type of interactive data file rendered on the EDGAR web site.
        /// </summary>
        public InteractiveDataFileTypeValue InteractiveDataFileType
        {
            get
            {
                var stringValue = this[InteractiveDataFileTypeColumn];
                var status = InteractiveDataFileTypeValue.Unknown;
                switch (stringValue)
                {
                    case "H":
                        status = InteractiveDataFileTypeValue.Html;
                        break;
                    case "X":
                        status = InteractiveDataFileTypeValue.Xml;
                        break;
                    default:
                        break;
                }
                return status;
            }
        }

        /// <summary>
        /// The tag chosen by the filer for this line item.
        /// </summary>
        public string Tag { get { return this[TagColumn]; } }

        /// <summary>
        /// The taxonomy identifier if the tag is a standard tag, otherwise adsh.
        /// </summary>
        public string Version { get { return this[VersionColumn]; } }

        /// <summary>
        /// The text presented on the line item, also known as a “preferred” label.
        /// </summary>
        public string PreferredLabel { get { return this[PreferredLabelColumn]; } }
    }
}