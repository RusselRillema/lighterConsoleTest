using System.Runtime.InteropServices;

namespace LighterTest;

public static class LighterSignerLinux
{
    private static string? MarshalString(IntPtr ptr, 
        bool printDebug = false)
    {
        if (printDebug)
            Console.WriteLine($"MarshalString: {ptr} - Start");
        var result = ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
        if (printDebug)
            Console.WriteLine($"MarshalString: {ptr} - End");
        return result;
    }
    
    private static string? MarshalAndFreeString(IntPtr ptr, 
        bool printDebug = false)
    {
        if (printDebug)
            Console.WriteLine($"MarshalAndFreeString: {ptr} - Start");
        if (ptr == IntPtr.Zero)
        {
            if (printDebug)
                Console.WriteLine($"MarshalAndFreeString: {ptr} - End");
            return null;
        }
        var value = Marshal.PtrToStringUTF8(ptr);
        Free(ptr);
        if (printDebug)
            Console.WriteLine($"MarshalAndFreeString: {ptr} - End");
        return value;
    }
    
    public static string? CreateClient(
        string url,
        string privateKey,
        int chainId,
        byte apiKeyIndex,
        long accountIndex, 
        bool printDebug = false)
    {
        if (printDebug)
            Console.WriteLine($"CreateClient: {url} - Start");
        var ptr = LighterNativeLinux.CreateClient(url, privateKey, chainId, apiKeyIndex, accountIndex);
        var t = MarshalAndFreeString(ptr);
        if (printDebug)
        {
            Console.WriteLine($"CreateClient: {url} - Result: {t}");
            Console.WriteLine($"CreateClient: {url} - End");
        }
        
        return t;
    }
    
    public static string CreateAuthToken(long deadline, int apiKeyIndex, long accountIndex, bool printDebug = false)
    {
        if (printDebug)
            Console.WriteLine($"LighterNativeLinux CreateAuthToken: {deadline} - Start");
        var native = LighterNativeLinux.CreateAuthToken(deadline, apiKeyIndex, accountIndex);
        var managed = new LighterNativeLinux.ManagedStrOrErr
        {
            Str = MarshalString(native.Str, printDebug),
            Err = MarshalString(native.Err, printDebug),
        };
        if (printDebug)
            Console.WriteLine($"created StrOnErr: {deadline}");

        if (native.Str != IntPtr.Zero) Free(native.Str);
        if (native.Err != IntPtr.Zero) Free(native.Err);

        if (printDebug)
            Console.WriteLine($"native error checks done: {deadline}");

        if (!string.IsNullOrEmpty(managed.Err)) throw new Exception(managed.Err);
        if (printDebug)
            Console.WriteLine($"managed error checks done: {deadline}");

        if (printDebug)
            Console.WriteLine($"LighterNativeLinux CreateAuthToken: {deadline} - End");
        return managed.Str;
    }
    
    public static void InitSignerThread()
    {
        LighterNativeLinux.InitSignerThread();
    }

    private static void Free(IntPtr ptr)
    {
        LighterNativeLinux.InitSignerThread();
        LighterNativeLinux.Free(ptr);
    }
}