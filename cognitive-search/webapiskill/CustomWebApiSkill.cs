using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Arinti.Templates.CognitiveSearch.Helpers;

namespace Arinti.Templates.CognitiveSearch
{
    public static class CustomWebApiSkill
    {
        [FunctionName("CustomWebApiSkill")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null)
            {
                return new BadRequestObjectResult($"GetMetaData - Invalid request record array.");
            }


            WebApiSkillResponse response = WebApiSkillHelpers.ProcessRequestRecords("GetMetaData", requestRecords,
                (inRecord, outRecord) =>
                {
                    var input1 = inRecord.Data["field_1"].ToString();
                    var input2 = inRecord.Data["field_2"].ToString();

                    outRecord.Data["output_field_1"] = input1;
                    outRecord.Data["output_field_2"] = input2;
                    return outRecord;
                });

            return (ActionResult)new OkObjectResult(response);
        }
    }
}
