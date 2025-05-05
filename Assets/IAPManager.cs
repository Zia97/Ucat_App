using UnityEngine.Purchasing;
using UnityEngine;
using System;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public static string kProductIDSubscription = "monthly_sub"; // Must match store ID

    void Start()
    {
        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(kProductIDSubscription, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuySubscription()
    {
        BuyProductID(kProductIDSubscription);
    }

    void BuyProductID(string productId)
    {
        if (storeController != null && storeController.products != null)
        {
            Product product = storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
            }
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error) => Debug.Log("IAP Init Failed: " + error);

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
        {
            Debug.Log("Subscription purchased!");
            // Grant premium access here
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Purchase failed: " + failureReason);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
