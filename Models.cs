using System.Text.Json.Serialization;

namespace LighterTest;

public class Models
{
    #region Lighter Accounts Info
    internal class LighterAccountsInfo
    {
        [JsonPropertyName("code")] public int? Code { get; set; }
        [JsonPropertyName("message")] public string Message { get; set; }
        [JsonPropertyName("total")] public long? Total { get; set; }
        [JsonPropertyName("accounts")] public List<LighterAccount> Accounts { get; set; }
    }

    internal class LighterAccount
    {
        [JsonPropertyName("code")] public int? Code { get; set; }
        [JsonPropertyName("message")] public string Message { get; set; }
        [JsonPropertyName("account_type")] public int AccountType { get; set; }
        [JsonPropertyName("account_trading_mode")] public int AccountTradingMode { get; set; }
        [JsonPropertyName("index")] public long Index { get; set; }
        [JsonPropertyName("l1_address")] public string L1Address { get; set; }
        [JsonPropertyName("cancel_all_time")] public long? CancelAllTime { get; set; }
        [JsonPropertyName("total_order_count")] public long? TotalOrderCount { get; set; }
        [JsonPropertyName("total_isolated_order_count")] public long? TotalIsolatedOrderCount { get; set; }
        [JsonPropertyName("pending_order_count")] public long? PendingOrderCount { get; set; }
        [JsonPropertyName("available_balance")] public string AvailableBalance { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("collateral")] public string Collateral { get; set; }
        [JsonPropertyName("account_index")] public long AccountIndex { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("can_invite")] public bool CanInvite { get; set; }
        [JsonPropertyName("referral_points_percentage")] public string ReferralPointsPercentage { get; set; }
        [JsonPropertyName("positions")] public List<LighterPosition> Positions { get; set; }
        [JsonPropertyName("assets")] public List<LighterAssets> Assets { get; set; }
        [JsonPropertyName("total_asset_value")] public string TotalAssetValue { get; set; }
        [JsonPropertyName("cross_asset_value")] public string CrossAssetValue { get; set; }
        [JsonPropertyName("pool_info")] public LighterPoolInfo PoolInfo { get; set; }
        [JsonPropertyName("shares")] public List<LighterPoolShares> Shares { get; set; }
        [JsonPropertyName("created_at")] public int CreatedAt { get; set; }
        [JsonPropertyName("transaction_time")] public long TransactionTime { get; set; }
        [JsonPropertyName("pending_unlocks")] public List<LighterPendingUnlocks> PendingUnlocks { get; set; }
        [JsonPropertyName("approved_integrators")] public List<LighterApprovedIntegrators> ApprovedIntegrators { get; set; }
    }

    internal class LighterPosition
    {
        [JsonPropertyName("market_id")] public string MarketId { get; set; }
        [JsonPropertyName("symbol")] public string Symbol { get; set; }
        [JsonPropertyName("initial_margin_fraction")] public decimal? InitialMarginFraction { get; set; }
        [JsonPropertyName("open_order_count")] public int OpenOrderCount { get; set; }
        [JsonPropertyName("pending_order_count")] public int PendingOrderCount { get; set; }
        [JsonPropertyName("position_tied_order_count")] public int PositionTiedOrderCount { get; set; }
        [JsonPropertyName("sign")] public int Sign { get; set; }
        [JsonPropertyName("position")] public decimal? Position { get; set; }
        [JsonPropertyName("avg_entry_price")] public decimal? AvgEntryPrice { get; set; }
        [JsonPropertyName("position_value")] public decimal? PositionValue { get; set; }
        [JsonPropertyName("unrealized_pnl")] public decimal? UnrealizedPnl { get; set; }
        [JsonPropertyName("realized_pnl")] public decimal? RealizedPnl { get; set; }
        [JsonPropertyName("liquidation_price")] public decimal? LiquidationPrice { get; set; }
        [JsonPropertyName("total_funding_paid_out")] public decimal? TotalFundingPaidOut { get; set; }
        [JsonPropertyName("margin_mode")] public int MarginMode { get; set; }
        [JsonPropertyName("allocated_margin")] public decimal? AllocatedMargin { get; set; }
        [JsonPropertyName("total_discount")] public decimal? TotalDiscount { get; set; }
    }

    internal class LighterAssets
    {
        [JsonPropertyName("symbol")] public string Symbol { get; set; }
        [JsonPropertyName("asset_id")] public int AssetId { get; set; }
        [JsonPropertyName("balance")] public decimal? Balance { get; set; }
        [JsonPropertyName("locked_balance")] public decimal? LockedBalance { get; set; }
    }

    internal class LighterPoolInfo
    {
        [JsonPropertyName("status")] public int? Status { get; set; }
        [JsonPropertyName("operator_fee")] public string OperatorFee { get; set; }
        [JsonPropertyName("min_operator_share_rate")] public string MinOperatorShareRate { get; set; }
        [JsonPropertyName("total_shares")] public long? TotalShares { get; set; }
        [JsonPropertyName("operator_shares")] public long? OperatorShares { get; set; }
        [JsonPropertyName("annual_percentage_yield")] public decimal? AnnualPercentageYield { get; set; }
        [JsonPropertyName("daily_returns")] public List<LighterDailyReturn> DailyReturns { get; set; }
        [JsonPropertyName("share_prices")] public List<LighterSharePrice> SharePrices { get; set; }
        [JsonPropertyName("sharpe_ratio")] public decimal? SharpeRatio { get; set; }
        [JsonPropertyName("strategies")] public List<LighterStrategy> Strategies { get; set; }
    }

    internal class LighterDailyReturn
    {
        [JsonPropertyName("timestamp")] public long Timestamp { get; set; }
        [JsonPropertyName("daily_return")] public decimal? DailyReturn { get; set; }
    }

    internal class LighterSharePrice
    {
        [JsonPropertyName("timestamp")] public long Timestamp { get; set; }
        [JsonPropertyName("share_price")] public decimal? SharePrice { get; set; }
    }

    internal class LighterStrategy
    {
        [JsonPropertyName("collateral")] public decimal? Collateral { get; set; }
    }

    internal class LighterPoolShares
    {
        [JsonPropertyName("public_pool_index")] public long PublicPoolIndex { get; set; }
        [JsonPropertyName("shares_amount")] public long SharesAmount { get; set; }
        [JsonPropertyName("entry_usdc")] public decimal? EntryUsdc { get; set; }
        [JsonPropertyName("principal_amount")] public decimal? PrincipalAmount { get; set; }
        [JsonPropertyName("entry_timestamp")] public long EntryTimestamp { get; set; }
    }

    internal class LighterPendingUnlocks
    {
        [JsonPropertyName("unlock_timestamp")] public int UnlockTimestamp { get; set; }
        [JsonPropertyName("asset_index")] public int AssetIndex { get; set; }
        [JsonPropertyName("amount")] public string Amount { get; set; }
    }

    internal class LighterApprovedIntegrators
    {
        [JsonPropertyName("account_index")] public string AccountIndex { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("max_perps_taker_fee")] public string MaxPerpsTakerFee { get; set; }
        [JsonPropertyName("max_perps_maker_fee")] public string MaxPerpsMakerFee { get; set; }
        [JsonPropertyName("max_spot_taker_fee")] public string MaxSpotTakerFee { get; set; }
        [JsonPropertyName("max_spot_maker_fee")] public string MaxSpotMakerFee { get; set; }
        [JsonPropertyName("approval_expiry")] public string ApprovalExpiry { get; set; }
    }

    #region Lighter API Keys Info
    internal class LighterApiKeysInfo
    {
        [JsonPropertyName("code")] public int? Code { get; set; }
        [JsonPropertyName("api_keys")] public List<LighterApiKeyInfo> ApiKeys { get; set; }
    }

    internal class LighterApiKeyInfo
    {
        [JsonPropertyName("account_index")] public long AccountIndex { get; set; }
        [JsonPropertyName("api_key_index")] public short ApiKeyIndex { get; set; }
        [JsonPropertyName("nonce")] public long Nonce { get; set; }
        [JsonPropertyName("public_key")] public string PublicKey { get; set; }
        [JsonPropertyName("transaction_time")] public long TransactionTime { get; set; }
    }
    #endregion
    #endregion
}
