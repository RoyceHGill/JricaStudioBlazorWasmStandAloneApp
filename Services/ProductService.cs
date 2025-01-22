using Blazored.LocalStorage;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components.Forms;
using JricaStudioSharedLibrary.Constants;


using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace JricaStudioApp.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _localStorageService;

        public ProductService(HttpClient httpClient, IManageLocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<IEnumerable<ProductDto>> GetItems()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Products/GetAll");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<ProductDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }

            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<ProductDto> GetItem(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(ProductDto);
                    }

                    return await response.Content.ReadFromJsonAsync<ProductDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }

            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<IEnumerable<ProductDto>> GetRandomProducts(int targetLength)
        {
            try
            {
                var product = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>($"api/Products/GetRandom/{targetLength}");

                

                return product;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ProductDto> GetShowcaseProduct()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Products/GetShowcaseProduct");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(ProductDto);
                    }

                    return await response.Content.ReadFromJsonAsync<ProductDto>();
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return default;
                    }
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<AdminProductDto>> SearchProducts(ProductFilterDto filter)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.PostAsJsonAsync($"api/Products/Search", filter);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new NullReferenceException("No Products");
                    }
                    var products = await response.Content.ReadFromJsonAsync<IEnumerable<AdminProductDto>>();
                    RemoveAdminHeaders();
                    return products;
                }
                RemoveAdminHeaders();
                return default;
            }
            catch (NullReferenceException ne)
            {
                RemoveAdminHeaders();
                return new List<AdminProductDto>();
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }
        }

        public async Task<IEnumerable<AdminProductCategoryDto>> GetCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Products/Categories");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<AdminProductCategoryDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AdminProductDto> DeleteProduct(Guid id)
        {
            try
            {
                await AddAdminHeaders();

                var response = await _httpClient.DeleteAsync($"api/Products/{id}");

                RemoveAdminHeaders();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AdminProductDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }


        private async Task AddAdminHeaders()
        {
            var key = await _localStorageService.GetLocalAdminKeyGuid();

            if (!_httpClient.DefaultRequestHeaders.TryGetValues("AdminKey", out var values))
            {
                _httpClient.DefaultRequestHeaders.Add("AdminKey", key.ToString());
            }
        }

        private void RemoveAdminHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Adminkey");
        }

        public async Task<AdminProductCategoryDto> PostNewProductCategory(AddProductCategoryDto dto)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.PostAsJsonAsync($"api/Products/Category", dto);
                if (response.IsSuccessStatusCode)
                {
                    
                    var category = await response.Content.ReadFromJsonAsync<AdminProductCategoryDto>();
                    RemoveAdminHeaders();
                    return category;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new ArgumentException("Category already exists.");
                }

                RemoveAdminHeaders();
                return default;
            }
            catch (ArgumentException ne)
            {
                RemoveAdminHeaders();
                return new AdminProductCategoryDto();
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }
        }

        public async Task<AdminProductCategoryDto> DeleteProductCategory(Guid id)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.DeleteAsync($"api/Products/Category/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var category = await response.Content.ReadFromJsonAsync<AdminProductCategoryDto>();
                    RemoveAdminHeaders();
                    return category;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentException("Category not found.");
                }

                RemoveAdminHeaders();
                return default;
            }
            catch (ArgumentException ne)
            {
                RemoveAdminHeaders();
                return new AdminProductCategoryDto();
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }
        }

        public async Task<UploadResultDto> UploadProductImage(IBrowserFile file)
        {
            try
            {
                await AddAdminHeaders();

                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(file.OpenReadStream(FileResources.maxuploadSize));

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(fileContent, nameof(file), file.Name);

                var response = await _httpClient.PostAsync("api/Products/ImageUpload", content);
                RemoveAdminHeaders();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UploadResultDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<AdminProductDto> PostNewProduct(AdminProductToAddDto dto)
        {
            try
            {
                await AddAdminHeaders();

                var response = await _httpClient.PostAsJsonAsync<AdminProductToAddDto>("api/Products", dto);
                RemoveAdminHeaders();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AdminProductDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ImageUpdateResultDto> PatchProductImage(Guid id, IBrowserFile image)
        {
            try
            {
                await AddAdminHeaders();

                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(image.OpenReadStream(FileResources.maxuploadSize));

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

                content.Add(new StringContent(id.ToString()), nameof(id));

                content.Add(fileContent, "ImageFile", image.Name);

                var response = await _httpClient.PatchAsync("api/Products", content);

                RemoveAdminHeaders();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ImageUpdateResultDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<EditProductDto> PutProduct(Guid id, EditProductDto dto)
        {
            await AddAdminHeaders();

            var jsonRequest = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PutAsync($"api/Products/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                RemoveAdminHeaders();
                return await response.Content.ReadFromJsonAsync<EditProductDto>();
            }

            else
            {
                RemoveAdminHeaders();
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Http status: {response.StatusCode}Message: {message}");
            }
        }

        public async Task<ProductDto> UpdateProductShowcase(UpdateProductShowcaseDto updateDto)
        {
            try
            {
                await AddAdminHeaders();

                var jsonRequest = JsonConvert.SerializeObject(updateDto);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PutAsync($"api/products/productShowcase", content);
                RemoveAdminHeaders();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductDto>();
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
