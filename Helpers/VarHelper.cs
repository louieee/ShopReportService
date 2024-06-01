namespace ReportService.Helpers
{
    public class VarHelper
    {

        public enum ResponseStatus
        {
            ERROR,
            SUCCESS,
            PENDING,
            WARNING
        }
        
        public enum TimeFilter
        {
            Yearly,
            Monthly,
            Daily,
            Hourly
        }

        public enum MonthEnum
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }


    }
}
