using System;
using System.Globalization;

namespace XBRL.Importer
{
    public class QuarterlyDatabaseRecord
    {
        protected string[] _fields;
        public string this[int number]
        {
            get
            {
                if (number < 0)
                    throw new IndexOutOfRangeException();
                if (number >= _fields.Length)
                    throw new IndexOutOfRangeException();
                return _fields[number];
            }
        }

        public QuarterlyDatabaseRecord()
        {
        }

        public QuarterlyDatabaseRecord(string lineFromFile)
        {
            Populate(lineFromFile);
        }

        internal void Populate(string lineFromFile)
        {
            _fields = lineFromFile.Split('\t');
            for (var index = 0; index < _fields.Length; index++)
            {
                _fields[index] = _fields[index].Trim();
            }
        }

        /// <summary>
        /// Convert a string to a Boolean.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        protected bool ConvertToBool(string stringValue)
        {
            return stringValue.Equals("1");
        }

        /// <summary>
        /// Convert a string to a DateTime.
        /// </summary>
        /// <param name="stringValue">
        /// Date in yymmdd format.
        /// </param>
        /// <returns>
        /// </returns>
        protected DateTime ConvertToDateFromYyyyMmDd(string stringValue)
        {
            return DateTime.ParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        /// <summary>
        /// Convert a string to a DateTime.
        /// </summary>
        /// <param name="stringValue">
        /// Date in yymmdd format.
        /// </param>
        /// <returns>
        /// </returns>
        protected DateTime ConvertToDateTime(string stringValue)
        {
            return DateTime.Parse(stringValue);
        }

        /// <summary>
        /// Convert a string to an integer.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        protected int ConvertToInt(string stringValue)
        {
            return int.Parse(stringValue);
        }
    }
}