using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KataPayDeskTerminal
{
    public class PayDeskTerminal : IPayDeskTerminal
    {
        public double PurchasesValue { get; private set; }
        public List<Product> Products { get; private set; }
        public List<Purchase> PurchasesCollection { get; private set; }
        public string FilePath;
        public char Separator;

        public PayDeskTerminal(string filePathToProductPrices, char separator)
        {
            if (separator == ',')
            {
                throw new Exception("It is not possible to use \",\" as a separator. Use another char. Example of the separator: ;");
            }
            FilePath = filePathToProductPrices;
            Separator = separator;
            Products = new List<Product>();
            PurchasesCollection = new List<Purchase>();
            this.SetPrices();
        }

        public void SetPrices()
        {
            ReadFromFile();
            ValidatePricesAndProduct();
        }

        private void ReadFromFile()
        {
            string[] productWithPrices;
            try
            {
                var fileContent = File.ReadAllText(FilePath);
                productWithPrices = fileContent.Split('\n');
            }
            catch (Exception ex)
            {
                throw new Exception("File with products and prices processing exception",ex);
            }
            LoadProducts(productWithPrices);
        }

        private void LoadProducts(string[] productWithPrices)
        {
            foreach (string s in productWithPrices)
            {
                string[] item = s.TrimEnd('\r').Split(Separator);
                var product = new Product()
                    {
                        ProductName = item[0],
                        PricePerUnit = Convert.ToDouble(item[1]),
                        PackPrice =
                            new KeyValuePair<int, double>(Convert.ToInt32(item[2]), Convert.ToDouble(item[3]))
                    };
                if (Products.All(p => p.ProductName != product.ProductName))
                    Products.Add(product);
                else
                {
                    throw new DuplicateProductException(product.ProductName + " : product name should be unique");
                }
            }
        }


        public void Scan(string productName)
        {
            var product = ValidateProduct(productName);
            DoProductPurchase(product);
        }

        private void DoProductPurchase(Product product)
        {
            AddIfNotExistInPurchases(product);
            UpdatePurchasesValue(product);
        }

        private void UpdatePurchasesValue(Product product)
        {
            var purchasedProduct = PurchasesCollection.First(p => p.ProductName == product.ProductName);
            purchasedProduct.RestQuantity++;
            if (purchasedProduct.RestQuantity >= purchasedProduct.PackPrice.Key)
            {
                purchasedProduct.BulkQuantity++;
                PurchasesValue -= (purchasedProduct.RestQuantity - 1) * purchasedProduct.PricePerUnit;
                PurchasesValue += purchasedProduct.PackPrice.Value;
                purchasedProduct.RestQuantity = purchasedProduct.RestQuantity - purchasedProduct.PackPrice.Key;
            }
            else
            {
                PurchasesValue += purchasedProduct.PricePerUnit;
            }
        }

        private void AddIfNotExistInPurchases(Product product)
        {
            if (PurchasesCollection.All(pc => pc.ProductName != product.ProductName))
            {
                PurchasesCollection.Add(new Purchase()
                    {
                        PricePerUnit = product.PricePerUnit,
                        ProductName = product.ProductName,
                        PackPrice = product.PackPrice
                    });
            }
        }

        private Product ValidateProduct(string productName)
        {
            //add some other business rules here
            if (Products.All(p => p.ProductName != productName))
                throw new ProductNotExistException(productName + " : no such product in general productList");
            return Products.FirstOrDefault(p => p.ProductName == productName);
        }

        public double Calculate()
        {
            ValidateTotalSum();
            return PurchasesValue;
        }

        private void ValidateTotalSum()
        {
            //add some other business rules here
            if (!PurchasesCollection.Any())
                throw new ArgumentException("Purchases Collection is empty");
        }

        public bool ValidatePricesAndProduct()
        {
            //add some other business rules here
            if (Products.Any(p => p.PricePerUnit <= 0 || p.PackPrice.Key <= 0 || p.PackPrice.Value <= 0))
            {
                throw new PricesValidationException(String.Format("Price should be more then 0"));
            }
            foreach (var product in Products.Where(product => product.PackPrice.Value / product.PackPrice.Key > product.PricePerUnit))
            {
                throw new PricesValidationException(String.Format("Price Per unit in pack price is higher then price per unit in product {0}", product.ProductName));
            }
            return true;
        }

    }


    public class Purchase : Product
    {
        public int BulkQuantity = 0;
        public int RestQuantity = 0;
        public double PurchasedValue { get { return (BulkQuantity * PackPrice.Value + RestQuantity * PricePerUnit); } }
    }
}
