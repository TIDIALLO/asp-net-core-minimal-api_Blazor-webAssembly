using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Blazored.LocalStorage;

using GestionBibliotheque.Blazor.Models;

namespace GestionBibliotheque.Blazor.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public BookService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }


        public async Task<List<BookDto>> GetBooksAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var books = await _httpClient.GetFromJsonAsync<IEnumerable<BookDto>>("books");
            return books?.ToList() ?? new List<BookDto>();
                 // var books = await _httpClient.GetFromJsonAsync<List<BookDto>>("/books");
            // return books ?? new List<BookDto>();
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookDto>($"/books/{id}");
        }
        public async Task CreateBookAsync(CreateBookDto newBook)
        {
            //await _httpClient.PostAsJsonAsync("/books", newBook);
            var response = await _httpClient.PostAsJsonAsync("/books", newBook);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateBookAsync(int id, UpdateBookDto updatedBook)
        {
            var response = await _httpClient.PutAsJsonAsync($"/books/{id}", updatedBook);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBookAsync(int id)
        {
            await _httpClient.DeleteAsync($"/books/{id}");
        }

    }
}