using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TipCalc.Core.Services
{
    // Permet de faire les appels API.
    public class API
    {
        public BillDetailsClient BillDetailsClient;
        public BillsClient BillsClient;
        public ShopItemsClient ShopItemsClient;
        public ShopListsClient ShopListsClient;
        public UsersClient UsersClient;


        public API()
        {
            // Valide tout les certificats ssl même si il ne sont pas conforme. (auto-signé par exemple ... ^^')
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            HttpClient client = new HttpClient(handler);

            BillDetailsClient = new BillDetailsClient(client);
            BillsClient = new BillsClient(client);
            ShopItemsClient = new ShopItemsClient(client);
            ShopListsClient = new ShopListsClient(client);
            UsersClient = new UsersClient(client);


        }
    }
}
