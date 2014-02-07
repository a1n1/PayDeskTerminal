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
        public DiscountCard DiscountCard { get; private set; }
        public string PathToCardStorage;
        private const string DefaultCardId = "defaultCard";
        private bool PurchaseIsOpen = false;

        public PayDeskTerminal(string filePathToProductPrices, char separator, string pathToCardsStorage)
        {
            if (separator == ',')
            {
                throw new Exception("It is not possible to use \",\" as a separator. Use another char. Example of the separator: ;");
            }
            FilePath = filePathToProductPrices;
            Separator = separator;
            Products = new List<Product>();
            PurchasesCollection = new List<Purchase>();
            DiscountCard = new DiscountCard() { CardId = DefaultCardId, DiscountPercent = 0, PurchasesSum = 0 };
            this.SetPrices();
            PathToCardStorage = pathToCardsStorage;
            PurchaseIsOpen = true;

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
                throw new Exception("File with products and prices processing exception", ex);
            }
            LoadProducts(productWithPrices);
        }


        private DiscountCard LookupDiscountCard(string cardId)
        {
            var discountCards = DiscountCardRepository.GetDiscountCards(PathToCardStorage);
            var discountCard = discountCards.FirstOrDefault(c => c.CardId == cardId);
            if (discountCard == null)
            {
                throw new DiscountCardNotFoundException("No such discountcard in the Discount Cards Storage");
            }
            return discountCard;
        }

        private void ValidateDiscountCard(DiscountCard discountCard)
        {
            //add some business rules
            if (discountCard.DiscountPercent > 99 || discountCard.PurchasesSum < 0 || discountCard.CardId == DefaultCardId)
            {
                throw new Exception("Entered discount card is not valid");
            }
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
                product.PackPriceAvailable = product.PackPrice.Key > 1;
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
            if (!PurchaseIsOpen) return;
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
            if (purchasedProduct.RestQuantity >= purchasedProduct.PackPrice.Key && purchasedProduct.PackPriceAvailable)
            {
                purchasedProduct.BulkQuantity++;
                PurchasesValue -= DiscountCard.DiscountMultiplicator * (purchasedProduct.RestQuantity - 1) * purchasedProduct.PricePerUnit;
                PurchasesValue += purchasedProduct.PackPrice.Value;
                purchasedProduct.RestQuantity = purchasedProduct.RestQuantity - purchasedProduct.PackPrice.Key;
            }
            else
            {
                PurchasesValue += DiscountCard.DiscountMultiplicator * purchasedProduct.PricePerUnit;
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
                        PackPrice = product.PackPrice,
                        PackPriceAvailable = (product.PackPrice.Key > 1)
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
            if (PurchaseIsOpen)
            {
                UpdateDiscountCardBalance();
                PurchaseIsOpen = false;    
            }
            return PurchasesValue;
        }

        public void UseDiscountCard(string cardId)
        {
            if (DiscountCard.CardId != DefaultCardId || !PurchaseIsOpen) return;
            var discountCard = LookupDiscountCard(cardId);
            ValidateDiscountCard(discountCard);
            DiscountCard = discountCard;
            PurchasesValue -= (DiscountCard.DiscountPercent) * (PurchasesCollection.Sum(p => p.RestQuantity * p.PricePerUnit)) / 100;
        }

        private void UpdateDiscountCardBalance()
        {
            if (DiscountCard.CardId == DefaultCardId || !PurchaseIsOpen) return;
            DiscountCard.PurchasesSum +=
                PurchasesValue +
                (DiscountCard.DiscountPercent) * PurchasesCollection.Sum(p => p.RestQuantity * p.PricePerUnit) / 100;
            
            var discountCards = DiscountCardRepository.GetDiscountCards(PathToCardStorage);
            discountCards.ForEach(c =>
                { if (c.CardId == DiscountCard.CardId) c.PurchasesSum = DiscountCard.PurchasesSum; });
            
            DiscountCardRepository.WriteDiscountCards(discountCards, PathToCardStorage);
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
