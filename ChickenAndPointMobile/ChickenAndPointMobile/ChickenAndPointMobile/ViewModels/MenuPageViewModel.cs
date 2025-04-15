using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChickenAndPointMobile.Models;
using Xamarin.Forms;

namespace ChickenAndPointMobile.ViewModels
{
    public class MenuCategoryGroup : List<DishViewModel>
    {
        public string CategoryName { get; private set; }
        public Guid CategoryId { get; private set; }
        public MenuCategoryGroup(string categoryName, Guid categoryId, List<DishViewModel> dishes) : base(dishes)
        {
            CategoryName = categoryName;
            CategoryId = categoryId;
        }
    }

    public class MenuPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MenuCategoryGroup> _groupedMenu;
        public ObservableCollection<MenuCategoryGroup> GroupedMenu
        {
            get => _groupedMenu;
            set => SetProperty(ref _groupedMenu, value);
        }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        private string _loadingStatus;
        public string LoadingStatus { get => _loadingStatus; set => SetProperty(ref _loadingStatus, value); }

        public Command LoadMenuCommand { get; }

        public MenuPageViewModel()
        {
            GroupedMenu = new ObservableCollection<MenuCategoryGroup>();
            LoadMenuCommand = new Command(async () => await ExecuteLoadMenuCommand());
        }

        public async Task ExecuteLoadMenuCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            LoadingStatus = "Загрузка меню...";

            try
            {
                if (GroupedMenu.Any()) GroupedMenu.Clear();

                var categoriesTask = App.SupabaseClient.From<Категории>().Select("*").Get();
                var dishesTask = App.SupabaseClient.From<Блюда>().Select("*").Get();
                await Task.WhenAll(categoriesTask, dishesTask);

                var categoriesResponse = categoriesTask.Result;
                var dishesResponse = dishesTask.Result;

                if (categoriesResponse?.Models == null || dishesResponse?.Models == null)
                {
                    LoadingStatus = "Ошибка загрузки данных."; IsBusy = false; return;
                }

                var allCategories = categoriesResponse.Models.OrderBy(c => c.НазваниеКатегории).ToList();
                var allAvailableDishes = dishesResponse.Models
                                           .Where(d => d.Доступно)
                                           .ToList();

                if (!allCategories.Any())
                {
                    LoadingStatus = "Категории не найдены."; IsBusy = false; return;
                }

                var tempGroupedMenu = new List<MenuCategoryGroup>();
                foreach (var category in allCategories)
                {
                    var dishesInCategory = allAvailableDishes
                                           .Where(d => d.IdКатегории == category.Id)
                                           .OrderBy(d => d.Цена)
                                           .Select(dish => new DishViewModel(dish))
                                           .ToList();

                    if (dishesInCategory.Any())
                    {
                        tempGroupedMenu.Add(new MenuCategoryGroup(category.НазваниеКатегории, category.Id, dishesInCategory));
                    }
                }
                GroupedMenu = new ObservableCollection<MenuCategoryGroup>(tempGroupedMenu);

                if (!GroupedMenu.Any()) { LoadingStatus = "Нет доступных блюд в меню."; }
                else { LoadingStatus = string.Empty; }

            }
            catch (Exception ex)
            {
                LoadingStatus = $"Ошибка: {ex.Message}"; System.Diagnostics.Debug.WriteLine($"Menu Loading Error: {ex}");
            }
            finally { IsBusy = false; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}