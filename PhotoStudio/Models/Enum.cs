using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoStudio.Models
{
    public class Enum
    {

        public enum RESULT_CODE
        {
            NONE = 0,
            SUCCESS = 1,
            IMAGE_FORMAT_ERROR = 2,
            IMAGE_SIZE_ERROR = 3,
            ERROR = 10
        }
    }
}