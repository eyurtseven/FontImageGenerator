using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontImageGenerator
{
    public class FontInfo
    {
        public string FontName { get; set; }
        public string FontFile { get; set; }
        public List<FontImageInfo> FontImageInfo { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
