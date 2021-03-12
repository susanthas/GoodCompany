using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static cinventory.web.Utils;

namespace cinventory.web
{
    public class Utils
    {
        // I would use the following class to send the JSON response to the client so we always know
        // the response has 3 components (code, message, data). I would set code > 0 when the result is a success
        // else failure. That makes it easier for me to process the result. 
        // But for this sample code, I won't be using this
        public class JsonResponse
        {
            public int code { get; set; }
            public string message { get; set; }
            public dynamic data { get; set; }

            public JsonResponse()
            {
                this.code = 0;
                this.message = "Not processed";
            }

            public JsonResponse(int code, string message)
            {
                this.code = code;
                this.message = message;
            }

            public JsonResponse(int code, string message, dynamic data)
            {
                this.code = code;
                this.message = message;
                this.data = data;
            }
        }
    }

    public class Validation
    {
        public static bool TryValidate(object objectToValidate, ref JsonResponse response)
        {
            List<ValidationResult> vr = new List<ValidationResult>();
            ValidationContext ctx = new ValidationContext(objectToValidate);
            if (!Validator.TryValidateObject(objectToValidate, ctx, vr))
            {
                response.code = 0;
                response.message = "There are errors!";
                response.data = vr;
                return false;
            }
            else
            {
                response.code = 1;
                return true;
            }
        }
    }
}
