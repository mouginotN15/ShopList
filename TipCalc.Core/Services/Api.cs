using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TipCalc.Core.Services
{
    // Permet de faire les appels API.
    public class API
    {
        public UsersClient UsersClient;
        public ShopListsClient ShopListsClient;
        public ShopItemsClient ShopItemsClient;

        public API()
        {
            // Valide tout les certificats ssl même si il ne sont pas conforme. (auto-signé par exemple ... ^^')
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            HttpClient client = new HttpClient(handler);
            UsersClient = new UsersClient(client);
            ShopListsClient = new ShopListsClient(client);
            ShopItemsClient = new ShopItemsClient(client);
        }
    }
}
