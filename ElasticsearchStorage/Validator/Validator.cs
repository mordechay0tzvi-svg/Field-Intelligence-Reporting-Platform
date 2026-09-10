using System.Text.Json;
using Models;
namespace Validator;
public interface IValidating
{
    bool Validate(FieldReport report);
}
public class Validating : IValidating
{
    private readonly List<string> _validPriority = new(){ "Low", "Medium", "High", "Critical" };
    private readonly List<string> _validType = new(){"Observation","Movement","Meeting","Access","Communication","Logistics","Incident"};
    public bool Validate(FieldReport report)
    {
        if (!_validPriority.Contains(report.Priority))
        {
            return false;
        }
        if (!_validType.Contains(report.ReportType))
        {
            return false;
        }
        if (report.SubjectId == null && report.SubjectType != null)
        {
            return false;
        }
        if (report.SubjectId != null && report.SubjectType == null)
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.ReportId))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.ReportType))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.Sector))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.Theater))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.AgentId))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.Unit))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.Location))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.Priority))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.SourceType))
        {
            return false;
        }
        if (String.IsNullOrWhiteSpace(report.Message))
        {
            return false;
        }
        return true;
    }
}