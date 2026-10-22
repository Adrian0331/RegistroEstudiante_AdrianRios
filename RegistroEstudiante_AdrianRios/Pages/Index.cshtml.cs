using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace RegistroEstudiante_AdrianRios.Pages
{
    public class IndexModel : PageModel
    {
        public string MensajeSistema { get; set; } = "";

        public class Estudiante
        {
            public int ID { get; set; } 
            public string Nombre { get; set; } = "";
            public string Apellido { get; set; } = "";
        }

        public List<Estudiante> ListaEstudiantes { get; set; } = new List<Estudiante>();

        string cadenaConexion = "Server=localhost\\SQLEXPRESS01;Database=EstudianteDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public void OnGet()
        {
            try{
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string consultaSql = "SELECT Id, Nombre, Apellido FROM Estudiante";
                    using (SqlCommand comando = new SqlCommand(consultaSql, conexion))
                    {
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                Estudiante estudiante = new Estudiante
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Apellido = lector.GetString(2)
                                };
                                ListaEstudiantes.Add(estudiante);
                            }
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                MensajeSistema = "Falla crítica en Base de Datos: " + ex.Message;
            }
        }

        public IActionResult OnPost(string nombreForm, string apellidoForm, int estudianteId, string accion )
        {

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                { 
                    conexion.Open();

                    if (accion == "guardar")
                    {
                        if (string.IsNullOrEmpty(nombreForm) || string.IsNullOrEmpty(apellidoForm))
                        {
                            MensajeSistema = "Error: Debes llenar ambos campos.";
                            return Page();
                        }

                        string consultaSql = "INSERT INTO Estudiante (Nombre, Apellido) VALUES (@nom, @ape)";
                        using (SqlCommand comando = new SqlCommand(consultaSql, conexion))
                        {
                            {
                                comando.Parameters.AddWithValue("@nom", nombreForm);
                                comando.Parameters.AddWithValue("@ape", apellidoForm);
                                comando.ExecuteNonQuery();
                            }
                        }
                        MensajeSistema = $"¡Éxito! El estudiante {nombreForm} {apellidoForm} fue guardado en SQL Server.";
                    }
                    else if (accion == "borrar_individual")
                    {
                        string consultaSQL = "DELETE FROM Estudiante WHERE Id = @id";
                        using (SqlCommand comando = new SqlCommand(consultaSQL, conexion))
                        {
                            comando.Parameters.AddWithValue("@id", estudianteId);
                            comando.ExecuteNonQuery();
                        }
                        MensajeSistema = $"Registro con ID {estudianteId} eliminado correctamente.";

                    }
                    else if (accion == "vaciar_tabla")
                {
                    string consultaSQL = "DELETE FROM Estudiante";
                    using (SqlCommand comando = new SqlCommand(consultaSQL, conexion))
                        {
                            comando.ExecuteNonQuery();
                        }
                        MensajeSistema = "Se han borrado todos los registros de la tabla Estudiates.";
                    }
                }
               return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                MensajeSistema = "Falla crítica en Base de Datos: " + ex.Message;
                return Page();
            }
        }
    }
}
