using System;

namespace XBRL.Importer
{
    /// <summary>
    /// A simple wrapper over a quarterly database record to provide property names
    /// reflecting the columns in the NUM file.
    /// </summary>
    public class NumRecord : QuarterlyDatabaseRecord
    {
        internal const int AccessionNumberColumn = 0;
        internal const int UniqueIdentifierColumn = 1;
        internal const int VersionColumn = 2;
        internal const int CoRegistrantColumn = 3;
        internal const int DataValueEndDateColumn = 4;
        internal const int NumberOfQuartersRepresentedColumn = 5;
        internal const int UnitOfMeasureColumn = 6;
        internal const int ValueColumn = 7;
        internal const int FootnoteColumn = 8;

        /// <summary>
        /// Accession Number. The 20-character string formed from the 18-digit number assigned
        /// by the SEC to each EDGAR submission.
        /// </summary>
        public string AccessionNumber { get { return this[AccessionNumberColumn]; } }

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
        /// If specified, indicates a specific co-registrant, the parent company, or other entity(e.g.,
        /// guarantor). NULL indicates the consolidated entity.
        /// </summary>
        public string CoRegistrant { get { return this[CoRegistrantColumn]; } }

        /// <summary>
        /// The end date for the data value, rounded to the nearest month end.
        /// </summary>
        public DateTime DataValueEndDate { get { return ConvertToDateFromYyyyMmDd(this[DataValueEndDateColumn]); } }

        /// <summary>
        /// The count of the number of quarters represented by the data value, rounded to the nearest
        /// whole number. “0” indicates it is a point-in-time value.
        /// </summary>
        public int NumberOfQuartersRepresented { get { return ConvertToInt(this[NumberOfQuartersRepresentedColumn]); } }

        /// <summary>
        /// The unit of measure for the value.
        /// </summary>
        public string UnitOfMeasure { get { return this[UnitOfMeasureColumn]; } }

        /// <summary>
        /// The value. This is not scaled, it is as found in the Interactive Data file, but is limited to
        /// four digits to the right of the decimal point.
        /// </summary>
        public decimal Value { get { return decimal.Parse(this[ValueColumn]); } }

        /// <summary>
        /// The text of any superscripted footnotes on the value, as shown on the statement page,
        /// truncated to 512 characters, or if there is no footnote, then this field will be blank.
        /// </summary>
        public string Footnote { get { return this[FootnoteColumn]; } }
    }
}