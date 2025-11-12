using Integration.Abstract.Helpers;
using Integration.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Template.Utilities
{
    /// <summary>
    /// This class is an optional utility class that implementers may find useful. It allows you to queue up APICalls and run them in parallel in batches. For example, you are sending 
    /// a product to the external system and that product has variants which require separate API calls, you could run each variant call sequentially, but that will be slow. It is much 
    /// faster to build a list of variant API calls, then use this method to execute them all in groups. 
    /// If no batchSize is specified, the iPaaS setting for "Concurrent Batch Executions" will be used.
    /// 
    /// A sample usage of this might look like the following. In this case, we have a list of variants that need to each make a PUT call to the API. The code below shows how to queue up 
    /// a list of APICalls, execute them using this BatchExecution model, then save the results into the responseVariants variable:
    /// 
    /// var variantPutAndPostCalls = new List<APICall>();
    ///  var responseVariants = new List<Variants>()
    ///  foreach (var variant in variants)
    ///  {
    ///	    variantPutAndPostCalls.Add(await wrapper.Product_PUT_ConstructAPICall(variant));
    ///  }
    ///var batchExecutionVariants = new BatchExecution(variantPutAndPostCalls, stopProcessingAfterException: true);
    ///try
    ///{
    ///    var successfulResponses = await batchExecutionVariants.ExecuteAPICallsInBatch<CatalogDataProductInterface>();
    ///    if (successfulResponses != null)
    ///        responseVariants.AddRange(successfulResponses);
    ///}
    ///catch (Exception ex)
    ///{
    ///    throw new ExpectedException(ex.Message) { SavedExternalData = responseVariants };
    ///}
    /// </summary>
    public class BatchExecution
    {
        public List<APICall> ApiCalls;
        public int BatchSize;
        public bool StopProcessingAfterException;

        public List<object> SuccessfulResponses;
        public List<Exception> ExceptionResponse;

        public BatchExecution(List<APICall> apiCalls, int? batchSize = null, bool stopProcessingAfterException = true)
        {
            if (CallContext.DebugMode)
            {
                if (apiCalls != null && apiCalls.Count > 0)
                    apiCalls[0].Connection.Logger.Log_Technical("D", "BatchExecution.ctor", $"Since we are in DebugMode, batch execution will be stopped by setting the batchSize to 1");
                batchSize = 1;
            }

            //If no batch size was supplied, check if we have one locally. If 
            if (!batchSize.HasValue)
            {
                var batchSizeSettingStr = ConversionFunctions.ContextConnection.Settings.GetSetting("Concurrent Batch Executions");
                if (batchSizeSettingStr != null && int.TryParse(batchSizeSettingStr, out int batchSizeSetting))
                    batchSize = batchSizeSetting;
                else
                    batchSize = 10;
            }

            ApiCalls = apiCalls;
            BatchSize = batchSize.Value;
            StopProcessingAfterException = stopProcessingAfterException;
            SuccessfulResponses = new List<object>();
            ExceptionResponse = new List<Exception>();
        }

        public async Task<List<T>> ExecuteAPICallsInBatch<T>()
        {
            int numberOfBatches = (int)Math.Ceiling((double)ApiCalls.Count() / BatchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var batchStartDT = DateTime.Now;
                var responses = new List<object>();
                var currentApiCalls = ApiCalls.Skip(i * BatchSize).Take(BatchSize);
                var tasks = currentApiCalls.Select(x => x.ProcessBatchRequestAsync());
                responses.AddRange(await Task.WhenAll(tasks));

                foreach (var response in responses)
                {
                    if (response is Exception)
                        ExceptionResponse.Add((Exception)response);
                    else
                        SuccessfulResponses.Add(response);
                }

                if (HasExceptions())
                {
                    if (StopProcessingAfterException)
                        throw new Exception($"Received an exception during batch run on of {currentApiCalls.First().Action}. {FirstExceptionMessage()}");
                    else
                        ApiCalls[0].Connection.Logger.Log_Technical("D", "BatchExecution.ExecuteAPICallsInBatch", $"Errors during batch, but processing will continue. (Most recent: {currentApiCalls.First().Action}:{FirstExceptionMessage()}). Remaining: {ApiCalls.Count() - (i * BatchSize)}. Total Successful calls: {SuccessfulResponses.Count}, Total Errors: {ExceptionResponse.Count}");
                }
                else if (BatchSize > 1) //Include details about each size, unless the size is 1 (i.e. we are in DebugMode), in which case it is unnecessary
                    ApiCalls[0].Connection.Logger.Log_Technical("D", "BatchExecution.ExecuteAPICallsInBatch", $"Successfully completed batch of {BatchSize} in {(DateTime.Now - batchStartDT).TotalMilliseconds} ms (Most recent: {currentApiCalls.First().Action}). Remaining: {ApiCalls.Count() - (i * BatchSize)}");
            }

            //if (ExceptionResponse.Count > 0)
            //{
            //    foreach (var exception in ExceptionResponse)
            //    {
            //        Console.WriteLine($"Exception: {exception.Message}");
            //    }
            //}

            return SuccessfulResponses.Cast<T>().ToList();
        }

        public bool HasExceptions()
        {
            return ExceptionResponse.Count() > 0;
        }

        public string FirstExceptionMessage()
        {
            if (ExceptionResponse != null || ExceptionResponse.Count == 0)
                return null;
            return ExceptionResponse[0].Message;
        }

        //Return a typed list
        public List<T> GetSuccessfulResponses<T>()
        {
            return SuccessfulResponses.Cast<T>().ToList();
        }
    }
}
