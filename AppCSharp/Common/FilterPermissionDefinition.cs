using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Common
{
    public class FilterPermissionDefinition
    {
        public string Name { get; private set; }
        public long FieldTypeId { get; private set; }
        public IdFilter IdFilter { get; private set; }
        public GuidFilter GuidFilter { get; private set; }
        public DecimalFilter DecimalFilter { get; private set; }
        public LongFilter LongFilter { get; private set; }
        public DateFilter DateFilter { get; private set; }
        public StringFilter StringFilter { get; private set; }

        public FilterPermissionDefinition(string name, long FieldTypeId, long PermissionOperatorId, string value)
        {
            this.Name = name;
            this.FieldTypeId = FieldTypeId;
            if (FieldTypeId == FieldTypeEnum.ID.Id)
            {
                if (IdFilter == null) IdFilter = new IdFilter();
                if (long.TryParse(value, out long result))
                {
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_EQ.Id)
                        IdFilter.Equal = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_NE.Id)
                        IdFilter.NotEqual = result;
                    if (PermissionOperatorId == PermissionOperatorEnum.ID_IN.Id)
                    {
                        if (IdFilter.In == null) IdFilter.In = new List<long>
    ();
    IdFilter.In.Add(result);
    }
    if (PermissionOperatorId == PermissionOperatorEnum.ID_NI.Id)
    {
    if (IdFilter.NotIn == null) IdFilter.NotIn = new List<long>
        ();
        IdFilter.NotIn.Add(result);
        }
        }
        }

        if (FieldTypeId == FieldTypeEnum.GUID.Id)
        {
        if (GuidFilter == null) GuidFilter = new GuidFilter();
        if (Guid.TryParse(value, out Guid result))
        {
        if (PermissionOperatorId == PermissionOperatorEnum.GUID_EQ.Id)
        GuidFilter.Equal = result;
        if (PermissionOperatorId == PermissionOperatorEnum.GUID_NE.Id)
        GuidFilter.NotEqual = result;
        if (PermissionOperatorId == PermissionOperatorEnum.GUID_IN.Id)
        {
        if (GuidFilter.In == null) GuidFilter.In = new List<Guid>
            ();
            GuidFilter.In.Add(result);
            }
            if (PermissionOperatorId == PermissionOperatorEnum.GUID_NI.Id)
            {
            if (GuidFilter.NotIn == null) GuidFilter.NotIn = new List<Guid>
                ();
                GuidFilter.NotIn.Add(result);
                }
                }
                }

                if (FieldTypeId == FieldTypeEnum.STRING.Id)
                {
                if (StringFilter == null) StringFilter = new StringFilter();
                if (string.IsNullOrWhiteSpace(value))
                {
                string result = value;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_EQ.Id)
                StringFilter.Equal = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_NE.Id)
                StringFilter.NotEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_SW.Id)
                StringFilter.StartWith = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_NSW.Id)
                StringFilter.NotStartWith = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_EW.Id)
                StringFilter.EndWith = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_NEW.Id)
                StringFilter.NotEndWith = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_CT.Id)
                StringFilter.Contain = result;
                if (PermissionOperatorId == PermissionOperatorEnum.STRING_NC.Id)
                StringFilter.NotContain = result;
                }
                }

                if (FieldTypeId == FieldTypeEnum.LONG.Id)
                {
                if (LongFilter == null) LongFilter = new LongFilter();
                if (long.TryParse(value, out long result))
                {
                if (PermissionOperatorId == PermissionOperatorEnum.LONG_EQ.Id)
                LongFilter.Equal = result;
                if (PermissionOperatorId == PermissionOperatorEnum.LONG_NE.Id)
                LongFilter.NotEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.LONG_GT.Id)
                if (LongFilter.Greater == null || LongFilter.Greater > result) LongFilter.Greater = result;
                if (PermissionOperatorId == PermissionOperatorEnum.LONG_GE.Id)
                if (LongFilter.GreaterEqual == null || LongFilter.GreaterEqual >= result) LongFilter.GreaterEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.LONG_LT.Id)
                if (LongFilter.Less == null || LongFilter.Less < result) LongFilter.Less = result;
                if (PermissionOperatorId == PermissionOperatorEnum.LONG_LE.Id)
                if (LongFilter.LessEqual == null || LongFilter.LessEqual <= result) LongFilter.LessEqual = result;
                }
                }

                if (FieldTypeId == FieldTypeEnum.DECIMAL.Id)
                {
                if (DecimalFilter == null) DecimalFilter = new DecimalFilter();
                if (decimal.TryParse(value, out decimal result))
                {
                if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_EQ.Id)
                DecimalFilter.Equal = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_NE.Id)
                DecimalFilter.NotEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_GT.Id)
                if (DecimalFilter.Greater == null || DecimalFilter.Greater > result) DecimalFilter.Greater = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_GE.Id)
                if (DecimalFilter.GreaterEqual == null || DecimalFilter.GreaterEqual >= result) DecimalFilter.GreaterEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_LT.Id)
                if (DecimalFilter.Less == null || DecimalFilter.Less < result) DecimalFilter.Less = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DECIMAL_LE.Id)
                if (DecimalFilter.LessEqual == null || DecimalFilter.LessEqual <= result) DecimalFilter.LessEqual = result;
                }
                }

                if (FieldTypeId == FieldTypeEnum.DATE.Id)
                {
                if (DateFilter == null) DateFilter = new DateFilter();
                if (DateTime.TryParse(value, out DateTime result))
                {
                if (PermissionOperatorId == PermissionOperatorEnum.DATE_EQ.Id)
                DateFilter.Equal = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DATE_NE.Id)
                DateFilter.NotEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DATE_GT.Id)
                if (DateFilter.Greater == null || DateFilter.Greater > result) DateFilter.Greater = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DATE_GE.Id)
                if (DateFilter.GreaterEqual == null || DateFilter.GreaterEqual >= result) DateFilter.GreaterEqual = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DATE_LT.Id)
                if (DateFilter.Less == null || DateFilter.Less < result) DateFilter.Less = result;
                if (PermissionOperatorId == PermissionOperatorEnum.DATE_LE.Id)
                if (DateFilter.LessEqual == null || DateFilter.LessEqual <= result) DateFilter.LessEqual = result;
                }
                }
                }
                }

                public class FieldTypeEnum
                {
                public static GenericEnum ID = new GenericEnum { Id = 1, Code = "ID", Name = "ID" };
                public static GenericEnum STRING = new GenericEnum { Id = 2, Code = "STRING", Name = "STRING" };
                public static GenericEnum LONG = new GenericEnum { Id = 3, Code = "LONG", Name = "LONG" };
                public static GenericEnum DECIMAL = new GenericEnum { Id = 4, Code = "DECIMAL", Name = "DATE" };
                public static GenericEnum DATE = new GenericEnum { Id = 5, Code = "DATE", Name = "DATE" };
                public static GenericEnum DOUBLE = new GenericEnum { Id = 6, Code = "DOUBLE", Name = "DOUBLE" };
                public static GenericEnum INT = new GenericEnum { Id = 7, Code = "INT", Name = "INT" };
                public static GenericEnum GUID = new GenericEnum { Id = 8, Code = "GUID", Name = "GUID" };


                public static List<GenericEnum>
                    List = new List<GenericEnum>
                        ()
                        {
                        ID, STRING, LONG, DECIMAL, DATE, DOUBLE, INT, GUID,
                        };
                        }

                        public class PermissionOperatorEnum
                        {
                        public static GenericEnum ID_EQ = new GenericEnum { Id = 101, Code = "ID_EQ", Name = "=" };
                        public static GenericEnum ID_NE = new GenericEnum { Id = 102, Code = "ID_NE", Name = "!=" };
                        public static GenericEnum ID_IN = new GenericEnum { Id = 103, Code = "ID_IN", Name = "Chứa" };
                        public static GenericEnum ID_NI = new GenericEnum { Id = 104, Code = "ID_NI", Name = "Không chứa" };

                        public static GenericEnum STRING_NE = new GenericEnum { Id = 201, Code = "STRING_NE", Name = "!=" };
                        public static GenericEnum STRING_EQ = new GenericEnum { Id = 202, Code = "STRING_EQ", Name = "=" };
                        public static GenericEnum STRING_SW = new GenericEnum { Id = 203, Code = "STRING_SW", Name = "Bắt đầu bởi" };
                        public static GenericEnum STRING_NSW = new GenericEnum { Id = 204, Code = "STRING_NSW", Name = "Không bắt đầu bởi" };
                        public static GenericEnum STRING_EW = new GenericEnum { Id = 205, Code = "STRING_EW", Name = "Kết thúc bởi" };
                        public static GenericEnum STRING_NEW = new GenericEnum { Id = 206, Code = "STRING_NEW", Name = "Không kết thúc bởi" };
                        public static GenericEnum STRING_CT = new GenericEnum { Id = 207, Code = "STRING_CT", Name = "Chứa" };
                        public static GenericEnum STRING_NC = new GenericEnum { Id = 208, Code = "STRING_NC", Name = "Không chứa" };

                        public static GenericEnum LONG_GT = new GenericEnum { Id = 301, Code = "LONG_GT", Name = ">" };
                        public static GenericEnum LONG_GE = new GenericEnum { Id = 302, Code = "LONG_GE", Name = ">=" };
                        public static GenericEnum LONG_LT = new GenericEnum { Id = 303, Code = "LONG_LT", Name = "<" };
                        public static GenericEnum LONG_LE = new GenericEnum { Id = 304, Code = "LONG_LE", Name = "<=" };
                        public static GenericEnum LONG_NE = new GenericEnum { Id = 305, Code = "LONG_NE", Name = "!=" };
                        public static GenericEnum LONG_EQ = new GenericEnum { Id = 306, Code = "LONG_EQ", Name = "=" };

                        public static GenericEnum DECIMAL_GT = new GenericEnum { Id = 401, Code = "DECIMAL_GT", Name = ">" };
                        public static GenericEnum DECIMAL_GE = new GenericEnum { Id = 402, Code = "DECIMAL_GE", Name = ">=" };
                        public static GenericEnum DECIMAL_LT = new GenericEnum { Id = 403, Code = "DECIMAL_LT", Name = "<" };
                        public static GenericEnum DECIMAL_LE = new GenericEnum { Id = 404, Code = "DECIMAL_LE", Name = "<=" };
                        public static GenericEnum DECIMAL_NE = new GenericEnum { Id = 405, Code = "DECIMAL_NE", Name = "!=" };
                        public static GenericEnum DECIMAL_EQ = new GenericEnum { Id = 406, Code = "DECIMAL_EQ", Name = "=" };

                        public static GenericEnum DATE_GT = new GenericEnum { Id = 501, Code = "DATE_GT", Name = ">" };
                        public static GenericEnum DATE_GE = new GenericEnum { Id = 502, Code = "DATE_GE", Name = ">=" };
                        public static GenericEnum DATE_LT = new GenericEnum { Id = 503, Code = "DATE_LT", Name = "<" };
                        public static GenericEnum DATE_LE = new GenericEnum { Id = 504, Code = "DATE_LE", Name = "<=" };
                        public static GenericEnum DATE_NE = new GenericEnum { Id = 505, Code = "DATE_NE", Name = "!=" };
                        public static GenericEnum DATE_EQ = new GenericEnum { Id = 506, Code = "DATE_EQ", Name = "=" };

                        public static GenericEnum GUID_EQ = new GenericEnum { Id = 601, Code = "GUID_EQ", Name = "=" };
                        public static GenericEnum GUID_NE = new GenericEnum { Id = 602, Code = "GUID_NE", Name = "!=" };
                        public static GenericEnum GUID_IN = new GenericEnum { Id = 603, Code = "GUID_IN", Name = "Chứa" };
                        public static GenericEnum GUID_NI = new GenericEnum { Id = 604, Code = "GUID_NI", Name = "Không chứa" };


                        public static List<GenericEnum>
                            PermissionOperatorEnumForID = new List<GenericEnum>
                                ()
                                {
                                ID_EQ, ID_NE, ID_IN, ID_NI,
                                };

                                public static List<GenericEnum>
                                    PermissionOperatorEnumForLONG = new List<GenericEnum>
                                        ()
                                        {
                                        LONG_GT, LONG_GE, LONG_LT, LONG_LE, LONG_NE, LONG_EQ,
                                        };

                                        public static List<GenericEnum>
                                            PermissionOperatorEnumForDECIMAL = new List<GenericEnum>
                                                ()
                                                {
                                                DECIMAL_GT, DECIMAL_GE, DECIMAL_LT, DECIMAL_LE, DECIMAL_NE, DECIMAL_EQ,
                                                };

                                                public static List<GenericEnum>
                                                    PermissionOperatorEnumForDATE = new List<GenericEnum>
                                                        ()
                                                        {
                                                        DATE_GT, DATE_GE, DATE_LT, DATE_LE, DATE_NE, DATE_EQ,
                                                        };

                                                        public static List<GenericEnum>
                                                            PermissionOperatorEnumForSTRING = new List<GenericEnum>
                                                                ()
                                                                {
                                                                STRING_NE, STRING_EQ, STRING_SW, STRING_NSW, STRING_EW, STRING_NEW, STRING_CT, STRING_NC,
                                                                };

                                                                public static List<GenericEnum>
                                                                    PermissionOperatorEnumForGUID = new List<GenericEnum>
                                                                        ()
                                                                        {
                                                                        GUID_EQ, GUID_NE, GUID_IN, GUID_NI,
                                                                        };
                                                                        }
                                                                        }
