using System;
using QuickFix.Fields.Converters;

namespace QuickFix.Fields;

public class DateOnlyField : DateTimeField
{
    public DateOnlyField(int tag)
        : base(tag, new DateTime()) { }

    public DateOnlyField(int tag, DateTime dt)
        : base(tag, dt) { }

    public DateOnlyField(int tag, DateTime dt, bool showMilliseconds)
        : base(tag, dt, showMilliseconds) { }

    public DateOnlyField(int tag, DateTime dt, TimeStampPrecision timeFormatPrecision)
        : base(tag, dt, timeFormatPrecision) { }

    protected override string MakeString()
    {
        return DateTimeConverter.ToFIXDateOnly(Value);
    }
}
