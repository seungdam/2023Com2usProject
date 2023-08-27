namespace Com2usProject.Service;

public interface IMail
{
    public Task<CSCommon.ErrorCode> UpdateMailBox(int PlayerId);
}
