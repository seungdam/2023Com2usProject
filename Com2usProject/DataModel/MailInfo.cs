using SqlKata.Execution;
using MySqlConnector;
namespace Com2usProject.DataModel;

public class MailInfo
{
    public int MailIndex { get; set; }

    public int ItemCode { get; set; }
    public int ItemCount { get; set; }
    public DateTime ReceiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}
