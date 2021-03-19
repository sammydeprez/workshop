using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Arinti.Templates.CognitiveSearch.Helpers
{
    public class WebApiSkillRequest
    {
        public List<WebApiRequestRecord> Values { get; set; } = new List<WebApiRequestRecord>();
    }

    public class WebApiSkillResponse
    {
        public List<WebApiResponseRecord> Values { get; set; } = new List<WebApiResponseRecord>();
    }

    public class WebApiRequestRecord
    {
        public string RecordId { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }

    public class WebApiResponseRecord
    {
        public string RecordId { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public List<WebApiErrorWarningContract> Errors { get; set; } = new List<WebApiErrorWarningContract>();
        public List<WebApiErrorWarningContract> Warnings { get; set; } = new List<WebApiErrorWarningContract>();
    }

    public class WebApiErrorWarningContract
    {
        public string Message { get; set; }
    }
    public class WebApiResponseError
    {
        public string Message { get; set; }
    }

    public class WebApiResponseWarning
    {
        public string Message { get; set; }
    }

    public class WebApiEnricherResponse
    {
        public List<WebApiResponseRecord> Values { get; set; }
    }

    static class WebApiSkillHelpers
    {
        public static IEnumerable<WebApiRequestRecord> GetRequestRecords(HttpRequest req)
        {
            string jsonRequest = new StreamReader(req.Body).ReadToEnd();
            WebApiSkillRequest docs = JsonConvert.DeserializeObject<WebApiSkillRequest>(jsonRequest);
            return docs.Values;
        }

        public static WebApiSkillResponse ProcessRequestRecords(string functionName, IEnumerable<WebApiRequestRecord> requestRecords, Func<WebApiRequestRecord, WebApiResponseRecord, WebApiResponseRecord> processRecord)
        {
            WebApiSkillResponse response = new WebApiSkillResponse();

            foreach (WebApiRequestRecord inRecord in requestRecords)
            {
                WebApiResponseRecord outRecord = new WebApiResponseRecord() { RecordId = inRecord.RecordId };

                try
                {
                    outRecord = processRecord(inRecord, outRecord);
                }
                catch (Exception e)
                {
                    outRecord.Errors.Add(new WebApiErrorWarningContract() { Message = $"{functionName} - Error processing the request record : {e.ToString() }" });
                }
                response.Values.Add(outRecord);
            }

            return response;
        }

        public static async Task<WebApiSkillResponse> ProcessRequestRecordsAsync(string functionName, IEnumerable<WebApiRequestRecord> requestRecords, Func<WebApiRequestRecord, WebApiResponseRecord, Task<WebApiResponseRecord>> processRecord)
        {
            WebApiSkillResponse response = new WebApiSkillResponse();

            foreach (WebApiRequestRecord inRecord in requestRecords)
            {
                WebApiResponseRecord outRecord = new WebApiResponseRecord() { RecordId = inRecord.RecordId };

                try
                {
                    outRecord = await processRecord(inRecord, outRecord);
                }
                catch (Exception e)
                {
                    outRecord.Errors.Add(new WebApiErrorWarningContract() { Message = $"{functionName} - Error processing the request record : {e.ToString() }" });
                }
                response.Values.Add(outRecord);
            }

            return response;
        }

    }
}
