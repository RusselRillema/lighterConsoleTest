using System.Runtime.InteropServices;

namespace LighterTest
{
    internal static class LighterSignerWindows
    {

        // ----- helpers ----------------------------------------------------
        private static string TakeString(IntPtr p)
        {
            if (p == IntPtr.Zero) return null;
            try { return Marshal.PtrToStringAnsi(p); }
            finally { LighterNativeWindows.Free(p); }
        }

        private static void ThrowIfError(IntPtr errPtr)
        {
            if (errPtr == IntPtr.Zero) return;
            string msg = Marshal.PtrToStringAnsi(errPtr) ?? "unknown error";
            LighterNativeWindows.Free(errPtr);
            throw new LighterException(msg);
        }

        private static void ThrowIfCharPtrError(IntPtr errPtr)
        {
            // Functions that return a bare char* use non-null == error, null == success.
            if (errPtr == IntPtr.Zero) return;
            string msg = Marshal.PtrToStringAnsi(errPtr) ?? "unknown error";
            LighterNativeWindows.Free(errPtr);
            throw new LighterException(msg);
        }

        private static LighterSignedTx Unpack(LighterNativeWindows.SignedTxResponse r)
        {
            // Always drain every char* the DLL handed us so nothing leaks,
            // even if one of them is an error.
            string err = r.err != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.err) : null;
            string txInfo = r.txInfo != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.txInfo) : null;
            string txHash = r.txHash != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.txHash) : null;
            string msg2Sig = r.messageToSign != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.messageToSign) : null;

            //Why is txType not freed?
            if (r.err != IntPtr.Zero) LighterNativeWindows.Free(r.err);
            if (r.txInfo != IntPtr.Zero) LighterNativeWindows.Free(r.txInfo);
            if (r.txHash != IntPtr.Zero) LighterNativeWindows.Free(r.txHash);
            if (r.messageToSign != IntPtr.Zero) LighterNativeWindows.Free(r.messageToSign);

            if (!string.IsNullOrEmpty(err))
                throw new LighterException(err);

            return new LighterSignedTx(r.txType, txInfo, txHash, msg2Sig);
        }

        // ----- public API -------------------------------------------------

        public static ApiKey GenerateApiKey()
        {
            var r = LighterNativeWindows.GenerateAPIKey();

            string err = r.err != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.err) : null;
            string priv = r.privateKey != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.privateKey) : null;
            string pub = r.publicKey != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.publicKey) : null;

            if (r.err != IntPtr.Zero) LighterNativeWindows.Free(r.err);
            if (r.privateKey != IntPtr.Zero) LighterNativeWindows.Free(r.privateKey);
            if (r.publicKey != IntPtr.Zero) LighterNativeWindows.Free(r.publicKey);

            if (!string.IsNullOrEmpty(err)) throw new LighterException(err);
            return new ApiKey(priv, pub);
        }

        /// <summary>Initialises an internal signer client. Must be called before signing.</summary>
        public static string CreateClient(string url, string privateKey, int chainId, int apiKeyIndex, long accountIndex)
        {
            IntPtr p = LighterNativeWindows.CreateClient(url, privateKey, chainId, apiKeyIndex, accountIndex);
            ThrowIfCharPtrError(p);
            return p.ToString();
        }

        /// <summary>Returns without throwing if a client for (apiKeyIndex, accountIndex) exists.</summary>
        public static void CheckClient(int apiKeyIndex, long accountIndex)
        {
            IntPtr p = LighterNativeWindows.CheckClient(apiKeyIndex, accountIndex);
            ThrowIfCharPtrError(p);
        }

        public static string CreateAuthToken(long deadline, int apiKeyIndex, long accountIndex)
        {
            var r = LighterNativeWindows.CreateAuthToken(deadline, apiKeyIndex, accountIndex);

            string err = r.err != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.err) : null;
            string val = r.str != IntPtr.Zero ? Marshal.PtrToStringAnsi(r.str) : null;

            if (r.err != IntPtr.Zero) LighterNativeWindows.Free(r.err);
            if (r.str != IntPtr.Zero) LighterNativeWindows.Free(r.str);

            if (!string.IsNullOrEmpty(err)) throw new LighterException(err);
            return val;
        }

        internal static LighterSignedTx SignChangePubKey(string pubKey, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignChangePubKey(pubKey, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignCreateOrder(
            int marketIndex, long clientOrderIndex, long baseAmount, int price,
            bool isAsk, int orderType, int timeInForce, bool reduceOnly,
            int triggerPrice, long orderExpiry,
            long integratorAccountIndex, int integratorTakerFee, int integratorMakerFee,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignCreateOrder(
                marketIndex, clientOrderIndex, baseAmount, price,
                isAsk ? 1 : 0, orderType, timeInForce, reduceOnly ? 1 : 0,
                triggerPrice, orderExpiry,
                integratorAccountIndex, integratorTakerFee, integratorMakerFee,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignCreateGroupedOrders(
            byte groupingType, CreateOrderTxReq[] orders,
            long integratorAccountIndex, int integratorTakerFee, int integratorMakerFee,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
        {
            if (orders == null) throw new ArgumentNullException(nameof(orders));
            var native = new LighterNativeWindows.CreateOrderTxReq[orders.Length];
            for (int i = 0; i < orders.Length; i++) native[i] = orders[i].ToNative();

            return Unpack(LighterNativeWindows.SignCreateGroupedOrders(
                groupingType, native, native.Length,
                integratorAccountIndex, integratorTakerFee, integratorMakerFee,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));
        }

        internal static LighterSignedTx SignCancelOrder(int marketIndex, long orderIndex, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignCancelOrder(marketIndex, orderIndex, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignWithdraw(int assetIndex, int routeType, ulong amount, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignWithdraw(assetIndex, routeType, amount, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignCreateSubAccount(bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignCreateSubAccount((byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignCancelAllOrders(int timeInForce, long time, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignCancelAllOrders(timeInForce, time, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignModifyOrder(
            int marketIndex, long index, long baseAmount, long price, long triggerPrice,
            long integratorAccountIndex, int integratorTakerFee, int integratorMakerFee,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignModifyOrder(
                marketIndex, index, baseAmount, price, triggerPrice,
                integratorAccountIndex, integratorTakerFee, integratorMakerFee,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignTransfer(
            long toAccountIndex, short assetIndex, byte fromRouteType, byte toRouteType,
            long amount, long usdcFee, string memo,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignTransfer(
                toAccountIndex, assetIndex, fromRouteType, toRouteType,
                amount, usdcFee, memo ?? string.Empty,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignCreatePublicPool(
            long operatorFee, int initialTotalShares, long minOperatorShareRate,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignCreatePublicPool(
                operatorFee, initialTotalShares, minOperatorShareRate,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignUpdatePublicPool(
            long publicPoolIndex, int status, long operatorFee, int minOperatorShareRate,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignUpdatePublicPool(
                publicPoolIndex, status, operatorFee, minOperatorShareRate,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignMintShares(long publicPoolIndex, long shareAmount, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignMintShares(publicPoolIndex, shareAmount, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignBurnShares(long publicPoolIndex, long shareAmount, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignBurnShares(publicPoolIndex, shareAmount, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignUpdateLeverage(
            int marketIndex, int initialMarginFraction, int marginMode,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignUpdateLeverage(
                marketIndex, initialMarginFraction, marginMode,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignUpdateMargin(
            int marketIndex, long usdcAmount, int direction,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignUpdateMargin(
                marketIndex, usdcAmount, direction,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignStakeAssets(long stakingPoolIndex, long shareAmount, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignStakeAssets(stakingPoolIndex, shareAmount, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignUnstakeAssets(long stakingPoolIndex, long shareAmount, bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignUnstakeAssets(stakingPoolIndex, shareAmount, (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));

        internal static LighterSignedTx SignApproveIntegrator(
            long integratorIndex,
            uint maxPerpsTakerFee, uint maxPerpsMakerFee,
            uint maxSpotTakerFee, uint maxSpotMakerFee,
            long approvalExpiry,
            bool skipNonce, long nonce, int apiKeyIndex, long accountIndex)
            => Unpack(LighterNativeWindows.SignApproveIntegrator(
                integratorIndex,
                maxPerpsTakerFee, maxPerpsMakerFee, maxSpotTakerFee, maxSpotMakerFee,
                approvalExpiry,
                (byte)(skipNonce ? 1 : 0), nonce, apiKeyIndex, accountIndex));
    }

}
