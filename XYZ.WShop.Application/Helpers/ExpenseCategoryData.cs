using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Expense;

namespace XYZ.WShop.Application.Helpers
{
    public static class ExpenseCategoryData
    {
        public static List<ExpenseCategory> GetCategories() => new List<ExpenseCategory>
    {
        new ExpenseCategory { Id = "inventory", Label = "📦 Inventory Purchase" },
        new ExpenseCategory { Id = "incoming_shipping", Label = "🚚 Shipping (to you)" },
        new ExpenseCategory { Id = "packaging", Label = "📦 Packaging Materials" },
        new ExpenseCategory { Id = "customization", Label = "🎨 Product Customization" },
        new ExpenseCategory { Id = "restocking_fee", Label = "💸 Restocking Fees" },
        new ExpenseCategory { Id = "customer_shipping", Label = "🚛 Delivery to Customers" },
        new ExpenseCategory { Id = "handling_fee", Label = "📥 Handling Fees" },
        new ExpenseCategory { Id = "courier_tip", Label = "💰 Courier Tips / Extras" },
        new ExpenseCategory { Id = "ads", Label = "📢 Social Media Ads" },
        new ExpenseCategory { Id = "influencer", Label = "🤳 Influencer Payments" },
        new ExpenseCategory { Id = "discounts", Label = "🏷️ Promo Discounts Given" },
        new ExpenseCategory { Id = "branding", Label = "🖼️ Branding / Logo Design" },
        new ExpenseCategory { Id = "flyers", Label = "🧾 Flyers / Banners" },
        new ExpenseCategory { Id = "app_subscription", Label = "📲 App Subscription" },
        new ExpenseCategory { Id = "data", Label = "🌐 Data / Internet Bundles" },
        new ExpenseCategory { Id = "domain", Label = "🔗 Website or Domain Fees" },
        new ExpenseCategory { Id = "whatsapp_tools", Label = "🛠️ WhatsApp Business Tools" },
        new ExpenseCategory { Id = "assistant", Label = "👩‍💼 Assistant / Staff Payment" },
        new ExpenseCategory { Id = "reseller_commission", Label = "🤝 Reseller Commission" },
        new ExpenseCategory { Id = "freelancer", Label = "🎥 Freelance Services" },
        new ExpenseCategory { Id = "rent", Label = "🏠 Shop Rent / Space Fee" },
        new ExpenseCategory { Id = "electricity", Label = "⚡ Electricity / Generator Fuel" },
        new ExpenseCategory { Id = "bank_fees", Label = "🏦 Bank Fees / POS Charges" },
        new ExpenseCategory { Id = "transport", Label = "🚌 Transport / Movement" },
        new ExpenseCategory { Id = "loan", Label = "📉 Loan Repayments" },
        new ExpenseCategory { Id = "repairs", Label = "🛠️ Repairs / Damaged Goods" },
        new ExpenseCategory { Id = "training", Label = "📚 Training / Course Fees" },
        new ExpenseCategory { Id = "gifts", Label = "🎁 Gifts / Customer Appreciation" },
        new ExpenseCategory { Id = "subscriptions", Label = "🔄 Other Subscriptions" },
        new ExpenseCategory { Id = "other", Label = "❓ Other Expenses" }
    };
    }
}
