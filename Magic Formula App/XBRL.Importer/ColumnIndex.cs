using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XBRL.Importer
{
    /// <summary>
    /// Manages an index to one of the columns in a quarterly database file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A column index is a pairing of a list of zero-based column numbers and a
    /// dictionary. The dictionary is a collection of keys and values. The keys
    /// represent the text found in the given column, and the values are a list of
    /// file offsets containing the key text in the given column. Note that a piece
    /// of text in a column may appear in more than one record, so there may be more
    /// than one file offset. The file offsets allow code to quickly navigate to a
    /// record if the caller needs to find the records with given key text.
    /// </para>
    /// <para>
    /// The list of zero-based column numbers allow for composite keys to be built.
    /// In the simple case, the list will have one number in it, which would denote an
    /// index built for a single column. The list allows for multiple column numbers,
    /// which, in turn, allows for composite column indexing.
    /// </para>
    /// <para>
    /// The text files in SEC quarterly database packages can be several hundreds
    /// of megabytes in size, and loading all of the files into memory for that they
    /// can be queried when needed would add quite a bit of memory pressure. It
    /// seems more efficient to build offset-based indexes for each of the records
    /// and navigate to the records needed by processing only when the specific
    /// records are needed. Loading all of the records in all of the files is
    /// inefficient, especially since it is most likely that most of the records
    /// will not even be requested during processing.
    /// </para>
    /// </remarks>
    internal class ColumnIndex
    {
        private SortedDictionary<string, List<long>> _index;

        // Lines in a quarterly database file end with one character: 0x0A.
        // This signals the end of a line of text. This character will not
        // be included in the string returned from StreamReader.ReadLine().
        //
        // Other text files might end a line with two characters, such as
        // 0x0D 0x0A. Obviously, if that were the case, the value of this
        // constant would be 2.
        private const int _numberOfLineEndingCharacters = 1;

        internal int[] Columns
        {
            get;
            private set;
        }


        internal ColumnIndex(int column, StreamReader stream)
        {
            int[] columns = new int[] { column };
            Initialize(columns, stream);
        }

        internal ColumnIndex(int[] columns, StreamReader stream)
        {
            Initialize(columns, stream);
        }

        internal void Initialize(int[] columns, StreamReader stream)
        {
            this.Columns = columns;
            stream.BaseStream.Seek(0, SeekOrigin.Begin);

            // The first line is the column headers, which we don't need.

            var headerLine = stream.ReadLine();
            long offset = headerLine.Length + _numberOfLineEndingCharacters;

            // Read the actual data lines.

            string line;
            _index = new SortedDictionary<string, List<long>>();

            while ((line = stream.ReadLine()) != null)
            {
                var record = new QuarterlyDatabaseRecord(line);
                var constructedKey = ConstructKey(columns, record);
                List<long> offsetList;
                if (_index.TryGetValue(constructedKey, out offsetList) == false)
                {
                    offsetList = new List<long>();
                    _index.Add(constructedKey, offsetList);
                }
                offsetList.Add(offset);
                offset += line.Length + _numberOfLineEndingCharacters;
            }
        }

        internal List<long> GetOffsets(string key)
        {
            List<long> offsets;
            _index.TryGetValue(key, out offsets);
            return offsets;
        }

        private string ConstructKey(int[] columns, QuarterlyDatabaseRecord record)
        {
            var constructedKey = new StringBuilder();
            foreach (int currentColumn in columns)
            {
                constructedKey.Append(record[currentColumn]);
            }
            return constructedKey.ToString();
        }
    }
}