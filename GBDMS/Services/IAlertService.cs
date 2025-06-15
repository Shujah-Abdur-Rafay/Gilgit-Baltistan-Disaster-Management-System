using GBDMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBDMS.Services
{
    public interface IAlertService
    {
        Task<DisasterAlert> CreateAlertFromRiskAssessmentAsync(
            string disasterType, 
            string district, 
            double riskScore, 
            string riskParameters, 
            string createdBy = "Risk Assessment Model");

        Task<List<DisasterAlert>> GetActiveAlertsAsync();
        Task<List<DisasterAlert>> GetAlertsByDistrictAsync(string district);
        Task<bool> DeactivateAlertAsync(int alertId);
        Task SendAlertNotificationsAsync(DisasterAlert alert);
        string DetermineAlertSeverity(double riskScore);
        string GenerateAlertMessage(string disasterType, string district, string severity, double riskScore);
    }
}
