namespace Iguagile
{
    public enum MessageTypes : byte
    {
        NewConnection,
        ExitConnection,
        Instantiate,
        Destroy,
        RequestObjectControlAuthority,
        TransferObjectControlAuthority,
        MigrateHost,
        Register,
        Transform,
        Rpc
    }
}