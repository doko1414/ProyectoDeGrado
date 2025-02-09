﻿using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class UsuarioActualizarDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los Apellidos son obligatorios")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El Numero de celular es obligatorio")]
        public int NumeroCelular { get; set; }

        [Required(ErrorMessage = "La Dirección es obligatoria")]
        public string Direccion { get; set; }

        public string Email { get; set; }

        // La contraseña ya no es obligatoria para la actualización
        public string Password { get; set; }
    }
}
