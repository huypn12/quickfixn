// This is a generated file.  Don't edit it directly!

using System;
using QuickFix.Fields;

namespace QuickFix.FIX50;

public class TradingSessionList : Message
{
    public const string MsgType = "BJ";

    public TradingSessionList() : base()
    {
        Header.SetField(new MsgType("BJ"));
    }

    public TradSesReqID TradSesReqID
    {
        get
        {
            TradSesReqID val = new();
            GetField(val);
            return val;
        }
        set  => SetField(value);
    }

    public void Set(TradSesReqID val) { TradSesReqID = val; }
    public TradSesReqID Get(TradSesReqID val) { GetField(val); return val; }
    public bool IsSet(TradSesReqID val) { return IsSetTradSesReqID(); }
    public bool IsSetTradSesReqID() { return IsSetField(Tags.TradSesReqID); }

    public NoTradingSessions NoTradingSessions
    {
        get
        {
            NoTradingSessions val = new();
            GetField(val);
            return val;
        }
        set  => SetField(value);
    }

    public void Set(NoTradingSessions val) { NoTradingSessions = val; }
    public NoTradingSessions Get(NoTradingSessions val) { GetField(val); return val; }
    public bool IsSet(NoTradingSessions val) { return IsSetNoTradingSessions(); }
    public bool IsSetNoTradingSessions() { return IsSetField(Tags.NoTradingSessions); }

    public class NoTradingSessionsGroup : Group
    {
        public static int[] DefaultFieldOrder = [Tags.TradingSessionID, Tags.TradingSessionSubID, Tags.SecurityExchange, Tags.TradSesMethod, Tags.TradSesMode, Tags.UnsolicitedIndicator, Tags.TradSesStatus, Tags.TradSesStatusRejReason, Tags.TradSesStartTime, Tags.TradSesOpenTime, Tags.TradSesPreCloseTime, Tags.TradSesCloseTime, Tags.TradSesEndTime, Tags.TotalVolumeTraded, Tags.Text, Tags.EncodedTextLen, Tags.EncodedText, 0];

        [Obsolete("Use DefaultFieldOrder instead.  This will be removed in a future release.")]
        public static int[] fieldOrder = {Tags.TradingSessionID, Tags.TradingSessionSubID, Tags.SecurityExchange, Tags.TradSesMethod, Tags.TradSesMode, Tags.UnsolicitedIndicator, Tags.TradSesStatus, Tags.TradSesStatusRejReason, Tags.TradSesStartTime, Tags.TradSesOpenTime, Tags.TradSesPreCloseTime, Tags.TradSesCloseTime, Tags.TradSesEndTime, Tags.TotalVolumeTraded, Tags.Text, Tags.EncodedTextLen, Tags.EncodedText, 0};

        public NoTradingSessionsGroup()
          : base(Tags.NoTradingSessions, Tags.TradingSessionID, DefaultFieldOrder)
        {
        }

        public override Group Clone()
        {
            var clone = new NoTradingSessionsGroup();
            clone.CopyStateFrom(this);
            return clone;
        }

        public TradingSessionID TradingSessionID
        {
            get
            {
                TradingSessionID val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradingSessionID val) { TradingSessionID = val; }
        public TradingSessionID Get(TradingSessionID val) { GetField(val); return val; }
        public bool IsSet(TradingSessionID val) { return IsSetTradingSessionID(); }
        public bool IsSetTradingSessionID() { return IsSetField(Tags.TradingSessionID); }

        public TradingSessionSubID TradingSessionSubID
        {
            get
            {
                TradingSessionSubID val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradingSessionSubID val) { TradingSessionSubID = val; }
        public TradingSessionSubID Get(TradingSessionSubID val) { GetField(val); return val; }
        public bool IsSet(TradingSessionSubID val) { return IsSetTradingSessionSubID(); }
        public bool IsSetTradingSessionSubID() { return IsSetField(Tags.TradingSessionSubID); }

        public SecurityExchange SecurityExchange
        {
            get
            {
                SecurityExchange val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(SecurityExchange val) { SecurityExchange = val; }
        public SecurityExchange Get(SecurityExchange val) { GetField(val); return val; }
        public bool IsSet(SecurityExchange val) { return IsSetSecurityExchange(); }
        public bool IsSetSecurityExchange() { return IsSetField(Tags.SecurityExchange); }

        public TradSesMethod TradSesMethod
        {
            get
            {
                TradSesMethod val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesMethod val) { TradSesMethod = val; }
        public TradSesMethod Get(TradSesMethod val) { GetField(val); return val; }
        public bool IsSet(TradSesMethod val) { return IsSetTradSesMethod(); }
        public bool IsSetTradSesMethod() { return IsSetField(Tags.TradSesMethod); }

        public TradSesMode TradSesMode
        {
            get
            {
                TradSesMode val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesMode val) { TradSesMode = val; }
        public TradSesMode Get(TradSesMode val) { GetField(val); return val; }
        public bool IsSet(TradSesMode val) { return IsSetTradSesMode(); }
        public bool IsSetTradSesMode() { return IsSetField(Tags.TradSesMode); }

        public UnsolicitedIndicator UnsolicitedIndicator
        {
            get
            {
                UnsolicitedIndicator val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(UnsolicitedIndicator val) { UnsolicitedIndicator = val; }
        public UnsolicitedIndicator Get(UnsolicitedIndicator val) { GetField(val); return val; }
        public bool IsSet(UnsolicitedIndicator val) { return IsSetUnsolicitedIndicator(); }
        public bool IsSetUnsolicitedIndicator() { return IsSetField(Tags.UnsolicitedIndicator); }

        public TradSesStatus TradSesStatus
        {
            get
            {
                TradSesStatus val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesStatus val) { TradSesStatus = val; }
        public TradSesStatus Get(TradSesStatus val) { GetField(val); return val; }
        public bool IsSet(TradSesStatus val) { return IsSetTradSesStatus(); }
        public bool IsSetTradSesStatus() { return IsSetField(Tags.TradSesStatus); }

        public TradSesStatusRejReason TradSesStatusRejReason
        {
            get
            {
                TradSesStatusRejReason val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesStatusRejReason val) { TradSesStatusRejReason = val; }
        public TradSesStatusRejReason Get(TradSesStatusRejReason val) { GetField(val); return val; }
        public bool IsSet(TradSesStatusRejReason val) { return IsSetTradSesStatusRejReason(); }
        public bool IsSetTradSesStatusRejReason() { return IsSetField(Tags.TradSesStatusRejReason); }

        public TradSesStartTime TradSesStartTime
        {
            get
            {
                TradSesStartTime val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesStartTime val) { TradSesStartTime = val; }
        public TradSesStartTime Get(TradSesStartTime val) { GetField(val); return val; }
        public bool IsSet(TradSesStartTime val) { return IsSetTradSesStartTime(); }
        public bool IsSetTradSesStartTime() { return IsSetField(Tags.TradSesStartTime); }

        public TradSesOpenTime TradSesOpenTime
        {
            get
            {
                TradSesOpenTime val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesOpenTime val) { TradSesOpenTime = val; }
        public TradSesOpenTime Get(TradSesOpenTime val) { GetField(val); return val; }
        public bool IsSet(TradSesOpenTime val) { return IsSetTradSesOpenTime(); }
        public bool IsSetTradSesOpenTime() { return IsSetField(Tags.TradSesOpenTime); }

        public TradSesPreCloseTime TradSesPreCloseTime
        {
            get
            {
                TradSesPreCloseTime val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesPreCloseTime val) { TradSesPreCloseTime = val; }
        public TradSesPreCloseTime Get(TradSesPreCloseTime val) { GetField(val); return val; }
        public bool IsSet(TradSesPreCloseTime val) { return IsSetTradSesPreCloseTime(); }
        public bool IsSetTradSesPreCloseTime() { return IsSetField(Tags.TradSesPreCloseTime); }

        public TradSesCloseTime TradSesCloseTime
        {
            get
            {
                TradSesCloseTime val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesCloseTime val) { TradSesCloseTime = val; }
        public TradSesCloseTime Get(TradSesCloseTime val) { GetField(val); return val; }
        public bool IsSet(TradSesCloseTime val) { return IsSetTradSesCloseTime(); }
        public bool IsSetTradSesCloseTime() { return IsSetField(Tags.TradSesCloseTime); }

        public TradSesEndTime TradSesEndTime
        {
            get
            {
                TradSesEndTime val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TradSesEndTime val) { TradSesEndTime = val; }
        public TradSesEndTime Get(TradSesEndTime val) { GetField(val); return val; }
        public bool IsSet(TradSesEndTime val) { return IsSetTradSesEndTime(); }
        public bool IsSetTradSesEndTime() { return IsSetField(Tags.TradSesEndTime); }

        public TotalVolumeTraded TotalVolumeTraded
        {
            get
            {
                TotalVolumeTraded val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(TotalVolumeTraded val) { TotalVolumeTraded = val; }
        public TotalVolumeTraded Get(TotalVolumeTraded val) { GetField(val); return val; }
        public bool IsSet(TotalVolumeTraded val) { return IsSetTotalVolumeTraded(); }
        public bool IsSetTotalVolumeTraded() { return IsSetField(Tags.TotalVolumeTraded); }

        public Text Text
        {
            get
            {
                Text val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(Text val) { Text = val; }
        public Text Get(Text val) { GetField(val); return val; }
        public bool IsSet(Text val) { return IsSetText(); }
        public bool IsSetText() { return IsSetField(Tags.Text); }

        public EncodedTextLen EncodedTextLen
        {
            get
            {
                EncodedTextLen val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(EncodedTextLen val) { EncodedTextLen = val; }
        public EncodedTextLen Get(EncodedTextLen val) { GetField(val); return val; }
        public bool IsSet(EncodedTextLen val) { return IsSetEncodedTextLen(); }
        public bool IsSetEncodedTextLen() { return IsSetField(Tags.EncodedTextLen); }

        public EncodedText EncodedText
        {
            get
            {
                EncodedText val = new();
                GetField(val);
                return val;
            }
            set  => SetField(value);
        }

        public void Set(EncodedText val) { EncodedText = val; }
        public EncodedText Get(EncodedText val) { GetField(val); return val; }
        public bool IsSet(EncodedText val) { return IsSetEncodedText(); }
        public bool IsSetEncodedText() { return IsSetField(Tags.EncodedText); }
    }
}
