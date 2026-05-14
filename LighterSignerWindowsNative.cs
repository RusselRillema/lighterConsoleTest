using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LighterTest
{
    internal class LighterNative
    {
        internal const string DllName = "lighter-signer-windows-amd64.dll";

        // typedef struct { char* str; char* err; } StrOrErr;
        [StructLayout(LayoutKind.Sequential)]
        internal struct StrOrErr
        {
            public IntPtr str;
            public IntPtr err;
        }

        // typedef struct {
        //     uint8_t txType;
        //     char*   txInfo;
        //     char*   txHash;
        //     char*   messageToSign;
        //     char*   err;
        // } SignedTxResponse;
        [StructLayout(LayoutKind.Sequential)]
        internal struct SignedTxResponse
        {
            public byte txType;
            public IntPtr txInfo;
            public IntPtr txHash;
            public IntPtr messageToSign;
            public IntPtr err;
        }

        // typedef struct { char* privateKey; char* publicKey; char* err; } ApiKeyResponse;
        [StructLayout(LayoutKind.Sequential)]
        internal struct ApiKeyResponse
        {
            public IntPtr privateKey;
            public IntPtr publicKey;
            public IntPtr err;
        }

        // typedef struct {
        //     int16_t  MarketIndex;
        //     int64_t  ClientOrderIndex;
        //     int64_t  BaseAmount;
        //     uint32_t Price;
        //     uint8_t  IsAsk;
        //     uint8_t  Type;
        //     uint8_t  TimeInForce;
        //     uint8_t  ReduceOnly;
        //     uint32_t TriggerPrice;
        //     int64_t  OrderExpiry;
        // } CreateOrderTxReq;
        //
        // Pack=1 to match the Go-side struct layout exactly (cgo emits no padding
        // hints, but Go packs these fields tightly; using Pack=1 is the safe match).
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct CreateOrderTxReq
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

        // ----- Exported functions ------------------------------------------------
        // All string inputs are marshalled as ANSI (the Go side expects char*/
        // UTF-8; plain ASCII hex / URLs pass through identically, and
        // CharSet.Ansi avoids unwanted wide-char marshalling).

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ApiKeyResponse GenerateAPIKey();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr CreateClient(
            [MarshalAs(UnmanagedType.LPStr)] string cUrl,
            [MarshalAs(UnmanagedType.LPStr)] string cPrivateKey,
            int cChainId,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CheckClient(int cApiKeyIndex, long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SignedTxResponse SignChangePubKey(
            [MarshalAs(UnmanagedType.LPStr)] string cPubKey,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignCreateOrder(
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

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignCreateGroupedOrders(
            byte cGroupingType,
            [In] CreateOrderTxReq[] cOrders,
            int cLen,
            long cIntegratorAccountIndex,
            int cIntegratorTakerFee,
            int cIntegratorMakerFee,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignCancelOrder(
            int cMarketIndex,
            long cOrderIndex,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignWithdraw(
            int cAssetIndex,
            int cRouteType,
            ulong cAmount,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignCreateSubAccount(
            byte cSkipNonce, long cNonce, int cApiKeyIndex, long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignCancelAllOrders(
            int cTimeInForce,
            long cTime,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignModifyOrder(
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

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SignedTxResponse SignTransfer(
            long cToAccountIndex,
            short cAssetIndex,
            byte cFromRouteType,
            byte cToRouteType,
            long cAmount,
            long cUsdcFee,
            [MarshalAs(UnmanagedType.LPStr)] string cMemo,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignCreatePublicPool(
            long cOperatorFee,
            int cInitialTotalShares,
            long cMinOperatorShareRate,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignUpdatePublicPool(
            long cPublicPoolIndex,
            int cStatus,
            long cOperatorFee,
            int cMinOperatorShareRate,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignMintShares(
            long cPublicPoolIndex,
            long cShareAmount,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignBurnShares(
            long cPublicPoolIndex,
            long cShareAmount,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignUpdateLeverage(
            int cMarketIndex,
            int cInitialMarginFraction,
            int cMarginMode,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern StrOrErr CreateAuthToken(
            long cDeadline, int cApiKeyIndex, long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignUpdateMargin(
            int cMarketIndex,
            long cUSDCAmount,
            int cDirection,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignStakeAssets(
            long cStakingPoolIndex,
            long cShareAmount,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignUnstakeAssets(
            long cStakingPoolIndex,
            long cShareAmount,
            byte cSkipNonce,
            long cNonce,
            int cApiKeyIndex,
            long cAccountIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SignedTxResponse SignApproveIntegrator(
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

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Free(IntPtr ptr);
    }

    //James - is this needed, doesn't extend base class
    // ---------------------------------------------------------------------
    // Managed result types
    // ---------------------------------------------------------------------
    public sealed class LighterException : Exception
    {
        public LighterException(string message) : base(message) { }
    }

    public sealed class ApiKey
    {
        public string PrivateKey { get; }
        public string PublicKey { get; }
        public ApiKey(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }

    internal sealed class LighterSignedTx
    {
        public byte TxType { get; }
        public string TxInfo { get; set; }
        public string TxHash { get; }
        public string MessageToSign { get; }

        public LighterSignedTx(byte txType, string txInfo, string txHash, string messageToSign)
        {
            TxType = txType;
            TxInfo = txInfo;
            TxHash = txHash;
            MessageToSign = messageToSign;
        }
    }

    public struct CreateOrderTxReq
    {
        public short MarketIndex;
        public long ClientOrderIndex;
        public long BaseAmount;
        public uint Price;
        public bool IsAsk;
        public byte Type;
        public byte TimeInForce;
        public bool ReduceOnly;
        public uint TriggerPrice;
        public long OrderExpiry;

        internal LighterNative.CreateOrderTxReq ToNative() => new LighterNative.CreateOrderTxReq
        {
            MarketIndex = MarketIndex,
            ClientOrderIndex = ClientOrderIndex,
            BaseAmount = BaseAmount,
            Price = Price,
            IsAsk = (byte)(IsAsk ? 1 : 0),
            Type = Type,
            TimeInForce = TimeInForce,
            ReduceOnly = (byte)(ReduceOnly ? 1 : 0),
            TriggerPrice = TriggerPrice,
            OrderExpiry = OrderExpiry,
        };
    }
}
