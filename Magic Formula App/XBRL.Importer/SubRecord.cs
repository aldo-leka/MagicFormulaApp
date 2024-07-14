namespace XBRL.Importer
{
    /// <summary>
    /// A simple wrapper over a quarterly database record to provide property names
    /// reflecting the columns in the SUB file.
    /// </summary>
    /// <remarks>
    /// The SUB file is the "Submission data set". This includes one record for each XBRL
    /// submission. The set includes fields of information pertinent to the submission and
    /// the filing entity.Information is extracted from the SEC’s EDGAR system and the filings
    /// submitted to the SEC by registrants.
    /// </remarks>
    public class SubRecord : QuarterlyDatabaseRecord
    {
        /// <summary>
        /// Possible filing status values.
        /// </summary>
        public enum FilerStatusValue
        {
            LargeAccelerated,
            Accelerated,
            SmallerReportingAccelerated,
            NonAccelerated,
            SmallerReportingFiler,
            NotAssigned
        }

        /// <summary>
        /// Possible fiscal period focus values.
        /// </summary>
        public enum FiscalPeriodFocusValue
        {
            FiscalYear,
            FirstQuarter,
            SecondQuarter,
            ThirdQuarter,
            FourthQuarter,
            FirstHalf,
            SecondHalf,
            NineMonths,
            FirstTrimester,
            SecondTrimester,
            ThirdTrimester,
            EightMonths,
            CalendarYear,
            Unknown
        }

        internal const int AccessionNumberColumn = 0;
        internal const int CentralIndexKeyColumn = 1;
        internal const int RegistrantNameColumn = 2;
        internal const int StandardIndustrialClassificationColumn = 3;
        internal const int CountryOfBusinessAddressColumn = 4;
        internal const int StateOrProvinceOfBusinessAddressColumn = 5;
        internal const int CityOfBusinessAddressColumn = 6;
        internal const int ZipCodeOfBusinessAddressColumn = 7;
        internal const int StreetOfBusinessAddressLine1Column = 8;
        internal const int StreetOfBusinessAddressLine2Column = 9;
        internal const int PhoneNumberOfBusinessAddressColumn = 10;
        internal const int CountryOfMailingAddressColumn = 11;
        internal const int StateOrProvinceOfMailingAddressColumn = 12;
        internal const int CityOfMailingAddressColumn = 13;
        internal const int ZipCodeOfMailingAddressColumn = 14;
        internal const int StreetOfMailingAddressLine1Column = 15;
        internal const int StreetOfMailingAddressLine2Column = 16;
        internal const int CountryOfIncorporationColumn = 17;
        internal const int StateOrProvinceOfIncorporationColumn = 18;
        internal const int EmployeeIdentificationNumberColumn = 19;
        internal const int FormerNameColumn = 20;
        internal const int FormerNameDateChangeColumn = 21;
        internal const int FilerStatusColumn = 22;
        internal const int WellKnownSeasonedIssuerColumn = 23;
        internal const int FiscalYearEndDateColumn = 24;
        internal const int FilingSubmissionTypeColumn = 25;
        internal const int BalanceSheetDateColumn = 26;
        internal const int FiscalYearFocusColumn = 27;
        internal const int FiscalPeriodFocusColumn = 28;
        internal const int FilingDateColumn = 29;
        internal const int FilingAcceptanceDateColumn = 30;
        internal const int PreviousReportFlagColumn = 31;
        internal const int QuantitativeDisclosuresInFootnotesAndSchedulesFlagColumn = 32;
        internal const int XbrlInstanceNameColumn = 33;
        internal const int CentralIndexKeyCountColumn = 34;
        internal const int AdditionalCentralIndexKeysColumn = 35;

        /// <summary>
        /// Accession Number. The 20-character string formed from the 18-digit number assigned
        /// by the SEC to each EDGAR submission.
        /// </summary>
        public string AccessionNumber { get { return this[AccessionNumberColumn]; } }

        /// <summary>
        /// Central Index Key (CIK). Ten digit number assigned by the SEC to each registrant
        /// that submits filings.
        /// </summary>
        public string CentralIndexKey { get { return this[CentralIndexKeyColumn]; } }

        /// <summary>
        /// Name of registrant. This corresponds to the name of the legal entity as recorded
        /// in EDGAR as of the filing date.
        /// </summary>
        public string RegistrantName { get { return this[RegistrantNameColumn]; } }

        /// <summary>
        /// Standard Industrial Classification(SIC). Four digit code assigned by the SEC as
        /// of the filing date, indicating the registrant’s type of business.
        /// </summary>
        public string StandardIndustrialClassification { get { return this[StandardIndustrialClassificationColumn]; } }

        /// <summary>
        /// The ISO 3166-1 country of the registrant's business address.
        /// </summary>
        public string CountryOfBusinessAddress { get { return this[CountryOfBusinessAddressColumn]; } }

        /// <summary>
        /// The state or province of the registrant’s business address, if field countryba
        /// is US or CA.
        /// </summary>
        public string StateOrProvinceOfBusinessAddress { get { return this[StateOrProvinceOfBusinessAddressColumn]; } }

        /// <summary>
        /// The city of the registrant's business address.
        /// </summary>
        public string CityOfBusinessAddress { get { return this[CityOfBusinessAddressColumn]; } }

        /// <summary>
        /// The zip code of the registrant’s business address.
        /// </summary>
        public string ZipCodeOfBusinessAddress { get { return this[ZipCodeOfBusinessAddressColumn]; } }

        /// <summary>
        /// The first line of the street of the registrant’s business address.
        /// </summary>
        public string StreetOfBusinessAddressLine1 { get { return this[StreetOfBusinessAddressLine1Column]; } }

        /// <summary>
        /// The second line of the street of the registrant’s business address.
        /// </summary>
        public string StreetOfBusinessAddressLine2 { get { return this[StreetOfBusinessAddressLine2Column]; } }

        /// <summary>
        /// The phone number of the registrant’s business address.
        /// </summary>
        public string PhoneNumberOfBusinessAddress { get { return this[PhoneNumberOfBusinessAddressColumn]; } }

        /// <summary>
        /// The ISO 3166-1 country of the registrant's mailing address.
        /// </summary>
        public string CountryOfMailingAddress { get { return this[CountryOfMailingAddressColumn]; } }

        /// <summary>
        /// The state or province of the registrant’s mailing address, if field countryma
        /// is US or CA.
        /// </summary>
        public string StateOrProvinceOfMailingAddress { get { return this[StateOrProvinceOfMailingAddressColumn]; } }

        /// <summary>
        /// The city of the registrant's mailing address.
        /// </summary>
        public string CityOfMailingAddress { get { return this[CityOfMailingAddressColumn]; } }

        /// <summary>
        /// The zip code of the registrant’s mailing address.
        /// </summary>
        public string ZipCodeOfMailingAddress { get { return this[ZipCodeOfMailingAddressColumn]; } }

        /// <summary>
        /// The first line of the street of the registrant’s mailing address.
        /// </summary>
        public string StreetOfMailingAddressLine1 { get { return this[StreetOfMailingAddressLine1Column]; } }

        /// <summary>
        /// The second line of the street of the registrant’s mailing address.
        /// </summary>
        public string StreetOfMailingAddressLine2 { get { return this[StreetOfMailingAddressLine2Column]; } }

        /// <summary>
        /// The country of incorporation for the registrant.
        /// </summary>
        public string CountryOfIncorporation { get { return this[CountryOfIncorporationColumn]; } }

        /// <summary>
        /// The state or province of incorporation for the registrant, if countryinc is
        /// US or CA.
        /// </summary>
        public string StateOrProvinceOfIncorporation { get { return this[StateOrProvinceOfIncorporationColumn]; } }

        /// <summary>
        /// Employee Identification Number, 9 digit identification number assigned by
        /// the Internal Revenue Service to business entities operating in the United States.
        /// </summary>
        public string EmployeeIdentificationNumber { get { return this[EmployeeIdentificationNumberColumn]; } }

        /// <summary>
        /// Most recent former name of the registrant, if any.
        /// </summary>
        public string FormerName { get { return this[FormerNameColumn]; } }

        /// <summary>
        /// Date of change from the former name, if any.
        /// </summary>
        public string FormerNameDateChange { get { return this[FormerNameDateChangeColumn]; } }

        /// <summary>
        /// Filer status with the SEC at the time of submission:
        /// 1-LAF=Large Accelerated,
        /// 2-ACC=Accelerated,
        /// 3-SRA=Smaller Reporting Accelerated,
        /// 4-NON=Non-Accelerated,
        /// 5-SML=Smaller Reporting Filer,
        /// NULL=not assigned.
        /// </summary>
        public FilerStatusValue FilerStatus
        {
            get
            {
                var stringValue = this[FilerStatusColumn];
                var status = FilerStatusValue.NotAssigned;
                switch (stringValue)
                {
                    case "1-LAF":
                        status = FilerStatusValue.LargeAccelerated;
                        break;
                    case "2-ACC":
                        status = FilerStatusValue.Accelerated;
                        break;
                    case "3-SRA":
                        status = FilerStatusValue.SmallerReportingAccelerated;
                        break;
                    case "4-NON":
                        status = FilerStatusValue.NonAccelerated;
                        break;
                    case "5-SML":
                        status = FilerStatusValue.SmallerReportingFiler;
                        break;
                    default:
                        break;
                }
                return status;
            }
        }

        /// <summary>
        /// Well Known Seasoned Issuer(WKSI). An issuer that meets specific SEC
        /// requirements at some point during a 60-day period preceding the date the issuer
        /// satisfies its obligation to update its shelf registration statement.
        /// </summary>
        public bool WellKnownSeasonedIssuer { get { return ConvertToBool(this[WellKnownSeasonedIssuerColumn]); } }

        /// <summary>
        /// Fiscal Year End Date.
        /// </summary>
        public string FiscalYearEndDate { get { return this[FiscalYearEndDateColumn]; } }

        /// <summary>
        /// The submission type of the registrant’s filing.
        /// </summary>
        public string FilingSubmissionType { get { return this[FilingSubmissionTypeColumn]; } }

        /// <summary>
        /// Balance Sheet Date.
        /// </summary>
        public DateTime BalanceSheetDate { get { return ConvertToDateFromYyyyMmDd(this[BalanceSheetDateColumn]); } }

        /// <summary>
        /// Fiscal Year Focus (as defined in EFM Ch. 6).
        /// </summary>
        public string FiscalYearFocus { get { return this[FiscalYearFocusColumn]; } }

        /// <summary>
        /// Fiscal Period Focus (as defined in EFM Ch. 6) within Fiscal Year.The 10-Q for the
        /// 1st, 2nd and 3rd quarters would have a fiscal period focus of Q1, Q2 (or H1), and
        /// Q3(or M9) respectively, and a 10-K would have a fiscal period focus of FY.
        /// </summary>
        public FiscalPeriodFocusValue FiscalPeriodFocus
        {
            get
            {
                var stringValue = this[FiscalPeriodFocusColumn];
                var focus = FiscalPeriodFocusValue.Unknown;
                switch (stringValue)
                {
                    case "FY":
                        focus = FiscalPeriodFocusValue.FiscalYear;
                        break;
                    case "Q1":
                        focus = FiscalPeriodFocusValue.FirstQuarter;
                        break;
                    case "Q2":
                        focus = FiscalPeriodFocusValue.SecondQuarter;
                        break;
                    case "Q3":
                        focus = FiscalPeriodFocusValue.ThirdQuarter;
                        break;
                    case "Q4":
                        focus = FiscalPeriodFocusValue.FourthQuarter;
                        break;
                    case "H1":
                        focus = FiscalPeriodFocusValue.FirstHalf;
                        break;
                    case "H2":
                        focus = FiscalPeriodFocusValue.SecondHalf;
                        break;
                    case "M9":
                        focus = FiscalPeriodFocusValue.NineMonths;
                        break;
                    case "T1":
                        focus = FiscalPeriodFocusValue.FirstTrimester;
                        break;
                    case "T2":
                        focus = FiscalPeriodFocusValue.SecondTrimester;
                        break;
                    case "T3":
                        focus = FiscalPeriodFocusValue.ThirdTrimester;
                        break;
                    case "M8":
                        focus = FiscalPeriodFocusValue.EightMonths;
                        break;
                    case "CY":
                        focus = FiscalPeriodFocusValue.CalendarYear;
                        break;
                    default:
                        break;
                }
                return focus;
            }
        }

        /// <summary>
        /// The date of the registrant’s filing with the Commission.
        /// </summary>
        public DateTime FilingDate { get { return ConvertToDateFromYyyyMmDd(this[FilingDateColumn]); } }

        /// <summary>
        /// The acceptance date and time of the registrant’s filing with the Commission.
        /// Filings accepted after 5:30pm EST are considered filed on the following
        /// business day.
        /// </summary>
        public DateTime FilingAcceptanceDate { get { return ConvertToDateTime(this[FilingAcceptanceDateColumn]); } }

        /// <summary>
        /// Previous Report –TRUE indicates that the submission information was
        /// subsequently amended.
        /// </summary>
        public bool PreviousReportFlag { get { return ConvertToBool(this[PreviousReportFlagColumn]); } }

        /// <summary>
        /// TRUE indicates that the XBRL submission contains quantitative disclosures
        /// within the footnotes and schedules at the required detail level(e.g., each
        /// amount).
        /// </summary>
        public bool QuantitativeDisclosuresInFootnotesAndSchedulesFlag { get { return ConvertToBool(this[QuantitativeDisclosuresInFootnotesAndSchedulesFlagColumn]); } }

        /// <summary>
        /// The name of the submitted XBRL Instance Document (EX-101.INS) type data file.
        /// The name often begins with the company ticker symbol.
        /// </summary>
        public string XbrlInstanceName { get { return this[XbrlInstanceNameColumn]; } }

        /// <summary>
        /// Number of Central Index Keys(CIK) of registrants (i.e., business units) included
        /// in the consolidating entity’s submitted filing.
        /// </summary>
        public int CentralIndexKeyCount { get { return ConvertToInt(this[CentralIndexKeyCountColumn]); } }

        /// <summary>
        /// Additional CIKs of coregistrants included in a consolidating entity’s EDGAR
        /// submission, separated by spaces.If there are no other coregistrants(i.e., nciks= 1),
        /// the value of aciks is NULL. For a very small number of filers, the list of
        /// coregistrants is too long to fit in the field.Where this is the case, PARTIAL will
        /// appear at the end of the list indicating that not all coregistrants’ CIKs are
        /// included in the field; users should refer to the complete submission file for all
        /// CIK information.
        /// </summary>
        public string AdditionalCentralIndexKeys { get { return this[AdditionalCentralIndexKeysColumn]; } }

        /// <summary>
        /// The absolute URL of the XBRL instance for this submission.
        /// </summary>
        /// <remarks>
        /// The SEC website folder http://www.sec.gov/Archives/edgar/data/{cik}/{accession}/
        /// will always contain all the data sets for a given submission.To assemble the folder
        /// address to any filing referenced in the SUB data set, simply substitute {cik} with
        /// the cik field and replace {accession} with the adsh field (after removing the dash
        /// character). 
        /// </remarks>
        public string XbrlInstanceUrl
        {
            get
            {
                var cik = CentralIndexKey;
                var accession = AccessionNumber;
                var instanceName = XbrlInstanceName;
                var accessionWithNoDashes = accession.Replace("-", string.Empty);
                var url = "http://www.sec.gov/Archives/edgar/data/" + cik + "/" + accessionWithNoDashes + "/" + instanceName;
                return url;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SubRecord()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="record">
        /// The quarterly database record to serve as the object's record.
        /// </param>
        public SubRecord(string lineFromFile) : base(lineFromFile)
        {
        }
    }

}