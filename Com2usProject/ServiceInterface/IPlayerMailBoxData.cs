using Com2usProject.DataModel;
using CSCommon;

namespace Com2usProject.ServiceInterface;
public interface IPlayerMailBoxData
{
    public Task<CSCommon.ErrorCode> RecieveMail(int PlayerId,int MailIndex);

    public Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadMail(int PlayerId, int MailPage);
}
