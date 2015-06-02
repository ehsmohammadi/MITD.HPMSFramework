using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;


namespace MITD.Core
{
    public class ModelValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                // Return the validation errors in the response body. 
                var errors = new StringBuilder();
                foreach (KeyValuePair<string, ModelState> keyValue in actionContext.ModelState)
                {
                    foreach (var e in keyValue.Value.Errors)
                    {
                        errors.Append( e.ErrorMessage);
                    }
                }

                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errors.ToString());
            }
        }
    }
}