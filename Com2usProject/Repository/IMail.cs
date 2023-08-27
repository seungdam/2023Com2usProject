namespace Com2usProject.Repository;

public interface IMail
{
    public Task<CSCommon.ErrorCode> UpdateMailBox(int PlayerId);
}
