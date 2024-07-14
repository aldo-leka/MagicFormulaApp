using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XBRL.Importer
{
    /// <summary>
    /// A base class to manage the various files in a quarterly database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All of the files in the quarterly database set have the same format.
    /// They are all tab-delimited text files. This base class handles all of
    /// the functionality common to each of the files in the set.
    /// </para>
    /// </remarks>
    public class QuarterlyDatabaseFile : IDisposable
    {
        private FileStream stream;
        protected StreamReader reader;
        internal List<ColumnIndex> indexes;
        private bool disposedValue = false;

        protected QuarterlyDatabaseFile()
        {
        }

        public virtual bool Load(string pathToDatabaseFile)
        {
            try
            {
                stream = new FileStream(pathToDatabaseFile, FileMode.Open, FileAccess.Read);
            }
            catch (DirectoryNotFoundException dnfe)
            {
                return false;
            }
            catch (FileNotFoundException fnfe)
            {
                return false;
            }
            reader = new StreamReader(stream);
            indexes = new List<ColumnIndex>();
            return true;
        }

        protected void BuildIndex(int column)
        {
            indexes.Add(new ColumnIndex(column, reader));
        }

        protected void BuildIndex(int[] columns)
        {
            indexes.Add(new ColumnIndex(columns, reader));
        }

        public List<T> GetRecords<T>() where T : QuarterlyDatabaseRecord, new()
        {
            var allRecords = new List<T>();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // The first line is the column headers, which we don't need.

            var headerLine = reader.ReadLine();

            // Read the actual data lines.

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var record = new T();
                record.Populate(line);
                allRecords.Add(record);
            }
            return allRecords;
        }

        public List<T> GetRecords<T>(int column, string key) where T : QuarterlyDatabaseRecord, new()
        {
            return GetRecords<T>(new int[] { column }, key);
        }

        public List<T> GetRecords<T>(int[] columns, string key) where T : QuarterlyDatabaseRecord, new()
        {
            return GetRecords<T>(columns, new string[] { key });
        }

        public List<T> GetRecords<T>(int[] columns, string[] keys) where T : QuarterlyDatabaseRecord, new()
        {
            var index = GetColumnIndex(columns);
            if (index != null)
                return GetRecords<T>(index, keys);
            return new List<T>();
        }

        private List<T> GetRecords<T>(ColumnIndex currentIndex, string[] keys) where T : QuarterlyDatabaseRecord, new()
        {
            var compositeKey = new StringBuilder();
            foreach (var currentKey in keys)
                compositeKey.Append(currentKey);
            var matchingRecords = new List<T>();
            var offsets = currentIndex.GetOffsets(compositeKey.ToString());
            if (offsets != null)
            {
                foreach (var currentOffset in offsets)
                {
                    reader.BaseStream.Position = currentOffset;
                    reader.DiscardBufferedData();
                    var line = reader.ReadLine();
                    var newRecord = new T();
                    newRecord.Populate(line);
                    matchingRecords.Add(newRecord);
                }
            }
            return matchingRecords;
        }

        private ColumnIndex GetColumnIndex(int[] columns)
        {
            foreach (var currentIndex in indexes)
            {
                if (ColumnsMatch(columns, currentIndex) == true)
                    return currentIndex;
            }
            return null;
        }

        /// <summary>
        /// Determines if there is a match between a list of column numbers and an index.
        /// </summary>
        /// <param name="columns">
        /// The list of zero-based columns to compare against the index.
        /// </param>
        /// <param name="indexCandidate">
        /// The index to compare.
        /// </param>
        /// <returns>
        /// True if all of the specified columns are represented in the specified
        /// index; false otherwise.
        /// </returns>
        private bool ColumnsMatch(int[] columns, ColumnIndex indexCandidate)
        {
            if (columns.Length != indexCandidate.Columns.Length)
                return false;
            foreach (var currentColumn in columns)
            {
                if (indexCandidate.Columns.Contains(currentColumn) == false)
                    return false;
            }
            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue == false)
            {
                if (disposing)
                {
                    reader?.Dispose();
                    stream?.Dispose();
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