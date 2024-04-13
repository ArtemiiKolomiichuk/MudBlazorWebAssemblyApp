namespace MudBlazorWebAssemblyApp.Client
{
    using Microsoft.JSInterop;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;

    public class LocalStorageService(IJSRuntime jsRuntime)
    {
        public readonly IJSRuntime _jsRuntime = jsRuntime;

        public async Task AddItem(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task<string> GetItem(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
    }

    public class AuthService(LocalStorageService _localStorage)
    {
        private readonly LocalStorageService localStorage = _localStorage;

        private string token = "";
        private string userName = "";

        private bool initialized = false;
        public async Task Init()
        {
            if (initialized)
                return;
            try
            {
                token = await localStorage.GetItem("token");
                userName = await localStorage.GetItem("userName");
            }
            catch{ }
        }

        public HttpClient Client
        {
            get
            {
                Init();
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                return http;
            }
        }

        public string UserName
        {
            get
            {
                Init();
                return userName;
            }
        }

        public void LogOut()
        {
            initialized = true;
            _ = localStorage.RemoveItem("token");
            _ = localStorage.RemoveItem("userName");
            token = "";
            userName = "";
        }

        public async Task<bool> LogIn(string login, string password)
        {
            initialized = true;
            token = await localStorage._jsRuntime.InvokeAsync<string>("window.btoa", $"{login}:{password}");
            var response = await Client.PostAsync("http://localhost:3000/login", null);
            if(!response.IsSuccessStatusCode)
            {
                token = "";
                return false;
            }
            userName = login;
            await localStorage.AddItem("token", token);
            await localStorage.AddItem("userName", userName);
            return true;
        }

        public async Task<bool> Register(string username, string password)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsync("http://localhost:3000/register", JsonContent.Create(new { username, password }));
            return response.IsSuccessStatusCode;
        }
    }
}
