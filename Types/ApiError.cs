public class ApiError
{
    public string ErrorCode { get; set; } = "unspecied_error";
}

public static class ApiErrorCodes
{
    public static string ItemDuplicated = "item_duplicated";
}