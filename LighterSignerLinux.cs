using System.Runtime.InteropServices;

namespace LighterTest;

public static class LighterSignerLinux
{
    private static string? MarshalString(IntPtr ptr)
    {
        Console.WriteLine($"MarshalString: {ptr} - Start");
        var result = ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
        Console.WriteLine($"MarshalString: {ptr} - End");
        return result;
    }
    
    private static string? MarshalAndFreeString(IntPtr ptr)
    {
        Console.WriteLine($"MarshalAndFreeString: {ptr} - Start");
        if (ptr == IntPtr.Zero)
        {
            Console.WriteLine($"MarshalAndFreeString: {ptr} - End");
            return null;
        }
        var value = Marshal.PtrToStringUTF8(ptr);
        Free(ptr);
        Console.WriteLine($"MarshalAndFreeString: {ptr} - End");
        return value;
    }
    
    public static string? CreateClient(
        string url,
        string privateKey,
        int chainId,
        int apiKeyIndex,
        long accountIndex)
    {
        Console.WriteLine($"CreateClient: {url} - Start");
        var ptr = LighterNativeLinux.CreateClient(url, privateKey, chainId, apiKeyIndex, accountIndex);
        var t = MarshalAndFreeString(ptr);
        Console.WriteLine($"CreateClient: {url} - Result: {t}");
        Console.WriteLine($"CreateClient: {url} - End");
        return t;
    }
    
    private static void Free(IntPtr ptr)
    {
        Console.WriteLine($"Free: {ptr} - Start");
        LighterNativeLinux.Free(ptr);
        Console.WriteLine($"Free: {ptr} - End");
    }
}