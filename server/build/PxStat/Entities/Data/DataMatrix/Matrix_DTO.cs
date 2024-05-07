using PxStat.Resources;
using System;

namespace PxStat.Data
{
    internal class Matrix_DTO
    {
        private string _mtrCode;
        private string _mtrTitle;
        private string _lngIsoCode;
        private string _cprValue;
        private string _frqCode;
        private string _frmVersion;
        private string _frqValue;
        private string _frmType;
        private string _cprCode;
        private string _cprUrl;

        public string MtrCode { get { return Sanitizer.PrepareJsonValue(_mtrCode); } internal set { _mtrCode = Sanitizer.PrepareJsonValue(value); } }
        public string MtrTitle { get { return Sanitizer.PrepareJsonValue(_mtrTitle); } internal set { _mtrTitle = Sanitizer.PrepareJsonValue(value); } }
        public string LngIsoCode { get { return Sanitizer.PrepareJsonValue(_lngIsoCode); } internal set { _lngIsoCode = Sanitizer.PrepareJsonValue(value); } }
        public string CprValue { get { return Sanitizer.PrepareJsonValue(_cprValue); } internal set { _cprValue = Sanitizer.PrepareJsonValue(value); } }
        public bool MtrOfficialFlag { get; set; }
        public string FrmType { get { return Sanitizer.PrepareJsonValue(_frmType); } internal set { _frmType = Sanitizer.PrepareJsonValue(value); } }
        public string FrmVersion { get { return Sanitizer.PrepareJsonValue(_frmVersion); } internal set { _frmVersion = Sanitizer.PrepareJsonValue(value); } }
        public string MtrNote { get; set; }
        public string MtrInput { get; internal set; }
        public string FrqCode { get { return Sanitizer.PrepareJsonValue(_frqCode); } internal set { _frqCode = Sanitizer.PrepareJsonValue(value); } }
        public string FrqValue { get { return Sanitizer.PrepareJsonValue(_frqValue); } internal set { _frqValue = Sanitizer.PrepareJsonValue(value); } }
        public string CprCode { get { return Sanitizer.PrepareJsonValue(_cprCode); } internal set { _cprCode = Sanitizer.PrepareJsonValue(value); } }
        public string LngIsoName { get; internal set; }
        public int RlsCode { get; internal set; }
        public DateTime DtgUpdateDatetime { get; internal set; }
        public string CcnUsernameUpdate { get; internal set; }
        public DateTime DtgCreateDatetime { get; internal set; }
        public string CcnUsername { get; internal set; }
        public string CprUrl { get { return Sanitizer.PrepareJsonValue(_cprUrl); } internal set { _cprUrl = Sanitizer.PrepareJsonValue(value); } }
        public int MtrId { get; set; }
    }
}
