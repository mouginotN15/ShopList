using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace TipCalc.Core.Model
{
    public class BillDetail : MvxViewModel
    {
        /// <summary>
        /// Id unique de cette ligne de facture.
        /// </summary>
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Id de la facture qui possède cette "ligne d'achat"
        /// </summary>
        private int _billId;
        public int BillId
        {
            get => _billId;
            set => SetProperty(ref _billId, value);
        }

        /// <summary>
        /// Id de l'item acheté.
        /// </summary>
        private int _itemId;
        public int ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        /// <summary>
        /// Quantité acheté de l'item.
        /// </summary>
        private double _quantity;
        public double Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        /// <summary>
        /// Prix unitaire de l'item.
        /// </summary>
        private double _unitPrice;
        public double UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        /// <summary>
        /// En cas de suppression de la facture.
        /// </summary>
        private bool _deleted;
        public bool Deleted
        {
            get => _deleted;
            set => SetProperty(ref _deleted, value);
        }
    }
}
