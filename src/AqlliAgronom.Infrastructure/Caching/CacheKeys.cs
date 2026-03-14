namespace AqlliAgronom.Infrastructure.Caching;

public static class CacheKeys
{
    public static string User(Guid id) => $"user:{id}";
    public static string UserByPhone(string phone) => $"user:phone:{phone}";
    public static string Session(long chatId) => $"session:telegram:{chatId}";
    public static string Product(Guid id) => $"product:{id}";
    public static string ProductList(int page, int size, string? search, string? category)
        => $"products:list:{page}:{size}:{search ?? "all"}:{category ?? "all"}";
    public static string KnowledgeEntry(Guid id) => $"knowledge:{id}";
    public static string KnowledgeSearch(string queryHash) => $"knowledge:search:{queryHash}";

    public static string ComputeHash(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToHexString(System.Security.Cryptography.MD5.HashData(bytes));
    }
}
