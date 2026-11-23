using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IStatisticsService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<RankingEntry> GetRanking(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    PlayerMatchRecord GetMyRecord(string sessionToken);
}
