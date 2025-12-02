# Security Deposit Investment Pool Workflow

## Overview

This document describes the complete lifecycle of security deposits from collection through dividend distribution in the Aquiis property management system.

## Workflow Stages

### 1. Deposit Collection

**When**: Prospective tenant accepts lease offer

**Where**: `/propertymanagement/leases/{LeaseId}/accept` (AcceptLease page)

**Required Before Acceptance**:

- ✅ Checkbox: "I confirm that the tenant has read, understood, and agrees to all terms"
- ✅ Checkbox: "Security deposit of $XXX has been paid in full"
- ✅ Payment Method: Must select from dropdown (Check, Credit Card, Bank Transfer, etc.)
- ⚠️ Transaction Reference: Optional (check number, confirmation, etc.)

**Action**: System automatically collects deposit via `CollectSecurityDepositAsync()`

**Critical Validation**:

- Deposit collection MUST succeed before lease acceptance completes
- If deposit collection fails, entire lease acceptance is aborted
- Error displayed: "CRITICAL ERROR: Failed to collect security deposit. Lease acceptance aborted."
- This ensures no lease can be activated without a security deposit record

**Result**:

- Creates `SecurityDeposit` record in database
- Status = "Held"
- InInvestmentPool = false (initially)
- Amount recorded with payment method and transaction reference
- **UNIQUE constraint**: Each lease can only have ONE security deposit (enforced by database)

**Where to View**: Navigate to `/property-management/security-deposits`

**Database Schema Protection**:

- UNIQUE INDEX on LeaseId prevents duplicate deposits
- Foreign key to Lease (ON DELETE RESTRICT) prevents orphaned records
- All required fields enforced at schema level

---

### 2. Add to Investment Pool

**When**: Lease becomes active and you want deposit to start earning

**Action**:

1. Navigate to `/property-management/security-deposits`
2. Find deposit with status "Held" and InInvestmentPool = "No"
3. Click the green **plus button** (➕) in the Actions column

**System Process**: Calls `AddToInvestmentPoolAsync(depositId)`

**Result**:

- InInvestmentPool = true
- PoolEntryDate = current date
- Deposit now part of "Current Pool Balance"
- Will participate in annual earnings

**Important**: Only deposits marked as "in pool" will receive dividends!

---

### 3. Record Annual Investment Performance

**When**: Once per year, typically after calendar year ends (e.g., January 2026 for 2025 performance)

**Action**:

1. Navigate to `/property-management/security-deposits/investment-pools`
2. Click **"Record Performance"** button
3. Enter the following information:
   - **Year**: Investment year (e.g., 2025)
   - **Total Earnings**: Dollar amount earned from actual investment account
     - Can be **positive** (gains) or **negative** (losses)
     - Get this from your bank/investment account statement
   - **Notes**: Optional details about fund performance

**System Calculations**:

- **Starting Balance**: Automatically calculated from deposits in pool at year start
- **Return Rate**: Automatically calculated (TotalEarnings / StartingBalance)
- **Organization Share**: TotalEarnings × OrganizationSharePercentage (default 20%)
- **Tenant Share Total**: TotalEarnings - OrganizationShare

**Special Cases**:

- **Positive Earnings**: Split between organization (20%) and tenants (80%)
- **Negative Earnings (Losses)**: Organization absorbs 100% of losses
  - Tenants never see negative dividends
  - Their security deposits remain unchanged

**Result**:

- Creates or updates `SecurityDepositInvestmentPool` for that year
- Pool status = "Open"
- Ready for dividend calculation

---

### 4. Calculate Dividends

**When**: After recording performance, typically before distribution month (configured in Organization Settings)

**Action**:

1. Navigate to `/property-management/security-deposits/investment-pools`
2. Find the year with status "Open"
3. Click the **calculator icon** (🖩) in the Actions column
4. Review dividend preview showing all calculations
5. Click **"Confirm & Calculate Dividends"**

**System Process**: Calls `CalculateDividendsAsync(year)`

**Calculations**:

1. Gets all deposits with InInvestmentPool = true during that year
2. Calculates pro-ration for each deposit:
   - **Months in Pool** = How many months deposit was invested (1-12)
   - **Proration Factor** = MonthsInPool / 12
   - Example: Deposit added July 1 = 6 months = 50% proration
3. Distributes tenant share equally:
   - **Base Dividend** = TenantShareTotal / ActiveLeaseCount
   - **Final Dividend** = BaseDividend × ProrationFactor

**Result**:

- Creates `SecurityDepositDividend` records for each active deposit
- Status = "Pending" (awaiting tenant payment method choice)
- Pool status changes to "Calculated"
- DividendsCalculatedOn date recorded

**Example**:

```
Year: 2025
Total Earnings: $10,000
Organization Share (20%): $2,000
Tenant Share Total (80%): $8,000
Active Leases: 10
Base Dividend Per Lease: $800

Tenant A (12 months in pool): $800 × 100% = $800
Tenant B (6 months in pool): $800 × 50% = $400
Tenant C (3 months in pool): $800 × 25% = $200
```

---

### 5. Tenant Dividend Choice

**When**: After dividends calculated, before distribution month

**Action**: Tenants choose how to receive their dividend

**Payment Methods**:

- **Lease Credit**: Applied to next month's rent
- **Check**: Mailed physical check

**Configuration**:

- `OrganizationSettings.AllowTenantDividendChoice`
  - If **true**: Tenants choose their method
  - If **false**: Uses `DefaultDividendPaymentMethod` from settings

**Result**:

- Dividend record updated with chosen payment method
- Status remains "Pending" until processed

---

### 6. Distribute Dividends

**When**: During configured distribution month (from Organization Settings)

**Action**:

1. Navigate to pool details page
2. Review all dividends with chosen payment methods
3. Process payments:
   - **Lease Credits**: Applied to tenant accounts
   - **Checks**: Generate and mail to tenants

**System Process**: Calls `ProcessDividendPaymentAsync(dividendId)`

**Result**:

- Dividend status changes to "Paid"
- Pool status may change to "Distributed" when all dividends paid
- Payment recorded with date and method

---

### 7. Remove from Pool (When Lease Ends)

**When**: Lease ends and deposit needs to be refunded

**Action**:

1. Navigate to `/property-management/security-deposits`
2. Find the deposit to remove
3. Click the **minus button** (➖) to remove from pool
4. Click the **refund button** (↩) to initiate refund

**System Process**:

- Calls `RemoveFromInvestmentPoolAsync(depositId)`
- Sets InInvestmentPool = false
- Sets PoolExitDate = current date

**Result**:

- Deposit no longer earns investment returns
- Ready for refund processing
- Pro-rated dividends already calculated up to exit date

---

### 8. Process Refund

**When**: After final inspection, deductions calculated

**Action**: Process refund via refund workflow (coming soon)

**Calculations**:

- Original deposit amount
- Plus: All dividends earned
- Minus: Any damages or unpaid rent
- Equals: Final refund amount

**Result**:

- Deposit status = "Refunded"
- Transaction recorded
- Lease fully closed

---

## Key Pages

| Page                | Route                                                                 | Purpose                                 |
| ------------------- | --------------------------------------------------------------------- | --------------------------------------- |
| Security Deposits   | `/property-management/security-deposits`                              | View all deposits, add/remove from pool |
| Investment Pools    | `/property-management/security-deposits/investment-pools`             | View annual performance history         |
| Record Performance  | `/property-management/security-deposits/record-performance`           | Enter annual earnings manually          |
| Calculate Dividends | `/property-management/security-deposits/calculate-dividends/{poolId}` | Calculate and confirm dividends         |
| Pool Details        | `/property-management/security-deposits/investment-pool/{poolId}`     | View specific year's details            |

---

## Summary Statistics

On the **Security Deposits** page, you'll see four key metrics:

1. **Total Deposits Held**: All deposits with status "Held" (not yet refunded)
2. **Current Pool Balance**: Sum of all deposits where InInvestmentPool = true
3. **Released Deposits**: Deposits ready for refund processing
4. **Total Refunded**: Historical total of all refunded deposits

---

## Important Notes

### Investment Performance Entry

- **Manual Process**: You must manually enter annual earnings
- **Data Source**: Get earnings from your actual investment/bank account
- **Not Automated**: System does NOT calculate earnings from transactions
- **Why**: Security deposits may be invested in external accounts (money market, bonds, etc.)

### Loss Protection

- **Tenant Protection**: Tenants never receive negative dividends
- **Organization Risk**: All investment losses absorbed by organization
- **Deposit Safety**: Tenant deposit amounts always remain whole

### Pro-ration Fairness

- **Mid-year Move-ins**: Tenants only earn dividends for months their deposit was invested
- **Example**: Tenant moves in July 1 = 6 months = 50% of annual dividend
- **Automatic**: System calculates based on PoolEntryDate and PoolExitDate

### Organization Share

- **Configurable**: Set in Organization Settings
- **Default**: 20% of earnings
- **Purpose**: Covers administrative costs of managing investment pool
- **Revenue**: Org share becomes organization revenue

---

## Configuration Settings

Located in **Organization Settings** (`/administration/organization-settings`):

- **OrganizationSharePercentage**: Org's cut of earnings (default 0.20 = 20%)
- **AllowTenantDividendChoice**: true/false - can tenants choose payment method?
- **DefaultDividendPaymentMethod**: Default method if choice not allowed
- **DividendDistributionMonth**: Which month to distribute (1-12)

---

## Status Values

### SecurityDeposit.Status

- **Held**: Deposit collected and held (may or may not be in pool)
- **Released**: Ready for refund processing
- **Refunded**: Fully refunded to tenant
- **Forfeited**: Kept due to damages/unpaid rent
- **PartiallyRefunded**: Partial refund processed

### SecurityDeposit.InInvestmentPool

- **true**: Currently invested, earning returns
- **false**: Not invested, held in regular account

### InvestmentPool.Status

- **Open**: Performance recorded, ready for dividend calculation
- **Calculated**: Dividends calculated, awaiting distribution
- **Distributed**: All dividends paid
- **Closed**: Year closed, final

### Dividend.Status

- **Pending**: Awaiting tenant payment method choice or processing
- **Paid**: Dividend distributed to tenant
- **Forfeited**: Tenant forfeited dividend (e.g., lease violation)

---

## Example Timeline

### Year 2025 - Complete Cycle

**January 1, 2025**:

- 10 deposits in pool, total $15,000 (Starting Balance)

**July 1, 2025**:

- New lease, deposit $1,500 collected
- Added to pool (will earn 6 months = 50% proration)

**December 31, 2025**:

- Pool ending balance = $16,500

**January 15, 2026**:

- Investment account statement shows $1,200 earned in 2025
- Navigate to Record Performance
- Enter: Year=2025, Total Earnings=$1,200

**System Calculates**:

- Return Rate: 8% ($1,200 / $15,000)
- Organization Share (20%): $240
- Tenant Share Total (80%): $960
- Active Leases: 11
- Base Dividend: $87.27

**January 20, 2026**:

- Navigate to Calculate Dividends
- Review preview showing pro-rated amounts
- Confirm calculation

**Example Dividends Created**:

- Tenant #1 (12 months): $87.27 × 100% = $87.27
- Tenant #2 (12 months): $87.27 × 100% = $87.27
- ...
- Tenant #11 (6 months, July start): $87.27 × 50% = $43.64

**February 1-28, 2026** (Distribution Month):

- Tenants choose: Lease credit or check
- Process all payments
- Pool status → "Distributed"

**Lease End** (varies):

- Remove from pool (PoolExitDate set)
- Calculate refund (deposit + dividends - deductions)
- Process refund
- Status → "Refunded"

---

## Troubleshooting

### Deposit Not Showing in Dividend Calculation

**Check**: Is InInvestmentPool = true?
**Fix**: Add to pool via main Security Deposits page

### Wrong Dividend Amount

**Check**: PoolEntryDate - may be pro-rated for partial year
**Expected**: (TenantShareTotal / ActiveLeaseCount) × (MonthsInPool / 12)

### Can't Calculate Dividends

**Check**: Has performance been recorded for that year?
**Fix**: Record performance first via Investment Pools page

### Zero Dividends

**Check**: Were earnings negative (loss)?
**Expected**: Losses absorbed by org, tenants get $0 (not negative)

---

## Future Enhancements

- [ ] Refund workflow UI (currently shows "coming soon")
- [ ] Automated dividend distribution
- [ ] Email notifications to tenants about dividends
- [ ] Historical dividend reports per tenant
- [ ] Multi-year performance charts
- [ ] Dividend tax reporting (1099-MISC generation)
