# Aquiis SimpleStart - Quick Start Guide

**Version:** 1.1.0  
**Last Updated:** February 18, 2026  
**Estimated Time:** 15 minutes

---

## 📖 Welcome!

This guide will help you get started with Aquiis SimpleStart in just 15 minutes. By the end, you'll have:

- ✅ Installed the application
- ✅ Created your organization
- ✅ Added your first property
- ✅ Added a tenant
- ✅ Created a lease
- ✅ Generated an invoice
- ✅ Recorded a payment
- ✅ Scheduled an inspection

Let's get started!

---

## 📋 Prerequisites

Before you begin, ensure you have:

- **Operating System:**
  - Linux (Ubuntu 20.04+, Debian 11+, Fedora 35+), OR
  - Windows 10/11 (64-bit)
- **Hardware:**
  - 2 GB RAM minimum (4 GB recommended)
  - 500 MB disk space
- **Downloaded:** Aquiis SimpleStart v1.1.0 installer for your platform

### Universal Linux Support:

Aquiis is distributed as an AppImage, which runs on all major Linux distributions—including Ubuntu, Debian, Fedora, RedHat, Arch, openSUSE, and more. No installation required: just download, make executable, and run.

### Windows Portable Version:

Aquiis is available as a portable Windows executable (.exe). No installation required—just download, extract, and run. All application data is stored locally in the same folder, making it easy to use Aquiis from a USB drive or move between systems.

---

## 🚀 Step 1: Installation (5 minutes)

### Linux Installation

**Option A: AppImage (Recommended for most users)**

```bash
# 1. Download the file
# File: Aquiis-1.1.0-x86_64.AppImage

# 2. Make it executable
chmod +x Aquiis-1.1.0-x86_64.AppImage

# 3. Run the application
./Aquiis-1.1.0-x86_64.AppImage
```

**Option B: Debian Package (Ubuntu/Debian users)**

```bash
# 1. Install the package
sudo dpkg -i Aquiis-1.1.0-amd64.deb

# 2. Run the application
aquiis-simplestart
```

### Windows Installation

**Option A: NSIS Installer (Recommended)**

1. **Download** `Aquiis-1.1.0-x64-Setup.exe`
2. **Double-click** the installer
3. **Follow the wizard:**
   - Click "Next" to begin
   - Accept license agreement
   - Choose installation directory (default: `C:\Program Files\Aquiis SimpleStart\`)
   - Create desktop shortcut (recommended)
   - Click "Install"
4. **Launch** from Start Menu or Desktop shortcut

**Option B: Portable Executable (No installation)**

1. **Download** `Aquiis-1.1.0-x64-Portable.exe`
2. **Place** in your desired folder (e.g., `C:\Aquiis\`)
3. **Double-click** to run

**✅ Checkpoint:** Application window should open showing the New Setup Wizard.

---

## 🏢 Step 2: Create Your Organization (2 minutes)

When you first launch Aquiis SimpleStart, the **New Setup Wizard** guides you through initial setup.

### Organization Setup

**On the "Create Organization" screen:**

1. **Organization Name:** Enter your business name
   - Example: "ABC Property Management" or "John Smith Rentals"
2. **Contact Information:**
   - **Phone:** Your business phone number
   - **Email:** Your business email address
   - **Website:** (Optional) Your website URL

3. **Address:**
   - Street address
   - City, State, ZIP code
   - Country

4. **Click** "Create Organization"

**✅ Checkpoint:** You should see "Organization created successfully!" message.

---

## 👤 Step 3: Register Your User Account (2 minutes)

**On the "Register User" screen:**

1. **Personal Information:**
   - **First Name:** Your first name
   - **Last Name:** Your last name

2. **Login Credentials:**
   - **Email:** Your email address (becomes your username)
   - **Password:** Choose a strong password
     - Minimum 8 characters
     - Must contain: uppercase, lowercase, number, special character
   - **Confirm Password:** Re-enter your password

3. **Role:** Administrator (automatically selected for first user)

4. **Click** "Register"

**✅ Checkpoint:** You should be logged in and see the Dashboard.

### First-Time Dashboard

After registration, you'll see the main dashboard with:

- **Navigation menu** on the left
- **Dashboard widgets** showing 0 properties, tenants, leases
- **Welcome message** with quick actions

**Note:** Your account is automatically confirmed in SimpleStart (no email verification step).

---

## 🏠 Step 4: Add Your First Property (3 minutes)

Let's add a rental property to your portfolio.

### Navigate to Properties

1. Click **"Property Management"** in the left navigation menu
2. Click **"Properties"**
3. Click **"Add Property"** button (top-right)

### Enter Property Details

**Basic Information:**

- **Property Name:** Give it a friendly name
  - Example: "123 Main Street House"
- **Address:**
  - **Street Address:** 123 Main Street
  - **City:** Anytown
  - **State/Province:** CA
  - **ZIP/Postal Code:** 12345
  - **Country:** USA

- **Property Type:** Select from dropdown
  - Options: Single Family, Multi-Family, Apartment, Condo, Townhouse
  - Choose: **Single Family**

- **Number of Units:** 1 (for single-family home)

**Financial Information:**

- **Monthly Rent:** $1,500.00
- **Security Deposit:** $1,500.00 (typically equal to one month's rent)

**Property Status:**

- Select: **Available** (ready to rent)

**Description (Optional):**

- Enter a brief description of the property
- Example: "Beautiful 3-bedroom, 2-bathroom single-family home with large backyard. Recently renovated kitchen with modern appliances."

### Save Property

1. Click **"Save"** button at the bottom
2. You'll be redirected to the property list

**✅ Checkpoint:** You should see your property in the list with status "Available".

### Add Property Photo (Optional)

1. Click on your property name to view details
2. Click **"Upload Photo"** button
3. Select an image file (max 10MB)
4. Photo appears in property profile

---

## 👥 Step 5: Add a Tenant (2 minutes)

Now let's add a tenant who will rent this property.

### Navigate to Tenants

1. Click **"Tenant Management"** in left navigation
2. Click **"Tenants"**
3. Click **"Add Tenant"** button

### Enter Tenant Details

**Personal Information:**

- **First Name:** Jane
- **Last Name:** Doe
- **Email:** jane.doe@example.com
- **Phone:** (555) 123-4567

**Current Address (Optional but Recommended):**

- Street Address: 456 Oak Avenue
- City: Anytown
- State: CA
- ZIP: 12345

**Emergency Contact (Optional):**

- Name: John Doe (Spouse)
- Relationship: Spouse
- Phone: (555) 123-4568

### Save Tenant

1. Click **"Save"** button
2. You'll be redirected to the tenant list

**✅ Checkpoint:** You should see Jane Doe in the tenant list.

**Note:** In a real-world scenario, you'd go through the full prospect-to-tenant journey (application, screening, approval). For this Quick Start, we're creating the tenant directly.

---

## 📄 Step 6: Create a Lease (2 minutes)

Let's create a lease agreement between your property and tenant.

### Navigate to Leases

1. Click **"Lease Management"** in left navigation
2. Click **"Leases"**
3. Click **"Create Lease"** button

### Enter Lease Details

**Lease Information:**

- **Property:** Select "123 Main Street House" from dropdown
- **Tenant:** Select "Jane Doe" from dropdown

**Lease Terms:**

- **Start Date:** Choose today's date (or desired move-in date)
  - Example: February 1, 2026
- **End Date:** Choose one year from start date
  - Example: January 31, 2027
- **Monthly Rent:** $1,500.00 (pre-filled from property)
- **Security Deposit:** $1,500.00 (pre-filled from property)
- **Due Day:** 1 (rent due on 1st of each month)

**Payment Terms:**

- **Late Fee Grace Period:** 5 days (rent due 1st, late fee applied on 6th)
- **Late Fee Amount:** $50.00 or 5% of rent
- **Payment Methods Accepted:** Check all that apply
  - ☑ Cash
  - ☑ Check
  - ☑ Credit Card
  - ☑ ACH
  - ☑ Online Portal

### Generate Lease

1. Click **"Generate Lease"** button
2. System creates the lease with status: **Active**
3. Property status automatically changes to: **Occupied**

**✅ Checkpoint:** You should see the new lease in the lease list with status "Active".

### View Lease PDF (Optional)

1. Click on the lease to view details
2. Click **"Download Lease PDF"** button
3. PDF opens showing complete lease agreement with all terms

---

## 💰 Step 7: Generate a Rent Invoice (1 minute)

Let's create the first month's rent invoice.

### Navigate to Invoices

1. Click **"Financial Management"** in left navigation
2. Click **"Invoices"**
3. Click **"Create Invoice"** button

### Enter Invoice Details

**Invoice Information:**

- **Lease:** Select "Jane Doe - 123 Main Street House" from dropdown
- **Invoice Type:** Rent (from dropdown)
- **Description:** "February 2026 Rent"

**Financial Details:**

- **Amount:** $1,500.00 (pre-filled from lease monthly rent)
- **Due Date:** February 1, 2026
- **Issue Date:** Today's date (auto-filled)

**Status:**

- **Invoice Status:** Pending (default)

### Save Invoice

1. Click **"Save"** button
2. Invoice is created and added to the list

**✅ Checkpoint:** You should see the invoice with status "Pending" and due date of February 1, 2026.

**Note:** In production, rent invoices are automatically generated monthly by the background task system. For this Quick Start, we're creating one manually.

---

## 🏦 Step 8: Record a Payment (1 minute)

Now let's record that the tenant paid their rent.

### Navigate to Payments

1. Stay in **"Financial Management"**
2. Click **"Payments"**
3. Click **"Record Payment"** button

### Enter Payment Details

**Payment Information:**

- **Invoice:** Select "February 2026 Rent - Jane Doe" from dropdown
- **Payment Method:** Check (or select what applies)

**Financial Details:**

- **Amount:** $1,500.00 (full rent payment)
- **Payment Date:** Today's date (or actual payment date)
- **Payment Reference:** Check #1234 (optional but recommended)

**Notes (Optional):**

- "Check received and deposited on [date]"

### Save Payment

1. Click **"Save"** button
2. Payment is recorded
3. Invoice status automatically updates to: **Paid**
4. Invoice "Amount Paid" field updates to $1,500.00

**✅ Checkpoint:** When you view the invoice list, the invoice should now show status "Paid" with green indicator.

### View Payment Receipt (Optional)

1. Click on the payment to view details
2. Click **"Generate Receipt"** button
3. PDF receipt generated showing payment confirmation

---

## 🔍 Step 9: Schedule an Inspection (1 minute)

Let's schedule a move-in inspection for the property.

### Navigate to Inspections

1. Click **"Maintenance & Inspections"** in left navigation
2. Click **"Inspections"**
3. Click **"Schedule Inspection"** button

### Enter Inspection Details

**Inspection Information:**

- **Property:** Select "123 Main Street House"
- **Inspection Type:** Move-In (from dropdown)
- **Inspection Date:** Choose today's date or soon after move-in

**Scheduled By:**

- Your name (auto-filled)

**Notes (Optional):**

- "Initial move-in inspection before tenant occupancy"

### Save Inspection

1. Click **"Schedule"** button
2. Inspection is added to calendar and inspection list

**✅ Checkpoint:** You should see the inspection scheduled on the calendar and in the inspection list.

### Complete Inspection (Optional)

Later, when you perform the inspection:

1. Click on the inspection from the list
2. Click **"Start Inspection"** button
3. Go through **26-item checklist:**
   - **Exterior** (4 items): Roof, Siding, Windows, Landscaping
   - **Interior** (6 items): Walls, Floors, Ceilings, Doors, Closets, Light Fixtures
   - **Kitchen** (4 items): Appliances, Cabinets, Countertops, Sink/Plumbing
   - **Bathroom** (4 items): Fixtures, Toilet, Sink, Shower/Tub
   - **Systems** (8 items): HVAC, Electrical, Plumbing, Water Heater, etc.
4. Mark each item: **Pass** / **Fail** / **Needs Repair**
5. Add notes for any issues found
6. Click **"Complete Inspection"**
7. Click **"Generate PDF Report"**

---

## 🎉 Congratulations!

**You've successfully completed the Quick Start Guide!**

In just 15 minutes, you've learned how to:

- ✅ Install Aquiis SimpleStart
- ✅ Create your organization
- ✅ Register your user account
- ✅ Add a property
- ✅ Add a tenant
- ✅ Create a lease
- ✅ Generate a rent invoice
- ✅ Record a payment
- ✅ Schedule an inspection

---

## 🧭 What's Next?

### Explore More Features

**Property Management:**

- Add multiple properties (up to 9 in SimpleStart)
- Upload property documents (certificates, insurance, photos)
- Mark properties as Under Renovation or Off Market
- Track property value and appreciation

**Tenant Workflow:**

- Use the **Prospect-to-Tenant** journey for new tenants:
  1. Add Prospect (inquiry phase)
  2. Schedule Tour
  3. Submit Rental Application
  4. Screen Application (background/credit checks)
  5. Approve/Deny Application
  6. Generate Lease Offer
  7. Tenant Accepts Lease
  8. Automatic Tenant Creation

**Financial Management:**

- Set up **recurring rent invoices** (auto-generated monthly)
- Configure **late fees** (grace period + amount/percentage)
- Generate **financial reports** (income, expenses, payment history)
- Track **security deposits** and annual dividends

**Maintenance & Inspections:**

- Create **maintenance requests** from tenants
- Assign requests to vendors
- Track repair costs and completion
- Schedule **routine inspections** (quarterly, semi-annual, annual)
- Generate inspection reports

**Calendar & Scheduling:**

- View all events in **monthly calendar**
- Schedule **property tours** for prospects
- Track **lease expiration dates**
- Set **payment due date reminders**

**Notifications:**

- Configure **email notifications** (SendGrid)
- Enable **SMS alerts** (Twilio)
- Customize **notification preferences** per user
- View **notification history** in Notification Center

**Background Automation:**

- Let the system automatically:
  - Apply late fees after grace period
  - Send lease expiration warnings (60/30/14 days)
  - Calculate and distribute security deposit dividends
  - Schedule routine inspections
  - Clean up old data

---

## 📚 Additional Resources

### Documentation

- **Release Notes** - What's new in v1.1.0
- **User Guide** - Comprehensive 10-chapter guide covering all features
- **Administrator Guide** - System configuration and management
- **Database Management Guide** - Backup, restore, troubleshooting

### Getting Help

**Support Channels:**

- 📧 **Email Support:** cisguru@outlook.com
- 🐛 **Report Bugs:** [GitHub Issues](https://github.com/xnodeoncode/Aquiis/issues)
- 💡 **Request Features:** [GitHub Discussions](https://github.com/xnodeoncode/Aquiis/discussions)
- 📖 **Documentation:** `/Documentation/v1.1.0/`

**Community:**

- Star the project on GitHub
- Contribute to development
- Share feedback and suggestions

---

## ⚙️ Settings & Configuration

### Essential Settings to Configure

**Organization Settings:**

1. Navigate to **Settings** → **Organization**
2. Update business hours, contact information, logo

**User Settings:**

1. Navigate to **Settings** → **Profile**
2. Update personal information, password, notification preferences

**Notification Settings:**

1. Navigate to **Settings** → **Notifications**
2. Enable/disable email, SMS, in-app notifications
3. Configure SendGrid (email) and Twilio (SMS) API keys if needed

**Database Settings:**

1. Navigate to **Settings** → **Database**
2. Configure **automatic backups:**
   - Enable scheduled backups
   - Set backup frequency (daily, weekly, monthly)
   - Choose backup time (default: 2 AM)
3. Monitor database health and size

**Financial Settings:**

1. Navigate to **Settings** → **Financial**
2. Configure **late fees:**
   - Grace period (default: 5 days)
   - Late fee amount ($50 or 5% of rent)
   - Apply late fees automatically: Yes/No

**Inspection Settings:**

1. Navigate to **Settings** → **Inspections**
2. Configure **routine inspection frequency:**
   - None, Quarterly, Semi-Annual, Annual
3. Enable auto-scheduling of move-in inspections

---

## 🔒 Security & Data Protection

### Best Practices

**Password Management:**

- Use a **strong, unique password**
- Enable **password manager** for secure storage
- Change password periodically

**Data Backups:**

- Enable **automatic daily backups**
- Store backups in **multiple locations** (local + cloud)
- Test restore process periodically

**Session Security:**

- Application auto-locks after **18 minutes of inactivity**
- Always **log out** when leaving computer unattended

**Data Privacy:**

- All tenant data encrypted at rest
- Soft delete enabled (data recoverable if accidentally deleted)
- Audit trails track all data changes (who, when, what)

---

## ❓ Common Questions

### Q: Can I import data from another property management system?

**A:** Not directly in v1.1.0. You'll need to manually enter your properties, tenants, and leases. Data import features are planned for a future release.

### Q: What happens when I reach the 9-property limit?

**A:** When you try to add a 10th property, you'll see an upgrade message explaining that SimpleStart is limited to 9 properties. You can either:

- Remove an inactive property to add a new one
- Upgrade to Aquiis Professional (future release) for unlimited properties

### Q: Can I have more than 3 users?

**A:** No, SimpleStart is limited to 3 users (1 system account + 2 login users). This is a product-level restriction. For more users, you'll need to upgrade to Aquiis Professional when available.

### Q: Is my data stored in the cloud?

**A:** No, Aquiis SimpleStart stores all data **locally on your computer** in a SQLite database file. Your data never leaves your device unless you choose to enable email/SMS notifications or back up to a cloud service.

### Q: Do I need internet access?

**A:** **No** for core features - the application works completely offline. **Yes** for optional features:

- Email notifications (SendGrid)
- SMS notifications (Twilio)
- Future online payment processing

### Q: How do I back up my data?

**A:** Navigate to **Settings** → **Database** → **Backup & Restore**:

- Click **"Create Backup"** for manual backup (recommended before major changes)
- Enable **"Schedule Automatic Backups"** for daily/weekly backups
- Backups stored in: `Data/Backups/` folder
- Copy backups to external drive or cloud storage for safety

### Q: What if I accidentally delete something?

**A:** SimpleStart uses **soft delete** - deleted records are marked as deleted but not permanently removed. Contact support for data recovery assistance if needed.

### Q: Can I run this on a Mac?

**A:** Not in v1.1.0. macOS support is planned for a future release. For now, use a Windows or Linux computer.

### Q: Is there a mobile app?

**A:** Not yet. A mobile companion app (view-only) is planned for a future release.

---

## 🐛 Troubleshooting

### Application won't start

**Windows:**

- Right-click installer → "Run as Administrator"
- Check if antivirus is blocking the application
- Ensure .NET 10 Runtime is installed (bundled with app)

**Linux:**

- Ensure AppImage has execute permissions: `chmod +x Aquiis*.AppImage`
- Check system logs: `journalctl -xe`
- Verify dependencies installed (usually auto-included)

### Database connection error

1. Navigate to **Settings** → **Database**
2. Click **"Check Database Health"**
3. If corrupted, click **"Restore from Backup"**
4. If no backup, click **"Reset Database"** (⚠️ loses all data)

### Can't log in

- Verify email and password are correct
- Check Caps Lock is off
- If forgotten password, use **"Reset Password"** link
- If still stuck, check logs in `Data/Logs/` folder

### Email notifications not working

1. Navigate to **Settings** → **Notifications**
2. Verify SendGrid API key is correct
3. Check SendGrid account is active and not rate-limited
4. View error logs in **Settings** → **System** → **Logs**

### Application is slow

- Check database size: **Settings** → **Database** → **Database Size**
- Run database optimization: **Settings** → **Database** → **Optimize**
- Close other applications to free up memory
- Consider hardware upgrade if below minimum specs

---

## 📞 Support

Need help? We're here for you!

**Email:** cisguru@outlook.com  
**GitHub:** [https://github.com/xnodeoncode/Aquiis](https://github.com/xnodeoncode/Aquiis)

**When contacting support, please include:**

1. **Version number** (Settings → About)
2. **Operating system** (Windows/Linux)
3. **Description of issue** (what happened vs what you expected)
4. **Steps to reproduce** (how to recreate the problem)
5. **Screenshots** (if applicable)
6. **Log files** (Settings → System → Export Logs)

---

## 🎓 Next Steps

**Ready to dive deeper?**

1. **Read the User Guide** - Comprehensive 10-chapter guide covering every feature
2. **Configure automation** - Set up late fees, recurring invoices, backups
3. **Explore reports** - Generate financial and operational reports
4. **Customize settings** - Tailor the application to your workflow
5. **Join the community** - Connect with other landlords using Aquiis

**Thank you for choosing Aquiis SimpleStart!** 🏠

We hope this Quick Start Guide helped you get up and running quickly. Enjoy managing your properties with confidence!

---

**Document Version:** 1.1  
**Last Updated:** February 18, 2026  
**Author:** CIS Guru with GitHub Copilot
