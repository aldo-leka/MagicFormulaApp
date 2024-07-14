using System;
using System.Formats.Tar;
using System.IO;

namespace XBRL.Importer
{
    /// <summary>
    /// Manages the set of quarterly database files issued by the SEC.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Quarterly databases provide selected information extracted from exhibits to corporate financial reports filed
    /// with the Commission using eXtensible Business Reporting Language (XBRL). The information is presented without
    /// change from the "as filed" annual and quarterly financial reports submitted by each registrant. The data is
    /// presented in a flattened format to help users analyze and compare it. The data sets also contain additional
    /// fields including a company's Standard Industrial Classification to facilitate the data's use.
    /// </para>
    /// <para>
    /// Quarterly databases are available from the SEC at http://www.sec.gov/dera/data/financial-statement-data-sets.html.
    /// </para>
    /// </remarks>
    public class QuarterlyDatabase : IDisposable
    {
        private bool disposedValue = false;

        public SubFile Sub { get; private set; }
        public TagFile Tag { get; private set; }
        public NumFile Num { get; private set; }
        public PreFile Pre { get; private set; }

        public QuarterlyDatabase()
        {
            Sub = null;
            Tag = null;
            Num = null;
            Pre = null;
        }

        public bool Load(string databaseLocation)
        {
            var subPath = Path.Combine(databaseLocation, "sub.txt");
            Sub = new SubFile();
            var subLoad = Sub.Load(subPath);
            if (subLoad == false)
                return false;

            var tagPath = Path.Combine(databaseLocation, "tag.txt");
            Tag = new TagFile();
            var tagLoad = Tag.Load(tagPath);
            if (tagLoad == false)
                return false;

            var numPath = Path.Combine(databaseLocation, "num.txt");
            Num = new NumFile();
            var numLoad = Num.Load(numPath);
            if (numLoad == false)
                return false;

            var prePath = Path.Combine(databaseLocation, "pre.txt");
            Pre = new PreFile();
            var preLoad = Pre.Load(prePath);
            if (preLoad == false)
                return false;

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Sub?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}