﻿using System.ComponentModel.DataAnnotations;

namespace UsuarioApi.Data.Dtos;

public class CreateUsuarioDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public DateTime DataNascimento { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [Compare("Password")]
    public string PasswordConfirm { get; set; }
}
