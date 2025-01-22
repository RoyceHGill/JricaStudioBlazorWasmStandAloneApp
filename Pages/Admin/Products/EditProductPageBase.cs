using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using JricaStudioSharedLibrary.Constants;

namespace JricaStudioApp.Pages.Admin.Products
{
    public class EditProductPageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public EditProductDto Product { get; set; }
        public string FieldMargins { get; set; } = "mb-4 mt-3";
        public string ImageInputData { get; set; }
        public string CurrentImageData { get; set; }
        public IBrowserFile ImageInput { get; set; }
        public IEnumerable<AdminProductCategoryDto> Categories { get; set; }
        public string ErrorMessage { get; set; }



        public AdminUserLoginDto Admin { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            Categories = await ProductService.GetCategories();
            var product = await ProductService.GetItem(Id);
            Product = new EditProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategoryid = product.CategoryId,
            };
            CurrentImageData = product.ImageData;

        }

        public async Task OnImageLoaded(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var dataBuffer = new byte[file.Size];
            await file.OpenReadStream(maxAllowedSize: FileResources.maxuploadSize).ReadAsync(dataBuffer);
            ImageInputData = $"data:image/png;base64,{Convert.ToBase64String(dataBuffer)}";

            if (file != null)
            {
                try
                {
                    ImageInput = e.File;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task BackToProducts_Click()
        {
            NavigationManager.NavigateTo("/admin/products");
        }

        public async Task SaveEdit_Click()
        {
            try
            {
                if (ImageInput != null)
                {
                    var result = await ProductService.PatchProductImage(Id, ImageInput);
                    if (result == null)
                    {
                        ErrorMessage = "there was an issue uploading the image. ";
                        return;
                    }

                    Product.ImageUploadId = result.UploadedImageResult.Id;
                }

                var serviceUpdateResult = await ProductService.PutProduct(Id, Product);
                if (serviceUpdateResult == null)
                {
                    ErrorMessage = "Unable to update product";
                }

                NavigationManager.NavigateTo("/admin/products");
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                throw;
            }
        }

        public async Task UpdatePreview_Click()
        {
            StateHasChanged();
        }

        protected ProductDto PassProductDto(EditProductDto toAddDto)
        {
            if (ImageInputData == null)
            {
                return new ProductDto()
                {
                    CategoryId = toAddDto.ProductCategoryid,
                    Name = toAddDto.Name,
                    Description = toAddDto.Description,
                    ImageData = CurrentImageData,
                    Quantity = toAddDto.Quantity,
                    Price = toAddDto.Price,
                    CategoryName = Categories.Single(c => c.Id == toAddDto.ProductCategoryid).Name,
                };
            }

            return new ProductDto()
            {
                CategoryId = toAddDto.ProductCategoryid,
                Name = toAddDto.Name,
                Description = toAddDto.Description,
                ImageData = ImageInputData,
                Quantity = toAddDto.Quantity,
                Price = toAddDto.Price,
                CategoryName = Categories.Single(c => c.Id == toAddDto.ProductCategoryid).Name,
            };
        }
    }
}
