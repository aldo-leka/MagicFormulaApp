using System.Collections.Generic;

namespace XBRL.Importer
{
    /// <summary>
    /// A class to manage the NUM file found in a quarterly database.
    /// </summary>
    /// <remarks>
    /// The NUM data set contains numeric data, one row per data point in the financial statements. The source
    /// for the table is the “as filed” XBRL filer submissions.
    /// </remarks>
    public class NumFile : QuarterlyDatabaseFile
    {
        public List<NumRecord> Records
        {
            get { return GetRecords<NumRecord>(); }
        }

        public override bool Load(string pathToDatabaseFile)
        {
            var loadResult = base.Load(pathToDatabaseFile);
            if (loadResult == false)
                return false;
            this.BuildIndex(NumRecord.AccessionNumberColumn);
            this.BuildIndex(new int[] { NumRecord.AccessionNumberColumn, NumRecord.UniqueIdentifierColumn, NumRecord.VersionColumn });
            return true;
        }

        public List<NumRecord> GetRecordsMatchingAccessionNumberTagAndVersion(string accessionNumber, string tag, string version)
        {
            return GetRecords<NumRecord>(
                new int[]
                {
                    NumRecord.AccessionNumberColumn,
                    NumRecord.UniqueIdentifierColumn,
                    NumRecord.VersionColumn
                },
                new string[]
                {
                    accessionNumber,
                    tag,
                    version
                }
            );
        }

    }
}