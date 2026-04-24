using System.Runtime.InteropServices;

namespace LighterTest;


public static class LighterNativeLinux
{
    public const string DllName = "/home/russel/github/lighter-go/build/lighter-signer-linux-amd64.so";

    // ── Native structs (must mirror the CGO C struct layout exactly) ────

    /// <summary>Mirrors: typedef struct { char* str; char* err; } StrOrErr;</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeStrOrErr
    {
        public IntPtr Str;
        public IntPtr Err;
    }

    internal sealed class ManagedStrOrErr
    {
        public string? Str { get; init; }
        public string? Err { get; init; }
    }
    
    /// <summary>
    /// Mirrors:
    /// typedef struct { uint8_t txType; char* txInfo; char* txHash;
    ///                  char* messageToSign; char* err; } SignedTxResponse;
    /// On arm64 Linux, the byte field is followed by 7 bytes of padding before
    /// the first pointer — Sequential with default (natural) alignment handles this.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSignedTxResponse
    {
        public byte TxType;
        public IntPtr TxInfo;
        public IntPtr TxHash;
        public IntPtr MessageToSign;
        public IntPtr Err;
    }

    /// <summary>Mirrors: typedef struct { char* privateKey; char* publicKey; char* err; } ApiKeyResponse;</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeApiKeyResponse
    {
        public IntPtr PrivateKey;
        public IntPtr PublicKey;
        public IntPtr Err;
    }

    /// <summary>
    /// Mirrors the CreateOrderTxReq struct used by SignCreateGroupedOrders.
    /// Layout on arm64 Linux (natural alignment):
    ///   offset  0 – int16  MarketIndex        (2 B)
    ///   offset  8 – int64  ClientOrderIndex   (8 B)  [6 B padding before]
    ///   offset 16 – int64  BaseAmount         (8 B)
    ///   offset 24 – uint32 Price              (4 B)
    ///   offset 28 – uint8  IsAsk              (1 B)
    ///   offset 29 – uint8  Type               (1 B)
    ///   offset 30 – uint8  TimeInForce        (1 B)
    ///   offset 31 – uint8  ReduceOnly         (1 B)
    ///   offset 32 – uint32 TriggerPrice       (4 B)
    ///   offset 40 – int64  OrderExpiry        (8 B)  [4 B padding before]
    ///   total: 48 B
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeCreateOrderTxReq
    {
        public short MarketIndex;
        public long ClientOrderIndex;
        public long BaseAmount;
        public uint Price;
        public byte IsAsk;
        public byte Type;
        public byte TimeInForce;
        public byte ReduceOnly;
        public uint TriggerPrice;
        public long OrderExpiry;
    }

    // ── P/Invoke declarations ────────────────────────────────────────────

    [DllImport(DllName, EntryPoint = "GenerateAPIKey", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeApiKeyResponse GenerateAPIKey();

    [DllImport(DllName, EntryPoint = "CreateClient", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr CreateClient(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string cUrl,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string cPrivateKey,
        int cChainId,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "CheckClient", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr CheckClient(
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignChangePubKey", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignChangePubKey(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string cPubKey,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignCreateOrder", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignCreateOrder(
        int cMarketIndex,
        long cClientOrderIndex,
        long cBaseAmount,
        int cPrice,
        int cIsAsk,
        int cOrderType,
        int cTimeInForce,
        int cReduceOnly,
        int cTriggerPrice,
        long cOrderExpiry,
        long cIntegratorAccountIndex,
        int cIntegratorTakerFee,
        int cIntegratorMakerFee,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignCreateGroupedOrders", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignCreateGroupedOrders(
        byte cGroupingType,
        [In] NativeCreateOrderTxReq[] cOrders,
        int cLen,
        long cIntegratorAccountIndex,
        int cIntegratorTakerFee,
        int cIntegratorMakerFee,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignCancelOrder", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignCancelOrder(
        int cMarketIndex,
        long cOrderIndex,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    // Note: the Linux header uses `long long unsigned int` for cAmount —
    // semantically identical to `unsigned long long` (ulong in C#).
    [DllImport(DllName, EntryPoint = "SignWithdraw", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignWithdraw(
        int cAssetIndex,
        int cRouteType,
        ulong cAmount,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignCreateSubAccount", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignCreateSubAccount(
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignCancelAllOrders", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignCancelAllOrders(
        int cTimeInForce,
        long cTime,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignModifyOrder", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignModifyOrder(
        int cMarketIndex,
        long cIndex,
        long cBaseAmount,
        long cPrice,
        long cTriggerPrice,
        long cIntegratorAccountIndex,
        int cIntegratorTakerFee,
        int cIntegratorMakerFee,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignTransfer", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignTransfer(
        long cToAccountIndex,
        short cAssetIndex,
        byte cFromRouteType,
        byte cToRouteType,
        long cAmount,
        long cUsdcFee,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string cMemo,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignCreatePublicPool", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignCreatePublicPool(
        long cOperatorFee,
        int cInitialTotalShares,
        long cMinOperatorShareRate,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignUpdatePublicPool", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignUpdatePublicPool(
        long cPublicPoolIndex,
        int cStatus,
        long cOperatorFee,
        int cMinOperatorShareRate,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignMintShares", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignMintShares(
        long cPublicPoolIndex,
        long cShareAmount,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignBurnShares", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignBurnShares(
        long cPublicPoolIndex,
        long cShareAmount,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignUpdateLeverage", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignUpdateLeverage(
        int cMarketIndex,
        int cInitialMarginFraction,
        int cMarginMode,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "CreateAuthToken", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeStrOrErr CreateAuthToken(
        long cDeadline,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignUpdateMargin", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignUpdateMargin(
        int cMarketIndex,
        long cUSDCAmount,
        int cDirection,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignStakeAssets", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignStakeAssets(
        long cStakingPoolIndex,
        long cShareAmount,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignUnstakeAssets", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignUnstakeAssets(
        long cStakingPoolIndex,
        long cShareAmount,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    [DllImport(DllName, EntryPoint = "SignApproveIntegrator", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeSignedTxResponse SignApproveIntegrator(
        long cIntegratorIndex,
        uint cMaxPerpsTakerFee,
        uint cMaxPerpsMakerFee,
        uint cMaxSpotTakerFee,
        uint cMaxSpotMakerFee,
        long cApprovalExpiry,
        byte cSkipNonce,
        long cNonce,
        int cApiKeyIndex,
        long cAccountIndex);

    /// <summary>
    /// Frees a pointer that was allocated by Go/CGO.
    /// Must be called for every non-null IntPtr returned inside a native struct.
    /// </summary>
    [DllImport(DllName, EntryPoint = "Free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void Free(IntPtr ptr);
}