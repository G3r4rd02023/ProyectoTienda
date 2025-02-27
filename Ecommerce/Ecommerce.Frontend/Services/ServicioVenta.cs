﻿using Ecommerce.Frontend.Models;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Enums;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.Frontend.Services
{
    public class ServicioVenta : IServicioVenta
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioProducto _producto;

        public ServicioVenta(IHttpClientFactory httpClientFactory, IServicioProducto servicioProducto)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/");
            _producto = servicioProducto;
        }

        public async Task<bool> ActualizarVentaTemporalAsync(VentaTemporal ventaTemporal)
        {
            var json = JsonConvert.SerializeObject(ventaTemporal);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Temporales/{ventaTemporal.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<Response> CancelarVenta(int id)
        {
            var ventas = await ObtenerVentasAsync();
            Venta venta = ventas.FirstOrDefault(x => x.Id == id)!;

            foreach (DetalleVenta detalle in venta.DetallesVenta!)
            {
                Producto? producto = await _producto.BuscarProductoAsync(detalle.Producto!.Id, detalle.Venta!.Usuario!.Correo!);
                if (producto != null)
                {
                    producto.Stock += detalle.Cantidad;
                }
            }

            venta.EstadoPedido = EstadoPedido.Cancelado;
            await ActualizarVentaAsync(venta);
            return new Response { IsSuccess = true };
        }

        public async Task<bool> EliminarVentaTemporalAsync(VentaTemporal ventaTemporal)
        {
            var response = await _httpClient.DeleteAsync($"/api/Temporales/{ventaTemporal.Id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> GuardarVentaTemporalAsync(VentaTemporal ventaTemporal)
        {
            var json = JsonConvert.SerializeObject(ventaTemporal);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Temporales/", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> GuardarVentaAsync(Venta venta)
        {
            var json = JsonConvert.SerializeObject(venta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Ventas/", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<int> ObtenerCantidad(string usuario)
        {
            var response = await _httpClient.GetAsync("/api/Temporales");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ventasTemporales = JsonConvert.DeserializeObject<IEnumerable<VentaTemporal>>(content)!;
                var carrito = ventasTemporales.Where(v => v.Usuario!.Correo == usuario).ToList();
                return carrito.Count;
            }
            return 0;
        }

        public async Task<IEnumerable<VentaTemporal>> ObtenerTemporalesAsync()
        {
            var response = await _httpClient.GetAsync("/api/Temporales");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<VentaTemporal>>(content)!;
            }
            return [];
        }

        public async Task<VentaTemporal> ObtenerVentaTemporalAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Temporales/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<VentaTemporal>(content)!;
            }
            return null!;
        }

        public async Task<Response> ProcesarVenta(CartViewModel model)
        {
            Response response = await ConfirmarInventario(model);
            if (!response.IsSuccess)
            {
                return response;
            }

            Venta venta = new()
            {
                Fecha = DateTime.Now,
                Usuario = model.Usuario,
                Comentario = model.Comentario,
                DetallesVenta = [],
                EstadoPedido = EstadoPedido.Nuevo
            };

            foreach (VentaTemporal item in model.VentasTemporales!)
            {
                venta.DetallesVenta.Add(new DetalleVenta
                {
                    Producto = item.Producto,
                    Cantidad = item.Cantidad,
                    Comentario = item.Comentario,
                });

                Producto? producto = await _producto.BuscarProductoAsync(item.Producto!.Id, item.Usuario!.Correo);
                if (producto != null)
                {
                    producto.Stock -= item.Cantidad;
                    await _producto.ActualizarProductoAsync(producto, item.Usuario.Correo);
                }
                await EliminarVentaTemporalAsync(item);
            }

            await GuardarVentaAsync(venta);
            return response;
        }

        private async Task<Response> ConfirmarInventario(CartViewModel model)
        {
            Response response = new() { IsSuccess = true };
            foreach (VentaTemporal item in model.VentasTemporales!)
            {
                Producto? producto = await _producto.BuscarProductoAsync(item.Producto!.Id, item.Usuario!.Correo);
                if (producto == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"El producto {item.Producto.Nombre}, ya no está disponible";
                    return response;
                }
                if (producto.Stock < item.Cantidad)
                {
                    response.IsSuccess = false;
                    response.Message = $"Lo sentimos no tenemos existencias suficientes del producto {item.Producto.Nombre}, Unidades disponibles: {item.Producto.Stock}";
                    return response;
                }
            }
            return response;
        }

        public async Task<IEnumerable<Venta>> ObtenerVentasAsync()
        {
            var response = await _httpClient.GetAsync("/api/Ventas");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Venta>>(content)!;
            }
            return [];
        }

        public async Task<Venta> ObtenerVentaAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Ventas/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Venta>(content)!;
            }
            return null!;
        }

        public async Task<bool> ActualizarVentaAsync(Venta venta)
        {
            var json = JsonConvert.SerializeObject(venta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/Ventas/{venta.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<VentasViewModel>> ObtenerResumenVenta()
        {
            var response = await _httpClient.GetAsync("/api/Ventas/Resumen");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resumen = JsonConvert.DeserializeObject<List<VentasViewModel>>(content);
                return resumen!;
            }
            return [];
        }
    }
}