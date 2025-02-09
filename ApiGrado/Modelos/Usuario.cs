﻿using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NombreUsuario { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public int NumeroCelular { get; set; }
        [Required]
        public string Direccion { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public RolEnum Rol { get; set; }
    }
}
