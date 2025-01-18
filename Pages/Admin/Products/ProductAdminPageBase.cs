using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;


namespace JricaStudioApp.Pages.Admin.Products
{
    public class ProductAdminPageBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public AdminUserLoginDto Admin { get; set; }
        public ProductFilterDto Filter { get; set; }
        public AddProductCategoryDto CategoryToAdd { get; set; }
        public Guid CategoryIdToDelete { get; set; }
        public IEnumerable<AdminProductDto> Products { get; set; }
        public IEnumerable<AdminProductCategoryDto> ProductCategories { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public Guid CategoryNameInputFieldId { get; set; } = Guid.NewGuid();
        public Guid CategoryNameInputButtonId { get; set; } = Guid.NewGuid();
        public Guid CategoryDeleteDropDownInputId { get; set; } = Guid.NewGuid();
        public Guid CategoryDeleteConfirmationButtonId { get; set; } = Guid.NewGuid();
        public string AddCategoryErrorMessage { get; set; }
        public string AddCategoryConfirmationMessage { get; set; }
        public string DeleteCategoryErrorMessage { get; set; }
        public string DeleteCategoryConfirmationMessage { get; set; }
        public string ErrorMessage { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            Filter = new ProductFilterDto();
            ProductCategories = await ProductService.GetCategories();
            Products = await ProductService.SearchProducts(Filter);
            CategoryToAdd = new();
        }

        protected async Task Enter(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                    await SearchProducts_Click();
                    break;
            }
        }

        protected async Task AddNewProductCategory_Click()
        {
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryNameInputButtonId);
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryNameInputFieldId);
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryDeleteDropDownInputId);
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryDeleteConfirmationButtonId);
            AddCategoryConfirmationMessage = string.Empty;
            AddCategoryErrorMessage = string.Empty;
            DeleteCategoryConfirmationMessage = string.Empty;
            DeleteCategoryErrorMessage = string.Empty;
            StateHasChanged();
        }

        protected async Task DeleteProductCategory_Click()
        {
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputButtonId);
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputFieldId);
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryDeleteDropDownInputId);
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryDeleteConfirmationButtonId);
            AddCategoryConfirmationMessage = string.Empty;
            AddCategoryErrorMessage = string.Empty;
            DeleteCategoryConfirmationMessage = string.Empty;
            DeleteCategoryErrorMessage = string.Empty;
            StateHasChanged();
        }

        protected async Task ConfirmCategoryDeletion()
        {
            try
            {
                if (CategoryIdToDelete == Guid.Empty )
                {
                    DeleteCategoryErrorMessage = "Please Select a Category.";
                    return;
                }
                var result = await ProductService.DeleteProductCategory(CategoryIdToDelete);
                if (result == null)
                {
                    DeleteCategoryErrorMessage = "The Category was not delete.";
                    return;
                }
                CategoryIdToDelete = default;
                DeleteCategoryConfirmationMessage = "Category Successfully Deleted";
                ProductCategories = await ProductService.GetCategories();
                StateHasChanged();
            }
            catch (Exception e)
            {
                DeleteCategoryErrorMessage = e.Message;
            }
        }

        protected async Task SearchProducts_Click()
        {
            var products = await ProductService.SearchProducts(Filter);

            Products = products.OrderBy(s => s.Name).ToList();
            StateHasChanged();
        }

        protected async Task AddNewProduct_Click()
        {
            NavigationManager.NavigateTo("/admin/products/new");
        }

        protected async Task EditProduct_Click(Guid id)
        {
            NavigationManager.NavigateTo($"admin/products/edit/{id}");
        }


        protected async Task SaveNewProductCategory()
        {
            try
            {
                var result = await ProductService.PostNewProductCategory(CategoryToAdd);

                if (result != null)
                {
                    CategoryToAdd = new();
                    await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputButtonId);
                    await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputFieldId);
                    ProductCategories = await ProductService.GetCategories();
                    AddCategoryConfirmationMessage = "Category Successfully Added";
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                AddCategoryErrorMessage = e.Message;
            }
        }

        protected async Task DeleteProduct_Click(Guid id)
        {
            try
            {
                var result = await ProductService.DeleteProduct(id);

                if (result != null)
                {
                    NavigationManager.NavigateTo("/admin/products");
                    Products = await ProductService.SearchProducts(Filter);
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}
