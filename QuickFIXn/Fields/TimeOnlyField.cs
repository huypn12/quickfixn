using System;
using QuickFix.Fields.Converters;

namespace QuickFix.Fields;

public class TimeOnlyField : DateTimeField
{
    public TimeOnlyField(int tag)
        : base(tag, new DateTime()) { }

    public TimeOnlyField(int tag, DateTime dt)
        : base(tag, dt) { }

    public TimeOnlyField(int tag, DateTime dt, bool showMilliseconds)
        : base(tag, dt, showMilliseconds) { }

    public TimeOnlyField(int tag, DateTime dt, TimeStampPrecision timeFormatPrecision)
        : base(tag, dt, timeFormatPrecision) { }

    protected override string MakeString()
    {
        return DateTimeConverter.ToFIXTimeOnly(Value, base.TimePrecision);
    }
}
