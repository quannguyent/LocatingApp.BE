using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class PlaceGroupEnum
    {
        public static GenericEnum Transport = new GenericEnum { Id = 1, Code = "Transport", Name = "Thuộc vận tải" };
        public static GenericEnum Recreation = new GenericEnum { Id = 2, Code = "Recreation", Name = "Thuộc giải trí" };
        public static GenericEnum Religion = new GenericEnum { Id = 3, Code = "Religion", Name = "Thuộc tôn giáo" };
        public static GenericEnum Education = new GenericEnum { Id = 4, Code = "Education", Name = "Thuộc giáo dục" };
        public static GenericEnum Entertainment = new GenericEnum { Id = 5, Code = "Entertainment", Name = "Thuộc vui chơi giải trí" };
        public static GenericEnum Nightlife = new GenericEnum { Id = 6, Code = "Nightlife", Name = "Thuộc hoạt động ban đêm" };
        public static GenericEnum Food = new GenericEnum { Id = 7, Code = "Food", Name = "Thuộc ẩm thực" };
        public static GenericEnum Government = new GenericEnum { Id = 8, Code = "Government", Name = "Thuộc chính phủ" };
        public static GenericEnum Professional = new GenericEnum { Id = 9, Code = "Professional", Name = "Thuộc tổ chức chuyên nghiệp" };
        public static GenericEnum Finance = new GenericEnum { Id = 10, Code = "Finance", Name = "Thuộc tài chính" };
        public static GenericEnum Health = new GenericEnum { Id = 11, Code = "Health", Name = "Thuộc sức khỏe" };
        public static GenericEnum Retail = new GenericEnum { Id = 12, Code = "Retail", Name = "Thuộc bán lẻ" };
        public static GenericEnum Accommodation = new GenericEnum { Id = 13, Code = "Accommodation", Name = "Thuộc nhà ở" };
        public static GenericEnum Industry = new GenericEnum { Id = 14, Code = "Industry", Name = "Thuộc công nghiệp" };
        public static GenericEnum Natural = new GenericEnum { Id = 15, Code = "Natural", Name = "Thuộc tự nhiên" };

        public static List<GenericEnum> PlaceGroupEnumList = new List<GenericEnum>
        {
            Transport, Recreation, Religion, 
            Education, Entertainment, Nightlife, 
            Food, Government, Professional, Finance, 
            Health, Retail, Accommodation, Industry, Natural
        };
    }
}
