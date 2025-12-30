namespace QMS.Application.Interfaces;

public interface IQMSHelperService
{
    // Sequence Number Generation
    Task<int> GetMaxSequenceAsync(int queueId, DateTime date);
    Task<int> CalculatePrioritySequenceAsync(int queueId, DateTime date);
    Task<int> CalculatePrioritySequenceKhamBenhAsync(int queueId, DateTime date);

    // Display Text Generation
    Task<string> GenerateDisplayTextAsync(int sequence, int? priority = null);
    Task<string> CalculateOrderForPriorityAsync(int queueId, string displayText, DateTime date);

    // Time Calculations
    Task<DateTime> CalculateEstimateTimeAsync(int queueId, DateTime createDate);
    Task<int> CalculateWaitingTimeAsync(int queueId);

    // Validation
    Task<bool> CheckPatientExistsInQueueAsync(int queueId, string patientCode, DateTime date);
    Task<bool> IsQueueManualAsync(int queueId);

    // Parameter Helpers
    Task<string> GetParameterAsync(string code, string defaultValue = "");
    Task<int> GetIntParameterAsync(string code, int defaultValue = 0);
    Task<bool> GetBoolParameterAsync(string code, bool defaultValue = false);
}