using System;
using System.Collections.Generic;
using FougeraClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FougeraClub.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            // Apply pending migrations first
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // No manual ADO.NET probing here; EF Core handles connection lifecycle.

            if (!await context.Suppliers.AnyAsync())
            {
                logger.LogInformation("Seeding suppliers...");

                var suppliers = new List<Supplier>
                {
                    new Supplier
                    {
                        Name        = "شركة الأمل للتوريدات",
                        ContactInfo = "info@amal-supplies.com",
                        Phone       = "0501234567",
                        Address     = "الرياض، حي العليا، شارع الملك فهد"
                    },
                    new Supplier
                    {
                        Name        = "المؤسسة العالمية للتجهيزات",
                        ContactInfo = "contact@global-equip.com",
                        Phone       = "0559876543",
                        Address     = "جدة، حي الروضة، طريق المدينة المنورة"
                    },
                    new Supplier
                    {
                        Name        = "مؤسسة سمارت فيجن",
                        ContactInfo = "hello@smartvision.sa",
                        Phone       = "0531122334",
                        Address     = "الدمام، حي الشاطئ، شارع الخليج"
                    },
                    new Supplier
                    {
                        Name        = "شركة النور للمستلزمات",
                        ContactInfo = "support@alnour.com",
                        Phone       = "0544455667",
                        Address     = "مكة المكرمة، حي العزيزية، الطريق الدائري"
                    },
                    new Supplier
                    {
                        Name        = "مجموعة الخليج للتجارة",
                        ContactInfo = "info@gulf-trading.com",
                        Phone       = "0567788990",
                        Address     = "المدينة المنورة، حي قباء، شارع الهجرة"
                    }
                };

                await context.Suppliers.AddRangeAsync(suppliers);
                try
                {
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} suppliers successfully.", suppliers.Count);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    logger.LogError(dbEx, "DbUpdateException while seeding suppliers: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected exception while seeding suppliers: {Message}", ex.Message);
                }
            }
            else
            {
                logger.LogInformation("Suppliers table already has data — skipping seed.");
            }
        }
    }
}
