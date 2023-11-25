using MainData.Entities;

namespace MainData.Constants;

public static class RequestTypeMetadata
{
    public static string GetRedirectPath(RequestType type)
    {
        var url = "/detail/";
        switch (type)
        {
            case RequestType.StatusCheck:
                return "asset-check" + url;
            case RequestType.Maintenance:
                return "maintenance" + url;
            case RequestType.Repairation:
                return "repairation" + url;
            case RequestType.Replacement:
                return "replacement" + url;
            case RequestType.Transportation:
                return "transportation" + url;
            case RequestType.InventoryCheck:
                return "inventory-check" + url;
            default:
                return string.Empty;
        }
    }
}