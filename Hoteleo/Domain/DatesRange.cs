namespace Hoteleo.Domain
{
    internal record struct DatesRange(DateTime Start, DateTime End)
    {
        public DatesRange(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay)
            : this(new DateTime(fromYear, fromMonth, fromDay), new DateTime(toYear, toMonth, toDay))
        {
        }

        public bool Overlap(DatesRange other)
        {
            return Start <= other.End && other.Start <= End;
        }
    }
}
