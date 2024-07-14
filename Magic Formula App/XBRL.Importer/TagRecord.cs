namespace XBRL.Importer
{
    /// <summary>
    /// A simple wrapper over a quarterly database record to provide property names
    /// reflecting the columns in the TAG file.
    /// </summary>
    public class TagRecord : QuarterlyDatabaseRecord
    {

        /// <summary>
        /// Possible values for the ValueType property.
        /// </summary>
        public enum ValueTypeValue
        {
            NoValue,
            PointInTime,
            Duration
        }

        /// <summary>
        /// Possible values for the NaturalAccountingBalance property.
        /// </summary>
        public enum NaturalAccountingBalanceValue
        {
            NoValue,
            Credit,
            Debit
        }

        internal const int UniqueIdentifierColumn = 0;
        internal const int VersionColumn = 1;
        internal const int CustomColumn = 2;
        internal const int AbstractColumn = 3;
        internal const int DataTypeColumn = 4;
        internal const int ValueTypeColumn = 5;
        internal const int NaturalAccountingBalanceColumn = 6;
        internal const int LabelTextColumn = 7;
        internal const int DocumentationColumn = 8;

        /// <summary>
        /// The unique identifier (name) for a tag in a specific taxonomy release.
        /// </summary>
        public string Tag { get { return this[UniqueIdentifierColumn]; } }

        /// <summary>
        /// For a standard tag, an identifier for the taxonomy; otherwise the accession number where
        /// the tag was defined.
        /// </summary>
        public string Version { get { return this[VersionColumn]; } }

        /// <summary>
        /// 1 if tag is custom (version=adsh), 0 if it is standard.Note: This flag is technically
        /// redundant with the version and adsh columns.
        /// </summary>
        public bool Custom { get { return ConvertToBool(this[CustomColumn]); } }

        /// <summary>
        /// 1 if the tag is not used to represent a numeric fact.
        /// </summary>
        public bool Abstract { get { return ConvertToBool(this[AbstractColumn]); } }

        /// <summary>
        /// If abstract=1, then NULL, otherwise the data type (e.g., monetary) for the tag.
        /// </summary>
        public string DataType { get { return this[DataTypeColumn]; } }

        /// <summary>
        /// If abstract=1, then NULL, otherwise the data type (e.g., monetary) for the tag.
        /// </summary>
        public ValueTypeValue ValueType
        {
            get
            {
                var stringValue = this[ValueTypeColumn].Trim();
                if (string.IsNullOrEmpty(stringValue) == true)
                    return ValueTypeValue.NoValue;
                if (stringValue.Equals("D") == true)
                    return ValueTypeValue.Duration;
                return ValueTypeValue.PointInTime;
            }
        }

        /// <summary>
        /// If datatype = monetary, then the tag’s natural accounting balance(debit or credit).
        /// </summary>
        public NaturalAccountingBalanceValue NaturalAccountingBalance
        {
            get
            {
                var stringValue = this[NaturalAccountingBalanceColumn].Trim();
                if (string.IsNullOrEmpty(stringValue) == true)
                    return NaturalAccountingBalanceValue.NoValue;
                if (stringValue.Equals("D") == true)
                    return NaturalAccountingBalanceValue.Debit;
                return NaturalAccountingBalanceValue.Credit;
            }
        }

        /// <summary>
        /// If a standard tag, then the label text provided by the taxonomy, otherwise the
        /// text provided by the filer.A tag which had neither would have a NULL value here.
        /// </summary>
        public string LabelText { get { return this[LabelTextColumn]; } }

        /// <summary>
        /// The detailed definition for the tag (truncated to 2048 characters). If a standard
        /// tag, then the text provided by the taxonomy, otherwise the text assigned by the filer.
        /// Some tags have neither, and this field is NULL.
        /// </summary>
        public string Documentation { get { return this[DocumentationColumn]; } }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TagRecord()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="record">
        /// The quarterly database record to serve as the object's record.
        /// </param>
        public TagRecord(string lineFromFile) : base(lineFromFile)
        {
        }
    }
}