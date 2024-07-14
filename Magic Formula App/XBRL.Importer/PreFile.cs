using System.Collections.Generic;

namespace XBRL.Importer
{
    /// <summary>
    /// A class to manage the PRE file found in a quarterly database.
    /// </summary>
    /// <remarks>
    /// The PRE data set contains one row for each line of the financial statements tagged by the filer.
    /// The source for the data set is the “as filed” XBRL filer submissions.Note that there may be more
    /// than one row per entry in NUM because the same tag can appear in more than one statement(the tag
    /// NetIncome, for example can appear in both the Income Statement and Cash Flows in a single financial
    /// statement, and the tag Cash may appear in both the Balance Sheet and Cash Flows).
    /// </remarks>
    public class PreFile : QuarterlyDatabaseFile
    {
        public List<PreRecord> Records
        {
            get { return GetRecords<PreRecord>(); }
        }
    }
}