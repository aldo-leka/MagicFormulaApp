namespace XBRL.Importer
{
    /// <summary>
    /// A class to manage the SUB file found in a quarterly database.
    /// </summary>
    /// <remarks>
    /// The submissions data set contains summary information about an entire EDGAR submission. Some fields
    /// were sourced directly from EDGAR submission information, while other columns of data were sourced
    /// from the Interactive Data attachments of the submission.Note: EDGAR derived fields represent the
    /// most recent EDGAR assignment as of a given filing’s submission date and do not necessarily represent
    /// the most current assignments.
    /// </remarks>
    public class SubFile : QuarterlyDatabaseFile
    {
        public List<SubRecord> Records
        {
            get { return GetRecords<SubRecord>(); }
        }

        /// <summary>
        /// Return the URL of the XBRL instance referenced by the supplied database record.
        /// </summary>
        /// <remarks>
        /// The SEC website folder http://www.sec.gov/Archives/edgar/data/{cik}/{accession}/
        /// will always contain all the data sets for a given submission.To assemble the folder
        /// address to any filing referenced in the SUB data set, simply substitute {cik} with
        /// the cik field and replace {accession} with the adsh field (after removing the dash
        /// character). 
        /// </remarks>
        /// <param name="subRecord">
        /// The SUB database record containing the data from which the XBRL instance URL should
        /// be constructed.
        /// </param>
        /// <returns>
        /// The URL of the XBRL instance referenced by the supplied record.
        /// </returns>
        public string GetXbrlInstanceUrl(SubRecord subRecord)
        {
            var cik = subRecord.CentralIndexKey;
            var accession = subRecord.AccessionNumber;
            var instanceName = subRecord.XbrlInstanceName;
            var accessionWithNoDashes = accession.Replace("-", string.Empty);
            var url = "http://www.sec.gov/Archives/edgar/data/" + cik + "/" + accessionWithNoDashes + "/" + instanceName;
            return url;
        }

        public SubFile()
        {
        }

        public override bool Load(string pathToDatabaseFile)
        {
            var loadResult = base.Load(pathToDatabaseFile);
            if (loadResult == false)
                return false;
            this.BuildIndex(SubRecord.CentralIndexKeyColumn);
            this.BuildIndex(SubRecord.AccessionNumberColumn);
            return true;
        }

        public List<SubRecord> GetRecordsMatchingCik(string cik)
        {
            return GetRecords<SubRecord>(SubRecord.CentralIndexKeyColumn, cik);
        }

        public List<SubRecord> GetRecordsMatchingAccessionNumber(string accessionNumber)
        {
            return GetRecords<SubRecord>(SubRecord.AccessionNumberColumn, accessionNumber);
        }
    }
}