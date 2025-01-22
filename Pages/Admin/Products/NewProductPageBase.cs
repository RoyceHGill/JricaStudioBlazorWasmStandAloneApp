using Blazorise;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using JricaStudioSharedLibrary.Constants;
using System.Runtime.CompilerServices;

namespace JricaStudioApp.Pages.Admin.Products
{
    public class NewProductPageBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public AdminProductToAddDto Product { get; set; }
        public IBrowserFile Image { get; set; }
        public string ImageData { get; set; }
        public string FieldMargins { get; set; } = "mb-4 mt-3";
        public string ErrorMessage { get; set; }
        public IEnumerable<AdminProductCategoryDto> Categories { get; set; }

        public async Task OnImageLoaded(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var dataBuffer = new byte[file.Size];
            await file.OpenReadStream(maxAllowedSize: FileResources.maxuploadSize).ReadAsync(dataBuffer);
            ImageData = $"data:image/png;base64,{Convert.ToBase64String(dataBuffer)}";

            if (file != null)
            {
                try
                {
                    Image = e.File;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            Product = new();
            Categories = await ProductService.GetCategories();
        }

        protected async Task UpdatePreview_Click()
        {
            StateHasChanged();
        }

        protected async Task SaveProduct_Click()
        {
            try
            {
                if (Image != null)
                {
                    var uploadResult = await ProductService.UploadProductImage(Image);
                    if (uploadResult == null)
                    {
                        ErrorMessage = "There was an issue uploading the image";
                        return;
                    }

                    Product.ImageUploadId = uploadResult.Id;

                    var product = await ProductService.PostNewProduct(Product);

                    NavigationManager.NavigateTo("admin/products");
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        protected ProductDto PassProductDto(AdminProductToAddDto toAddDto)
        {
            return new ProductDto()
            {
                CategoryId = toAddDto.ProductCategoryid,
                Name = toAddDto.Name,
                Description = toAddDto.Description,
                ImageData = ImageData,
                Quantity = toAddDto.Quantity,
                Price = toAddDto.Price,
                CategoryName = Categories.Single(c => c.Id == toAddDto.ProductCategoryid).Name,
            };
        }

    }
}
