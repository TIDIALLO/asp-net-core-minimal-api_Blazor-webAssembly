using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBibliotheque.Blazor.Models;

namespace GestionBibliotheque.Blazor.Services.Auth
{
    public interface IAuthService
    {
          Task<bool> Login(LoginDTO loginDto);
            Task Logout();
    }
}