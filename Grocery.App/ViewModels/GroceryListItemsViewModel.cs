using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using Grocery.Core.Data.Repositories;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class GroceryListItemsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IProductService _productService;
        public ObservableCollection<GroceryListItem> MyGroceryListItems { get; set; } = [];
        public ObservableCollection<Product> AvailableProducts { get; set; } = [];

        [ObservableProperty]
        GroceryList groceryList = new(0, "None", DateOnly.MinValue, "", 0);

        public GroceryListItemsViewModel(IGroceryListItemsService groceryListItemsService, IProductService productService)
        {
            _groceryListItemsService = groceryListItemsService;
            _productService = productService;
            Load(groceryList.Id);
        }

        private void Load(int id)
        {
            MyGroceryListItems.Clear();
            foreach (var item in _groceryListItemsService.GetAllOnGroceryListId(id)) MyGroceryListItems.Add(item);
            GetAvailableProducts();
        }

        private void GetAvailableProducts()
        {
            AvailableProducts.Clear();    //Maak de lijst AvailableProducts leeg
            foreach (Product product in _productService.GetAll()) //Haal de lijst met producten op
            {
                if (product.Stock > 0 && !MyGroceryListItems.Any(item => item.ProductId == product.Id)) //Controleer of de voorraad (Stock) groter is dan nul en dat het product nog niet op de boodschappenlijst staat
                {
                    AvailableProducts.Add(product); //Als dat zo is, voeg het product toe aan de AvailableProducts lijst
                }
            }
            
        }

        partial void OnGroceryListChanged(GroceryList value)
        {
            Load(value.Id);
        }

        [RelayCommand]
        public async Task ChangeColor()
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), GroceryList } };
            await Shell.Current.GoToAsync($"{nameof(ChangeColorView)}?Name={GroceryList.Name}", true, paramater);
        }
        [RelayCommand]
        public void AddProduct(Product product)
        {
            if (product != null && product.Id > 0) //Controleer of het product bestaat en dat de Id > 0
            {
                GroceryListItem newItem = new GroceryListItem(0, GroceryList.Id, product.Id, 1); //Maak een GroceryListItem met Id 0 en vul de juiste productid en grocerylistid
                _groceryListItemsService.Add(newItem); //Voeg het GroceryListItem toe aan de dataset middels de _groceryListItemsService
                product.Stock--; //Werk de voorraad (Stock) van het product bij
                _productService.Update(product); //Werk het product bij middels de _productService
                GetAvailableProducts();  //Werk de lijst AvailableProducts bij, want dit product is niet meer beschikbaar
                OnGroceryListChanged(GroceryList); //call OnGroceryListChanged(GroceryList);
            }
        }
    }
}
