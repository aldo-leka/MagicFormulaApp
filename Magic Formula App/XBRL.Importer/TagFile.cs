namespace XBRL.Importer
{
    /// <summary>
    /// A class to manage the TAG file found in a quarterly database.
    /// </summary>
    /// <remarks>
    /// The TAG data set contains all standard taxonomy tags, not just those appearing in submissions to date,
    /// and also includes all custom taxonomy tags defined in the submissions.The source is the “as filed” XBRL
    /// filer submissions.The standard tags are derived from taxonomies in
    /// http://www.sec.gov/info/edgar/edgartaxonomies.shtml.
    /// </remarks>
    public class TagFile : QuarterlyDatabaseFile
    {
        public List<TagRecord> Records
        {
            get { return GetRecords<TagRecord>(); }
        }

        public override bool Load(string pathToDatabaseFile)
        {
            var loadResult = base.Load(pathToDatabaseFile);
            if (loadResult == false)
                return false;
            this.BuildIndex(
                new int[]
                {
                    TagRecord.UniqueIdentifierColumn,
                    TagRecord.VersionColumn
                }
            );
            return true;
        }

        public List<TagRecord> GetRecordsMatchingTagAndVersion(string tag, string version)
        {
            return GetRecords<TagRecord>(
                new int[]
                {
                    TagRecord.UniqueIdentifierColumn,
                    TagRecord.VersionColumn
                },
                new string[]
                {
                    tag,
                    version
                }
            );
        }
    }
}