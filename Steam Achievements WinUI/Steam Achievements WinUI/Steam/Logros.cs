using Herramientas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Steam
{
    public static class Logros
    {
        public static async void ComprobarCarpetaUsuario(string idJugador)
        {
            StorageFolder carpetaJugadores = ApplicationData.Current.LocalFolder;

            try
            {
                await carpetaJugadores.GetFolderAsync(idJugador);
            }
            catch
            {
                await carpetaJugadores.CreateFolderAsync(idJugador, CreationCollisionOption.ReplaceExisting);
            }
        }

        public static async Task<List<Logro>> ComprobarJuego(string idJugador, string idJuego)
        {
            List<Logro> logros = new List<Logro>();
            StorageFolder carpetaJugador = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\" + idJugador);

            string htmlJugador = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + idJugador + "&appid=" + idJuego);
        
            if (htmlJugador != null)
            {
                SteamJugadorLogrosAPI jugadorLogrosAPI = JsonConvert.DeserializeObject<SteamJugadorLogrosAPI>(htmlJugador);

                if (jugadorLogrosAPI != null) 
                {
                    if (jugadorLogrosAPI.Datos != null)
                    {
                        if (jugadorLogrosAPI.Datos.Logros.Count > 0)
                        {
                            StorageFile ficheroJuegoLogros = null;

                            try
                            {
                                ficheroJuegoLogros = await carpetaJugador.GetFileAsync(idJuego + ".json");
                            }
                            catch 
                            {
                                ficheroJuegoLogros = await carpetaJugador.CreateFileAsync(idJuego + ".json");
                            }

                            if (ficheroJuegoLogros != null)
                            {
                                string textoJuegoLogros = await FileIO.ReadTextAsync(ficheroJuegoLogros);

                                if (textoJuegoLogros != null)
                                {
                                    logros = JsonConvert.DeserializeObject<List<Logro>>(textoJuegoLogros);
                                }

                                if (logros.Count == 0)
                                {
                                    string htmlLogros = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&appid=" + idJuego);
                                
                                    if (htmlLogros != null)
                                    {
                                        SteamJuegoLogrosAPI juegoLogrosAPI = JsonConvert.DeserializeObject<SteamJuegoLogrosAPI>(htmlLogros);

                                        if (juegoLogrosAPI != null)
                                        {
                                            foreach (SteamJuegoLogrosAPILogro juegoLogroAPI in juegoLogrosAPI.Datos.Datos2.Logros)
                                            {
                                                Logro logro = new Logro
                                                {
                                                    ID = juegoLogroAPI.NombreAPI,
                                                    Nombre = juegoLogroAPI.NombreMostrar,
                                                    Descripcion = juegoLogroAPI.Descripcion
                                                };

                                                foreach (SteamJugadorLogrosAPIDatosLogro jugadorLogroAPI in jugadorLogrosAPI.Datos.Logros)
                                                {
                                                    if (jugadorLogroAPI.NombreAPI == juegoLogroAPI.NombreAPI)
                                                    {
                                                        logro.Estado = jugadorLogroAPI.Estado;
                                                        
                                                        if (logro.Estado == "1")
                                                        {
                                                            logro.Imagen = juegoLogroAPI.IconoCompletado;
                                                        }
                                                        else
                                                        {
                                                            logro.Imagen = juegoLogroAPI.IconoPendiente;
                                                        }

                                                        break;
                                                    }
                                                }

                                                logros.Add(logro);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Logro logro in logros)
                                    {
                                        foreach (SteamJugadorLogrosAPIDatosLogro jugadorLogroAPI in jugadorLogrosAPI.Datos.Logros)
                                        {
                                            if (jugadorLogroAPI.NombreAPI == logro.NombreAPI)
                                            {
                                                logro.Estado = jugadorLogroAPI.Estado;

                                                if (logro.Estado == "1")
                                                {
                                                    logro.Imagen = logro.IconoCompletado;
                                                }
                                                else
                                                {
                                                    logro.Imagen = logro.IconoPendiente;
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }     
                }
            }

            return logros;
        }
    }

    //----------------------------------------------

    public class Logro
    {
        public string ID { get; set; }
        public string Estado { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Fecha { get; set; }
    }

    //----------------------------------------------

    public class SteamJugadorLogrosAPI
    {
        [JsonProperty("playerstats")]
        public SteamJugadorLogrosAPIDatos Datos { get; set; }
    }

    public class SteamJugadorLogrosAPIDatos
    {
        [JsonProperty("success")]
        public bool Estado { get; set; }

        [JsonProperty("steamID")]
        public string UsuarioID { get; set; }

        [JsonProperty("gameName")]
        public string TituloJuego { get; set; }

        [JsonProperty("gameName")]
        public List<SteamJugadorLogrosAPIDatosLogro> Logros { get; set; }
    }

    public class SteamJugadorLogrosAPIDatosLogro
    {
        [JsonProperty("apiname")]
        public string NombreAPI { get; set; }

        [JsonProperty("achieved")]
        public string Estado { get; set; }

        [JsonProperty("unlocktime")]
        public string Fecha { get; set; }
    }

    //----------------------------------------------

    public class SteamJuegoLogrosAPI
    {
        [JsonProperty("playerstats")]
        public SteamJuegoLogrosAPIDatos Datos { get; set; }
    }

    public class SteamJuegoLogrosAPIDatos
    {
        [JsonProperty("gameName")]
        public string TituloJuego { get; set; }

        [JsonProperty("availableGameStats")]
        public SteamJuegoLogrosAPIDatos2 Datos2 { get; set; }
    }

    public class SteamJuegoLogrosAPIDatos2
    {
        [JsonProperty("achievements")]
        public List<SteamJuegoLogrosAPILogro> Logros { get; set; }
    }

    public class SteamJuegoLogrosAPILogro
    {
        [JsonProperty("name")]
        public string NombreAPI { get; set; }

        [JsonProperty("displayName")]
        public string NombreMostrar { get; set; }

        [JsonProperty("description")]
        public string Descripcion { get; set; }

        [JsonProperty("icon")]
        public string IconoCompletado { get; set; }

        [JsonProperty("icongray")]
        public string IconoPendiente { get; set; }
    }
}
