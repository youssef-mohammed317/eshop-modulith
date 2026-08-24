using System;
using System.Collections.Generic;
using Catalog.Products.Models;

namespace Catalog.Infrastructure.Data.Seeding;

public static class InitialData
{
    public static IEnumerable<Product> Products
    {
        get
        {
            var laptop = Product.Create(
                id: Guid.NewGuid(),
                name: "Dell Precision 3541",
                description: "Mobile workstation optimized for heavy backend development and containerization.",
                imageFile: "dell-precision-3541.png",
                price: 1250.00m
            );
            laptop.AddCategory("Laptops");
            laptop.AddCategory("Workstations");

            var smartphone = Product.Create(
                id: Guid.NewGuid(),
                name: "Samsung Galaxy A56 5G",
                description: "Fast, reliable 5G smartphone with excellent battery life for daily use.",
                imageFile: "samsung-a56.png",
                price: 450.00m
            );
            smartphone.AddCategory("Smartphones");
            smartphone.AddCategory("Electronics");

            var monitor = Product.Create(
                id: Guid.NewGuid(),
                name: "Acer B246HYL 24-inch Monitor",
                description: "1080p IPS monitor perfect for dual-screen development setups.",
                imageFile: "acer-b246hyl.png",
                price: 120.00m
            );
            monitor.AddCategory("Monitors");
            monitor.AddCategory("Accessories");

            return new List<Product> { laptop, smartphone, monitor };
        }
    }
}