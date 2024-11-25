﻿using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class LoginRequestDto
    {
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
